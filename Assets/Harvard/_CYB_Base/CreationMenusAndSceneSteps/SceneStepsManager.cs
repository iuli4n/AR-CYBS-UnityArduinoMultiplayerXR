using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SceneStepsManager : MonoBehaviour
{
    // DESCRIPTION: This class is in charge of spawning a scene over the network, and cleaning/switching to the prev/next scene

    // TODO: Get rid of all the old code about notNetworked; and related, Prefab Saving code should be here not separate
    // TODO: Object instantiation should be done in a different class, to make it cleaner

    public static readonly string PREFABNAME_BASELINE = "BaseLineDrawing";
    
    public static SceneStepsManager Instance = null;

    public bool showGUI = false;
    public bool disableAutoSave = true;
    public bool loadProjectOnStart = true;
    
    //public bool DEBUG_usingNotNetworkedScene = true;
    public PrefabLoadSave DEBUG_prefabLoadSaveHelper = null;


    public enum SceneState
    {
        NotInitialized,
        Idle,
        Doing,
        WaitingForDone
    }
    public SceneState currentState;

    public GameObject currentSceneRoot = null;

    
    
    
    
    private void Awake()
    {
        Debug.Assert(Instance == null, "Should not have multiple instances of this object!");
        Instance = this;

        currentState = SceneState.NotInitialized;

        Debug.Assert(currentSceneRoot != null, "We need a root object to hold all the scene objects");
    }

    /***
    private IEnumerator DebugNotNetworked_LoadSceneWhenReady()
    {
        while (!PhotonNetwork.InRoom)
        {
            yield return new WaitForSeconds(.2f);
        }

        StudentProjectSceneManager.Instance.SwitchToProject(StudentProjectSceneManager.Instance.GetProjectNames()[0]);
        StudentProjectSceneManager.Instance.SwitchToScene(0);
        currentSceneRoot = DEBUG_notNetworkedSceneLoadSave.InstantiateFromPrefab();
    }
    ****/

    public void OpenProject(string projectName)
    {
        // TODO: this should be a PC only rpc
        
        // TODO:BUG: there might be an open scene right now

        // TODO: Close project if already inside one

#if UNITY_EDITOR

        //isInProject = true;
        StartCoroutine(PC_LoadFirstSceneWhenReady(projectName));

#endif
    }

    public void CloseProject(bool willLoadNewProject)
    {
        // TODO: this should be a PC only rpc call, and there should be a delay in it because objects are destroyed in network

        /**
        if (!willLoadNewProject)
        {
            // this already happens inside loading a new project, so don't do it if we're going to load
            ClearScene();
            currentSceneIndex = -1;
        }
        ***/

        // close scene [

        // TODO ***** this won't work properly unless this is done in an RPC that gets sent to everyone]
        if (StudentProjectSceneManager.Instance.CurrentSceneIndex != -1)
        {
            CleanLocalStuffFromScene();
            PC_CleanAndSaveNetworkObjectsFromScene(true, () =>
            {
                StudentProjectSceneManager.Instance.Clear();
                // TODO ****** SEND OUT THE RPC
            }
            );
        }
        
    }

    IEnumerator PC_LoadFirstSceneWhenReady(string projectName)
    {
#if !UNITY_EDITOR
        Debug.LogError("SHOULD NOT BE HERE");
        yield break;
#endif
        
        while (!PhotonNetwork.InRoom)
        {
            yield return new WaitForSeconds(.2f);
        }

        PhotonView.Get(this).RPC("RPC_LoadScene", RpcTarget.All, projectName, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (loadProjectOnStart) {
            // This will open the project after photon is initialized
            OpenProject(GetProjectNames()[0]);
        }
    }

    // Get all the names of student projects. Use OpenProject to open them.
    public string[] GetProjectNames()
    {
        return StudentProjectSceneManager.Instance.GetProjectNames();
    }



    private System.Action callbackWhenServerDone;


    [PunRPC]
    private void RPC_FromPC_ActionDone(string currentProjectName, int currentSceneIndex)
    {
        //currentProject
        //currentSceneIndex = currentSceneIndex;

        Debug.LogWarning("SCENE SWITCH TO " + currentProjectName + " / " + currentSceneIndex);

        StudentProjectSceneManager.Instance.SwitchToProject(currentProjectName);
        StudentProjectSceneManager.Instance.SwitchToScene(currentSceneIndex);

        callbackWhenServerDone?.Invoke();
        callbackWhenServerDone = null;
    }







    [PunRPC]
    private void RPC_ALL_ClearScene(string onlyPrefabsNamed = "")
    {
        if (StudentProjectSceneManager.Instance.CurrentSceneIndex == -1)
            // nothing to do 
            return;


        // Go into waiting mode and set callback
        DebugUI_WorkingStatus.Instance.OpenProgressIndicator();
        callbackWhenServerDone = new System.Action(delegate () {
            DebugUI_WorkingStatus.Instance.CloseProgressIndicator();
            //currentState = SceneState.Idle;
        });


        CleanLocalStuffFromScene();
#if UNITY_EDITOR
        PC_CleanAndSaveNetworkObjectsFromScene(false, () =>
        {
            PhotonView.Get(this).RPC("RPC_FromPC_ActionDone", RpcTarget.All, 
                StudentProjectSceneManager.Instance.CurrentProjectName, StudentProjectSceneManager.Instance.CurrentSceneIndex);
        },
        onlyPrefabsNamed
        );
#endif
    }


    private void CleanLocalStuffFromScene()
    {
        // Clean up local stuff from the scene, then server cleans networked objects after it saves (if closing)


        // Clean up our local stuff from this scene
        if (StudentProjectSceneManager.Instance.CurrentSceneIndex != -1)
        {
            DetachFromCurrentScene();
            ClearLocalObjectsFromScene();
        }
    }

    private void PC_CleanAndSaveNetworkObjectsFromScene(bool andSave, System.Action nextAction, string onlyPrefabsNamed = "")
    {

#if !UNITY_EDITOR
        Debug.LogError("NOT SUPPOSED TO BE HERE");
        return;
#endif

        // if editor PC, then first save the scene then clean it and then spawn objects
        if (StudentProjectSceneManager.Instance.CurrentSceneIndex != -1)
        {
            System.Action afterSaveAction = () => {

                PC_ClearNetworkObjectsFromScene(nextAction, onlyPrefabsNamed);
            };

            if (andSave)
            {
                SaveCurrentScene(afterSaveAction);
            }

            else
            {
                afterSaveAction?.Invoke();
            }
        }
    }

    [PunRPC]
    private void RPC_LoadScene(string projectName, int newSceneNumber)
    {
        Debug.Log("Received RPC_LoadScene");
           
        // Go into waiting mode and set callback
        DebugUI_WorkingStatus.Instance.OpenProgressIndicator();
        callbackWhenServerDone = new System.Action(delegate () {
            DebugUI_WorkingStatus.Instance.CloseProgressIndicator();
            currentState = SceneState.Idle;
        });

        // close scene from older project (or current project)
        if (StudentProjectSceneManager.Instance.CurrentSceneIndex != -1)
        {
            CleanLocalStuffFromScene();

            // If not PC, then stop here. If pc, will remotely spawn things
#if UNITY_EDITOR
            Debug.Log("Load: First cleaning the scene...");
            PC_CleanAndSaveNetworkObjectsFromScene(true, () => {

                Debug.Log("Load: Done cleaning the scene, spawning next...");

                // After everything is deleted, load new scene and send the RPC
                PC_LoadNewSceneContent(projectName, newSceneNumber);

            }
            );
#endif
        } else
        {
            // Wasn't in any scene
            Debug.Log("Load: Wasn't in any scene, spawning from clean");

#if UNITY_EDITOR
            PC_LoadNewSceneContent(projectName, newSceneNumber);
#endif


        }
    }

    private void PC_LoadNewSceneContent(string projectName, int newSceneNumber)
    {
        // Load new project content if needed
        if (projectName != StudentProjectSceneManager.Instance.CurrentProjectName)
        {
            // TODO: if this is done with rpc ignore that rpc
            StudentProjectSceneManager.Instance.SwitchToProject(projectName);
        }

        StudentProjectSceneManager.Instance.SwitchToScene(newSceneNumber);


        /***
        if (false) //DEBUG_usingNotNetworkedScene)
        {
            ///////// NOT NETWORKED LOAD

            currentSceneRoot = DEBUG_notNetworkedSceneLoadSave.InstantiateFromPrefab();

        }
        else
        ****/
        {
            ///////// NETWORKED SCENE LOAD


            StartCoroutine(
                Coroutine_PC_SpawnAllObjectsFromPrefab(StudentProjectSceneManager.Instance.CurrentScenePrefabFromDisk,
                () => {
                    PhotonView.Get(this).RPC("RPC_FromPC_ActionDone", RpcTarget.All,
                        StudentProjectSceneManager.Instance.CurrentProjectName, StudentProjectSceneManager.Instance.CurrentSceneIndex);
                })
            );
            
        }
    }


    private void SaveCurrentScene(System.Action nextAction, bool forceSave = false)
    {
        ///// NETWORKED SAVE


#if UNITY_EDITOR

        if (!forceSave && disableAutoSave)
        {
            // skip saving
            nextAction?.Invoke();
            return;
        }

        DEBUG_prefabLoadSaveHelper.SaveToPrefab(currentSceneRoot, StudentProjectSceneManager.Instance.CurrentScenePrefabLocationFull, nextAction);

#endif
    }

    private void DetachFromCurrentScene()
    {
        // This stops the player UI related things (ex: drawings, undo)

        // NOTE:TODOMAYBE: ideally this should be done earlier before prefab saving, in case scene disconnection generates other objects that need to be saved

        if (PlayersManager.Instance != null && PlayersManager.Instance.localPlayerHead != null)
        {

            foreach (DrawingVR d in GameObject.FindObjectsOfType<DrawingVR>())
            {
                // force stop drawing thigns !
                d.Local_ReleaseCurrentDrawing();
            }

            foreach (DebugUI_UndoRedo u in GameObject.FindObjectsOfType<DebugUI_UndoRedo>())
            {
                u.UndoRedo_Reset();
            }
        }

        // before we used to kill current scene object

        //GameObject.Destroy(currentSceneObject);
        //currentSceneObject = null;
    }

    private void ClearLocalObjectsFromScene()
    {
        // TODO: REMOVE THIS FUNCTION ? There's no more local objects, everything is remote.
        Debug.LogWarning("TODO: SceneStepsManager.ClearLocalObjectsFromScene: Called but this function isn't needed anymore");
        return;


        List<GameObject> gl = new List<GameObject>();

        int c = currentSceneRoot.transform.childCount;
        for (int i = 0; i < c; i++)
        {

            /*****
            if (currentSceneRoot.transform.GetChild(i).gameObject.name == PREFABNAME_BASELINE)
            {
                Debug.Log("Deleting local object " + currentSceneRoot.transform.GetChild(i).gameObject.name);

                gl.Add(currentSceneRoot.transform.GetChild(i).gameObject);
            }
            *****/
        }
        foreach (GameObject o in gl)
        {
            GameObject.DestroyImmediate(o);
        }
    }
    private void PC_ClearNetworkObjectsFromScene(System.Action nextAction, string onlyPrefabsNamed = "")
    {
        
        List<GameObject> gl = new List<GameObject>();

        // PC only
        int c = currentSceneRoot.transform.childCount;
        for (int i = 0; i < c; i++)
        {
            // assume all remaining objects are network spawned, so destroy them
            GameObject o = currentSceneRoot.transform.GetChild(i).gameObject;

            if (onlyPrefabsNamed == "" || o.name == onlyPrefabsNamed)
            {
                o.GetComponent<PhotonView>().RequestOwnership();
                gl.Add(o);

            }
        }

        if (gl.Count == 0)
        {
            nextAction?.Invoke();
        }
        else
        {
            foreach (GameObject o in gl)
            {
                // TODO:BUG? maybe a better way is to lock, increase to how many objects we want deleted, then wait for a coroutine that processes all of them
                lock (this)
                {
                    stillLeftToDelete++;
                    //Debug.Log("Increased still to " + stillLeftToDelete);
                }

                StartCoroutine(DestroyWhenOwned(o.GetComponent<PhotonView>(), () =>
                {
                    lock (this)
                    {
                        stillLeftToDelete--;
                        //Debug.Log("Decreased still to " + stillLeftToDelete);
                    }
                    if (stillLeftToDelete == 0)
                    {
                        //Debug.Log("Still is at 0, invoking ");
                        nextAction?.Invoke();
                    }
                }));
            }
        }
    }

    int stillLeftToDelete = 0;

    // TODO: maybe move object creation/destruction to somewhere else like PlayersManager
    public static IEnumerator DestroyWhenOwned(PhotonView pv, System.Action callback = null)
    {
        int retries = 0;

        yield return new WaitForSeconds(0.5f);
        while (retries > 10 && !pv.IsMine)
        {
            Debug.LogWarning("SSM: Wanting to delete network object but still waiting for ownership on PV " + pv.gameObject.name);
            pv.RequestOwnership();
            retries++;
            yield return new WaitForSeconds(0.5f);
        }
        if (retries > 10) Debug.LogWarning("SSM: Gave up waiting for ownership on " + pv.gameObject.name);

        // we're out of the loop, meaning it's mine or we gave up; try destroying it
        PhotonNetwork.Destroy(pv.gameObject);

        callback?.Invoke();
    }


    public void Activate_MoveToNextScene()
    {
        PhotonView.Get(this).RPC("RPC_PC_MoveToNextScene", RpcTarget.All);
    }
    public void Activate_MoveToPrevScene()
    {
        PhotonView.Get(this).RPC("RPC_PC_MoveToPrevScene", RpcTarget.All);
    }
    
    public void FromClient_DeleteObject(PhotonView pv)
    {
        PhotonView.Get(this).RPC("RPC_ForPC_FromClientDeleteObject", RpcTarget.All, pv.ViewID);
    }

    [PunRPC]
    private void RPC_ForPC_FromClientDeleteObject(int photonViewID)
    {
#if UNITY_EDITOR
        // NOTE:POSSIBLE BUG: if we're invoked to destroy something, and then immediately we switch scene, that object might be attempted to be deleted multiple times
        PhotonView pv = PhotonView.Find(photonViewID);
        StartCoroutine(DestroyWhenOwned(pv));
#endif
    }


    [PunRPC]
    private void RPC_PC_MoveToNextScene()
    {
#if UNITY_EDITOR
        if (StudentProjectSceneManager.Instance.CurrentSceneIndex >= 
            StudentProjectSceneManager.Instance.pc_prefabScenes.Length - 1)
        {
            Debug.Log("At last scene; cannot move further.");
            return;
        }
        if (StudentProjectSceneManager.Instance.pc_prefabScenes.Length == 0)
        {
            Debug.Log("There were no prefab scenes found in the scene resources folder");
            return;
        }
        PhotonView.Get(this).RPC("RPC_LoadScene", RpcTarget.All, 
            StudentProjectSceneManager.Instance.CurrentProjectName, 
            StudentProjectSceneManager.Instance.CurrentSceneIndex + 1);
#endif
    }
    [PunRPC]
    private void RPC_PC_MoveToPrevScene()
    {
#if UNITY_EDITOR
        if (StudentProjectSceneManager.Instance.CurrentSceneIndex <= 0)
        {
            Debug.Log("At last scene; cannot move further.");
            return;
        }
        if (StudentProjectSceneManager.Instance.pc_prefabScenes.Length == 0)
        {
            Debug.Log("There were no prefab scenes found in the scene resources folder");
            return;
        }
        PhotonView.Get(this).RPC("RPC_LoadScene", RpcTarget.All, StudentProjectSceneManager.Instance.CurrentProjectName, StudentProjectSceneManager.Instance.CurrentSceneIndex - 1);
#endif
    }

    public void Activate_ClearScene(string onlyPrefabsNamed = "")
    {
        PhotonView.Get(this).RPC("RPC_ALL_ClearScene", RpcTarget.All, onlyPrefabsNamed);
    }










    // Creates the object from a prefab found in CreationPrefabs/* for everyone; then configures it to be part of the current scene (ex: made grabbable, etc)
    public GameObject CreateAndConfigureNewObjectForScene_ModelFromPrefab(string newObjectPrefabName, Vector3 tp, Quaternion tq, Vector3 ts)
    {
        string prefabContainerName = "InternalPrefabs/BaseObjectPrefabContainer";

        // first fix the path and then instantiate the prefab subobject
        if (!newObjectPrefabName.StartsWith("/") && !newObjectPrefabName.StartsWith("\\"))
        {
            newObjectPrefabName = "/" + newObjectPrefabName;
        }
        string filepath = "CreationPrefabs" + newObjectPrefabName;

        // First create this prefab for everyone; it'll then be hooked into a parent container
        GameObject subObject = PhotonNetwork.Instantiate(filepath, Vector3.one * 999f, Quaternion.identity);
        PhotonView subPhotonView = subObject.GetComponent<PhotonView>();

        subPhotonView.OwnershipTransfer = OwnershipOption.Takeover;
        int photonId = subPhotonView.ViewID;

        // Now instantiate the container (it will be linked to the subobject when it instantiates on every client)
        GameObject gg = PhotonNetwork.Instantiate(prefabContainerName, new Vector3(999f, 999f, 999f), tq, 0, 
            new object[] { newObjectPrefabName, ts, tp, 0, photonId});

        return gg;
    }

    // Creates a network image container; the image file is at imageRelPath.
    public GameObject CreateAndConfigureNewObjectForScene_Image(Vector3 tp, Quaternion tq, Vector3 ts, string imageRelPath)
    {
        string containerPrefab = "InternalPrefabs/BaseImage";
        string spawnedObjectName = "BaseImage";

        // this spawns the container; the actual content will be created when the container prefab instantiates
        GameObject gg = PhotonNetwork.Instantiate(containerPrefab, new Vector3(999f, 999f, 999f), tq, 0, new object[] { spawnedObjectName, ts, tp, 1, imageRelPath});
        return gg;
    }

    // Creates a network External 3D Model (ex: OBJ file) container; the model itself is at driveRelPath.
    public GameObject CreateAndConfigureNewObjectForScene_ModelFrom3DFile(Vector3 tp, Quaternion tq, Vector3 ts, string driveRelPath, Vector3 modelOffset)
    {
        string containerPrefab = "InternalPrefabs/BaseModelQuad";
        string spawnedObjectName = "BaseModelQuad";
        
        // this spawns the container; the actual content will be created when the container prefab instantiates
        GameObject gg = PhotonNetwork.Instantiate(containerPrefab, new Vector3(999f, 999f, 999f), tq, 0, new object[] {
            spawnedObjectName, ts, tp, 2, driveRelPath, modelOffset });
        return gg;
    }


    // Just spawn this one prefab for everyone
    public GameObject CreateAndConfigureNewObjectForScene_Special(string newObjectName, string fullPrefabName, Vector3 tp, Quaternion tq, Vector3 ts)
    {
        GameObject gg = PhotonNetwork.Instantiate(fullPrefabName, new Vector3(999f, 999f, 999f), tq, 0, new object[] { newObjectName, ts, tp, 99, newObjectName });
        return gg;
    }
    
    // Used when starting a drawing; this spawns the container for the drawing path
    public GameObject CreateAndConfigureNewObjectForScene_BaseLineDrawing(string newObjectName, string fullPrefabName, Vector3 tp, Quaternion tq, Vector3 ts, Vector3[] positions = null)
    {
        if (positions == null)
        {
            positions = new Vector3[0];
        }
        GameObject gg = PhotonNetwork.Instantiate(fullPrefabName, new Vector3(999f, 999f, 999f), tq, 0, new object[] { newObjectName, ts, tp, 3, positions});
        return gg;
    }



    private IEnumerator Coroutine_PC_SpawnAllObjectsFromPrefab(GameObject prefa, System.Action doneAction)
    {
        int c = prefa.transform.childCount;
        for (int i = 0; i < c; i++)
        {
            GameObject g = prefa.transform.GetChild(i).gameObject;
            if (g.GetComponent<PlayerCreatedPrefab>())
            {
                SpawnSavedScenePrefabPart_PCPrefab(g);
            } 
            else if (g.GetComponent<PlayerCreatedImage>())
            {
                SpawnSavedScenePrefabPart_PCImage(g);
            }
            else if (g.GetComponent<PlayerCreatedModel>())
            {
                SpawnSavedScenePrefabPart_PCModel(g);
            } else if (g.name == PREFABNAME_BASELINE)
            {
                SpawnSavedScenePrefabPart_BaseLineDrawing(g);
            }
            /* else if (g.GetComponent<TooltipManager>())
            {

            }*/
            yield return new WaitForSeconds(0.01f);
        }

        // do callback at the end
        if (doneAction != null)
            doneAction();
    }
    void SpawnSavedScenePrefabPart_PCPrefab(GameObject objectFromPrefab)
    {
        // Creates an object that's coming from our saved scene

        // these are the positions/scales/rotation inside the prefab !
        Vector3 tp = objectFromPrefab.transform.position;
        Vector3 ts = objectFromPrefab.transform.localScale;
        Quaternion tq = objectFromPrefab.transform.rotation;
        GameObject newObject = null;
        
        PlayerCreatedPrefab pcp = objectFromPrefab.GetComponent<PlayerCreatedPrefab>();
        string modelPrefabName = pcp.GetModelName();
        newObject = CreateAndConfigureNewObjectForScene_ModelFromPrefab(modelPrefabName, tp, tq, ts);
        // NOTE: There may be a delay while the initialization RPC is called after instantiating this
        
        if (newObject == null)
        {
            Debug.LogError("Could not create object from scene prefab " + modelPrefabName);
            // this object couldn't be instantiated from prefab (is probably drawing)
            return;
        }

        // Now set child data according to the prefab; this will be networked through the individual models
        SpawnSavedScenePrefabPart_SetSubmodels(objectFromPrefab, newObject);
    }

    void SpawnSavedScenePrefabPart_PCImage(GameObject objectFromPrefab)
    {
        // Creates an object that's coming from our saved scene

        // these are the positions/scales/rotation inside the prefab !
        Vector3 tp = objectFromPrefab.transform.position;
        Vector3 ts = objectFromPrefab.transform.localScale;
        Quaternion tq = objectFromPrefab.transform.rotation;
        GameObject newObject = null;
        
        PlayerCreatedImage pci = objectFromPrefab.GetComponent<PlayerCreatedImage>();
        string imagePath = pci.GetImagePath();


        // just a regular object
        newObject = CreateAndConfigureNewObjectForScene_Image(tp, tq, ts, imagePath);
        // NOTE: There may be a delay while the initialization RPC is called after instantiating this

        if (newObject == null)
        {
            Debug.LogError("Could not create object from scene prefab " + imagePath);
            // this object couldn't be instantiated from prefab (is probably drawing)
            return;
        }

        // Now set child data according to the prefab; this will be networked through the individual models
        SpawnSavedScenePrefabPart_SetSubmodels(objectFromPrefab, newObject);

    }

    void SpawnSavedScenePrefabPart_PCModel(GameObject objectFromPrefab)
    {
        // Creates an object that's coming from our saved scene

        // these are the positions/scales/rotation inside the prefab !
        Vector3 tp = objectFromPrefab.transform.position;
        Vector3 ts = objectFromPrefab.transform.localScale;
        Quaternion tq = objectFromPrefab.transform.rotation;
        GameObject newObject = null;


        PlayerCreatedModel pcm = objectFromPrefab.GetComponent<PlayerCreatedModel>();
        string filePath = pcm.GetModelPath();
        Vector3 modelOffset = pcm.GetModelOffset();

        newObject = CreateAndConfigureNewObjectForScene_ModelFrom3DFile(tp, tq, ts, filePath, modelOffset);
        // NOTE: There may be a delay while the initialization RPC is called after instantiating this

        if (newObject == null)
        {
            Debug.LogError("Could not create object from scene prefab " + filePath);
            // this object couldn't be instantiated from prefab (is probably drawing)
            return;
        }

        // Now set child data according to the prefab; this will be networked through the individual models
        SpawnSavedScenePrefabPart_SetSubmodels(objectFromPrefab, newObject);

    }

    void SpawnSavedScenePrefabPart_BaseLineDrawing(GameObject objectFromPrefab)
    {
        // these are the positions/scales/rotation inside the prefab !
        Vector3 tp = objectFromPrefab.transform.position;
        Vector3 ts = objectFromPrefab.transform.localScale;
        Quaternion tq = objectFromPrefab.transform.rotation;
        GameObject newObject = null;

        string photonPrefabName = "InternalPrefabs/" + PREFABNAME_BASELINE;


        Vector3[] positions = new Vector3[objectFromPrefab.GetComponent<PlayerCreatedDrawing>().drawingLine.positionCount];
        objectFromPrefab.GetComponent<PlayerCreatedDrawing>().drawingLine.GetPositions(positions);

        newObject = CreateAndConfigureNewObjectForScene_BaseLineDrawing(
            objectFromPrefab.name, photonPrefabName, tp, tq, ts, positions);


        if (newObject == null)
        {
            Debug.LogError("Could not create object from scene prefab " + photonPrefabName);
            // this object couldn't be instantiated from prefab (is probably drawing)
            return;
        }

        SpawnSavedScenePrefabPart_SetSubmodels(objectFromPrefab, newObject);
    }





    private void SpawnSavedScenePrefabPart_SetSubmodels(GameObject objectFromPrefab, GameObject newObject)
    {
        // switches
        {
            AtomicDataSwitch[] gs = objectFromPrefab.GetComponentsInChildren<AtomicDataSwitch>();
            AtomicDataSwitch[] ggs = newObject.GetComponentsInChildren<AtomicDataSwitch>();
            Debug.Assert(gs.Length == ggs.Length, "Spawn networked scene prefab: did not find the same components in the spawned object !");
            for (int i = 0; i < gs.Length; i++)
            {
                ggs[i].Value = gs[i].Value;
                ggs[i].CurrentChannel = gs[i].CurrentChannel;
                Debug.Log("Set to channel " + ggs[i].CurrentChannel);
            }
        }
        // models
        {
            AtomicDataModel[] gs = objectFromPrefab.GetComponentsInChildren<AtomicDataModel>();
            AtomicDataModel[] ggs = newObject.GetComponentsInChildren<AtomicDataModel>();
            Debug.Assert(gs.Length == ggs.Length, "Spawn networked scene prefab: did not find the same components in the spawned object !");
            for (int i = 0; i < gs.Length; i++)
            {
                ggs[i].Value = gs[i].Value;
            }
        }

        /*** TODO: load content of effectmodel
        {
            Effect4Model[] e4m = objectFromPrefab.GetComponentsInChildren<Effect4Model>();
            Effect4Model[] ee4m = newObject.GetComponentsInChildren<Effect4Model>();
            Debug.Assert(e4m.Length == ee4m.Length, "Spawn networked scene prefab: did not find the same components in the spawned object !");
            for (int i = 0; i < e4m.Length; i++)
            {
                ee4m[i].LoadFromPrefabModel(e4m[i]);
            }
            
        }
        ***/
    }





    private void OnApplicationQuit()
    {
        SaveCurrentScene( ()=> { });
    }



    // Update is called once per frame
    void OnGUI()
    {
        if (!showGUI)
            return;

        GUILayout.BeginVertical();

        //GUILayout.Label(".");
        //GUILayout.FlexibleSpace();



        GUILayout.Space(Screen.height * 0.6f);

        if (GUILayout.Button("CLEAR ALL"))
            SceneStepsManager.Instance.Activate_ClearScene();

        if (GUILayout.Button("CLEAR LINES"))
            SceneStepsManager.Instance.Activate_ClearScene(PREFABNAME_BASELINE);




        if (GUILayout.Button("< SAVE >"))
            SaveCurrentScene(() => { }, true);

        if (GUILayout.Button(" << PREV"))
            SceneStepsManager.Instance.Activate_MoveToPrevScene();

        if (GUILayout.Button("NEXT >> "))
            SceneStepsManager.Instance.Activate_MoveToNextScene();

        


        /***
        if (GUILayout.Button("SAVE TEMP SCENE"))
            DEBUG_prefabLoadSaveHelper.SaveToPrefab(
                currentSceneRoot, 
                StudentProjectSceneManager.Instance.GetTempSceneDiskLocation(),
                () => { });

        if (GUILayout.Button("LOAD TEMP SCENE"))
            PhotonView.Get(this).RPC(
                "RPC_LoadScene", RpcTarget.All,
                StudentProjectSceneManager.Instance.CurrentProjectName,
                StudentProjectSceneManager.Instance.TempSceneIndex);
        ***/

        GUILayout.EndVertical();

    }
}
