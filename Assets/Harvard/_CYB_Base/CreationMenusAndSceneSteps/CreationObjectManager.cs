using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;

public class CreationObjectManager : MonoBehaviourPun
{
    // BUGS: when networked, auto open will cause RPC from all the users (or when a user logs in). same for image menu

    // TODO:PRETTY: Don't use ButtonScript or ChangeLabel scripts, instead just make a dynamic button and set its listener here


    private Object[] objects;
    public GameObject menuParentPrefab;

    public GameObject spawnPosition;

    // TODO: Make this private and fix the hand menu so it doesn't need it
    public GameObject menuParent;
    // TODO: Make this private and fix the hand menu so it doesn't need it
    public bool neverOpened;
    public GameObject referenceSize;

    public GameObject addButton;
    public GameObject locationPlaceholder; // an empty object for determining the next shortcut button location

    /*
    [SerializeField]
    GameObject effectsMenuLaunchButtonprefab;
    [SerializeField]
    GameObject effectsMenuPrefab;
    */

    private Vector3 newLinePos;
    private Vector3 buttonPos;
    private Vector3 startPos;
    private int newButtonID;
    List<GameObject> thumbnailList;
    public int objectListStart;
    public int objectListEnd;
    public int endpt;
    List<GameObject> objectList;
	public bool isClosed = true;
	[SerializeField]
    GameObject buttons;
    GameObject leftButton;
    GameObject rightButton;

    TextMeshPro buttonstextmesh;


    public bool autoOpen = true;
    private void Start()
    {
        buttonstextmesh = buttons.transform.Find("IconAndText/TextMeshPro").GetComponent<TextMeshPro>();

        LoadAllObjects();
        objectListStart = 0;
        objectListEnd = 9;
        AdjustListEndpt();

        //  menuParent = new GameObject("MENU");
        neverOpened = true;
        thumbnailList = new List<GameObject>();
        objectList = new List<GameObject>();
        newButtonID = 0;

        buttonPos = locationPlaceholder.transform.position;
        startPos = buttonPos;
        newLinePos = buttonPos;

        leftButton = buttons.transform.Find("leftObjectButton").gameObject;
        rightButton = buttons.transform.Find("rightObjectButton").gameObject;

        if (autoOpen)
        {
            StartCoroutine(AutoOpenNow());
        }
    }

    public IEnumerator AutoOpenNow()
    {
        // wait till the network initializes
        while (PlayersManager.Instance == null || PlayersManager.Instance.localPlayerFingerPenTip == null)
        {
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(1f);

        if (isClosed)
        {
            OpenMenu();
        }
    }

    private void Update()
    {
        // "O" keypress opens the MENU
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (isClosed)
            {
                OpenMenu();
            } else
            {
                CloseMenu();
            }
        }
    }


    // Loads all objects in the "Resources/Objects" folder
    public void LoadAllObjects()
    {
        objects = Resources.LoadAll("CreationPrefabs", typeof(GameObject));
        Debug.Log("Number of objects: "+objects.Length);
    }

    // Instantiates all menu buttons and associated 3D objects then calls RearrangeButtons()
    public void OpenMenu()
    {
        PhotonView.Get(this).RPC("RPC_OpenMenu", RpcTarget.All);
    }
    [PunRPC]
    public void RPC_OpenMenu()
    {
        thumbnailList = new List<GameObject>();
        objectList = new List<GameObject>();

        buttons.SetActive(true);

        buttonPos = locationPlaceholder.transform.position;
        startPos = buttonPos;
        newLinePos = buttonPos;


        // BUG? If networking might want to instantiate these
        GameObject newMenuParent = (GameObject)Instantiate(menuParentPrefab, 
            startPos + Vector3.right * .15f + Vector3.up * .15f, Quaternion.Euler(-90, 0, 0));

        newMenuParent.transform.parent = this.transform;

        menuParent = newMenuParent;

        /**
        buttons.transform.parent = menuParent.transform;
        buttons.transform.localPosition = new Vector3(-1.13f, 0.57f, -56.4f);
        buttons.transform.localScale = new Vector3(5.57f, -38.69f, -1.3f);
        buttons.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        ***/

        // MAKE BUTTONS

        for (int i = 0; i < objects.Length; i++)
        {
            if (i >= objectListStart && i < endpt)
            {
                Object o = objects[i];
                GameObject newButton = (GameObject)Instantiate(addButton, buttonPos, Quaternion.identity);
                newButton.transform.parent = menuParent.transform;
                //newButton.transform.localRotation.SetEulerAngles(-90, 90, 0);
                //newButton.name = newButtonID.ToString();
                newButton.name = o.name;



                if (newButton != null)
                {
                    thumbnailList.Add(newButton);
                }

                GameObject newObject = (GameObject)Instantiate(o, buttonPos + Vector3.forward * .05f, Quaternion.identity);
                newObject.transform.localScale *= 5f;
                
                // Delete photon stuff from this object because we won't need it
                foreach (var c in newObject.GetComponentsInChildren<PhotonTransformView>())
                    GameObject.Destroy(c);
                foreach (var c in newObject.GetComponentsInChildren<PhotonView>())
                    GameObject.Destroy(c);
                foreach (var c in newObject.GetComponentsInChildren<AtomicDataSwitch>())
                    GameObject.Destroy(c);
                foreach (var c in newObject.GetComponentsInChildren<PlayerCreatedObject>())
                    GameObject.Destroy(c);
                foreach (var c in newObject.GetComponentsInChildren<PlayerCreatedModel>())
                    GameObject.Destroy(c);
                foreach (var c in newObject.GetComponentsInChildren<PlayerCreatedImage>())
                    GameObject.Destroy(c);


                //newObject.transform.parent = newButton.transform;
                newObject.transform.localScale = Vector3.one*0.05f*0.25f;

                //newObject.tag = "MenuObject";
                newObject.transform.parent = menuParent.transform;
                newObject.transform.rotation = Quaternion.identity;


                var iconAndText = newButton.transform.Find("IconAndText");
                var textMeshP = iconAndText.transform.Find("TextMeshPro");
                textMeshP.gameObject.SetActive(true);

                var tmp = textMeshP.gameObject.GetComponent<TextMeshPro>();

                if (o.name.Length < 10)
                {
                    tmp.text = o.name;
                }
                else
                {
                    tmp.text = o.name.Substring(0, 9) + "...";
                }



                newObject.name = o.name; // newButtonID.ToString();

                if (newObject != null)
                {
                    objectList.Add(newObject);
                }

                newButtonID++;

                CreationButtonScript newButtonScript = newButton.GetComponentInChildren<CreationButtonScript>();

                newButtonScript.myObjectManager = this;
            }
                

        }
        RearrangeButtons();
        UpdatePageNumberLabel();
        menuParent.transform.localRotation = Quaternion.Euler(-90, 0, 0);

        buttons.transform.parent = menuParent.transform;
        buttons.transform.localPosition = new Vector3(-3f, 0f, -95.6f);

        isClosed = false;

        //neverOpened = false;
    }
        

    public void CloseMenu()
    {
        PhotonView.Get(this).RPC("RPC_CloseMenu", RpcTarget.All);
    }
    [PunRPC]
    public void RPC_CloseMenu() {
        locationPlaceholder.transform.position = menuParent.transform.position - (Vector3.right * .15f + Vector3.up * .15f);

        buttons.transform.parent = transform;
        buttons.SetActive(false);
        Destroy(menuParent);
        isClosed = true;
    }

    /*
    // Resizes the created object using the referenceSize object
    public void ResizeObject(GameObject obj, float scaleFactor)
    {
        Vector3 sizeCalculated = referenceSize.GetComponent<Renderer>().bounds.size;
        obj.transform.localScale = sizeCalculated * scaleFactor;
    }
    */



    // Instantiates the associated object 
    // prefabName = prefab name of the object to be instantiated. The object is found in the CreationPrefabs/ folder
    public void ExecuteButtonPress(string prefabName)
    {
        GameObject newObject = SceneStepsManager.Instance.CreateAndConfigureNewObjectForScene_ModelFromPrefab(
            prefabName, //+ buttonName,
            spawnPosition.transform.position,
            spawnPosition.transform.rotation,
            Vector3.negativeInfinity); 
            
            //startPos + -1 * this.transform.right * 0.3f,
            //Quaternion.identity,
            //Vector3.one);
            //referenceSize.GetComponent<Renderer>().bounds.size * 1f);

        //EffectsMenu effectsMenu = newObject.AddComponent<EffectsMenu>();
        //effectsMenu.PlaceLaunchButton(effectsMenuLaunchButtonprefab, effectsMenuPrefab);

        CreationTipManager.Instance.OnPlayerHasJustMadeThis(newObject);
    }

    /*** was used for creating objects, but now creating is done elsewhere
    [PunRPC]
    private void RPC_CreateObject(string buttonName) 
    {
        //string prefabName = "Objects/" + buttonName;
        //GameObject newObject = PhotonNetwork.Instantiate(prefabName, startPos + Vector3.left * 0.3f, Quaternion.identity); //(GameObject)Instantiate(o, startPos + Vector3.left * 0.3f, Quaternion.identity);
        //newObject.name = buttonName;
        //ResizeObject(newObject, 1f);

        SceneStepsManager.ConfigureNewObjectForScene(newObject);
    }
    ****/

    //rearranges the menu buttons
    public void RearrangeButtons()
    {
        buttonPos = startPos;
        newLinePos = startPos;
        Debug.Log("Thumbnail list size: " + thumbnailList.Count);
        thumbnailList[0].transform.position = buttonPos;
        objectList[0].transform.position = buttonPos + Vector3.forward * .05f;

        Vector3 dirRight = thumbnailList[0].transform.right;
        Vector3 dirDown = thumbnailList[0].transform.up * -1;

        if (thumbnailList.Count > 0)
        {
            for (int i = 1; i < thumbnailList.Count; i++)
            {
                if (i % 3 != 0)
                {
                    buttonPos += Vector3.right * .15f;
                    thumbnailList[i].transform.position = buttonPos;
                    objectList[i].transform.position = buttonPos + Vector3.forward * .05f;
                }

                if (i % 3 == 0)
                {
                    buttonPos = newLinePos + Vector3.down * .15f;
                    newLinePos = buttonPos;
                    thumbnailList[i].transform.position = newLinePos;
                    objectList[i].transform.position = newLinePos + Vector3.forward * .05f;
                }
            }
        }
    }
    
    public void PressLeftButton()
    {
        Debug.Log("pressed left");
        if (objectListStart > 0)
        {
            Debug.Log("moving menu left");
            CloseMenu();
            objectListStart -= 9;
            objectListEnd -= 9;
            AdjustListEndpt();

            OpenMenu();

            if (objectListStart == 0)
            {
                leftButton.SetActive(false);
            }

            if (objectListEnd < objects.Length)
            {
                rightButton.SetActive(true);
            }
        }

        UpdatePageNumberLabel();
    }
    public void PressRightButton()
    {
        Debug.Log("pressed right");
        if (objectListEnd < objects.Length)
        {
            Debug.Log("moving menu right");
            CloseMenu();
            objectListStart += 9;
            objectListEnd += 9;
            AdjustListEndpt();

            OpenMenu();

            if (objectListEnd >= objects.Length)
            {
                rightButton.SetActive(false);
            }

            if (objectListStart > 0)
            {
                leftButton.SetActive(true);
            }
        }

        UpdatePageNumberLabel();
    }
    void AdjustListEndpt()
    {
        if (objects.Length < objectListEnd)
        {
            endpt = objects.Length;
        }
        else
        {
            endpt = objectListEnd;
        }
    }

    void UpdatePageNumberLabel()
    {
        //TextMeshPro textmesh = buttons.transform.Find("IconAndText/TextMeshPro").GetComponent<TextMeshPro>();
        buttonstextmesh.SetText("Page " + (objectListStart/ 9 + 1).ToString() + " of " + ((objects.Length - 1) / 9 + 1).ToString());
    }
}