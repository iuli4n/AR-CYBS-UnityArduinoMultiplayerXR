using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabLoadSave : MonoBehaviour
{
    // TODO: Absorb this into SceneStepsManager

    // BUGS: - This instantiates the prefab over network, so won't work when multiple people are here

    GameObject objectToSave;

    public bool showGUI = true;
    public string resourcesRootFolder = "Assets/Iulian_Temp/Resources";
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /*** was used for local only
    public GameObject InstantiateFromPrefab()
    {        
        
        string localfilepath = StudentProjectSceneManager.Instance.CurrentScenePrefabLocationShort; 
        //string localfilepath = "_saves/" + "SCENEROOT_NOTNETWORKED";
        Debug.Log("path: " + localfilepath);
        
        objectToSave = PhotonNetwork.Instantiate(localfilepath, Vector3.zero, Quaternion.identity);
        objectToSave.transform.parent = this.gameObject.transform;
        
        return objectToSave;
    }
    ***/





    public void SaveToPrefab(GameObject objectToSave, string localPrefabPath)
    {
        StartCoroutine(Coroutine_SavePrefab(objectToSave, localPrefabPath));
    }
    IEnumerator Coroutine_SavePrefab(GameObject objectToSave, string localPrefabPath) {

#if UNITY_EDITOR


        ////// DISABLE THINGS BEFORE SAVING ////////////
        ///


        // First disable the effects
        List<Effect4Gen> le = new List<Effect4Gen>();
        foreach (var pco in objectToSave.GetComponentsInChildren<PlayerCreatedObject>())
        {
            if (pco.GetComponent<Effect4Gen>())
            {
                Effect4Gen e = pco.GetComponent<Effect4Gen>();
                le.Add(e);
                e.DisableEffects();
            }
        }
         // give them time to refresh to base
        yield return new WaitForSeconds(0.5f);

        // Now disable the bounds
        List<BoundsControl> lb = new List<BoundsControl>();
        foreach (var pco in objectToSave.GetComponentsInChildren<PlayerCreatedObject>())
        {
            if (pco.GetComponent<BoundsControl>() && pco.GetComponent<BoundsControl>().enabled)
            {
                lb.Add(pco.GetComponent<BoundsControl>());

                pco.GetComponent<BoundsControl>().enabled = false;
                yield return new WaitForEndOfFrame();
                //pco.GetComponent<BoundsControl>().enabled = true;
            }

            if (pco.GetComponent<NearInteractionGrabbable>())
            {
                pco.GetComponent<NearInteractionGrabbable>().enabled = false;
            }
        }

        /***
        // Now delete all the leftover stuff from the boundscontrols
        foreach (var pco in objectToSave.GetComponentsInChildren<PlayerCreatedObject>())
        {
            List<GameObject> cdelete = new List<GameObject>();
            foreach (Transform c in pco.gameObject.transform)
            {
                if (c.gameObject.name == "rigRoot")
                {
                    cdelete.Add(c.gameObject);
                }
            }
            foreach (var c in cdelete)
            {
                GameObject.Destroy(c);
            }
        }
        ****/

        // Reenable the effects (so they use clean bounds?) 
        foreach (var e in le)
        {
            e.EnableEffects();
        }



        ////// SAVE TO PREFAB ////////////
        ///

        // Then save to prefab [note this might cause bugs if overriding through unity's system]
        ///string localfilepath = resourcesRootFolder + "/_saves/" + "SCENEROOT_NOTNETWORKED.prefab";

        PrefabUtility.SaveAsPrefabAsset(objectToSave, localPrefabPath);



        ////// REENABLE THINGS AGAIN ////////////
        ///

        // reenable the bounds
        foreach (var b in lb)
        {
            b.enabled = true;
        }

#endif
        yield return 0;
    }







    int backupCounter = 0;
    string backupString = "_backup0";

    private void OnGUI()
    {
        if (!showGUI) return;

        GUILayout.BeginVertical();
        GUILayout.Label(" ");
        GUILayout.Label(" ");
        GUILayout.Label(" ");
        GUILayout.Label(" ");
        GUILayout.Label(" ");

        if (GUILayout.Button("FORCE SAVE"))
        {
            Debug.LogError("TODO: Move to old RPC from SceneStepsManager");

            SaveToPrefab(SceneStepsManager.Instance.currentSceneRoot, StudentProjectSceneManager.Instance.CurrentScenePrefabLocationFull);
        }
        if (GUILayout.Button("FORCE BSAVE "+ backupString))
        {
            Debug.LogError("TODO: Move to old RPC from SceneStepsManager");

            backupCounter++;
            backupString = "_backup" + backupCounter;

            string filename = StudentProjectSceneManager.Instance.CurrentScenePrefabLocationFull;

            filename = filename.Substring(0, filename.Length - ".prefab".Length);
            filename = filename + backupString + ".prefab";

            SaveToPrefab(SceneStepsManager.Instance.currentSceneRoot, 
                filename);
        }
        if (GUILayout.Button("CLEAR"))
        {
            Debug.LogError("TODO: Move to old RPC from SceneStepsManager");
            var objectToSave = SceneStepsManager.Instance.currentSceneRoot;

            List<GameObject> lg = new List<GameObject>();
            int l = objectToSave.transform.childCount;
            for (int i = 0; i < l; i++)
                lg.Add(objectToSave.transform.GetChild(i).gameObject);
            foreach (GameObject g in lg)
            {
                PhotonNetwork.Destroy(g);
            }
        }
        if (GUILayout.Button("refresh bounds"))
        {
            var objectToSave = SceneStepsManager.Instance.currentSceneRoot;

            foreach (var pco in objectToSave.GetComponentsInChildren<PlayerCreatedObject>())
            {
                if (pco.GetComponent<BoundsControl>() && pco.GetComponent<BoundsControl>().enabled)
                {
                    pco.GetComponent<BoundsControl>().enabled = false;
                    pco.GetComponent<BoundsControl>().enabled = true;
                }
            }
        }
        GUILayout.EndVertical();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
