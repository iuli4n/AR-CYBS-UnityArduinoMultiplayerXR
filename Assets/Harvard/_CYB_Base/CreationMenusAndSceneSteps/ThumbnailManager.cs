using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using Photon.Pun;

public class ThumbnailManager : MonoBehaviour
{
    // BUGS: when networked, auto open will cause RPC from all the users (or when a user logs in). same for image menu

    [SerializeField]
    GameObject nav;
    GameObject leftButton;
    GameObject rightButton;
    public int totalNumPages;
    int currentPageNum;
    public int imageListStart;
    public int imageListEnd;
    public int endpt;

    public static ThumbnailManager Instance = null;

    private Object[] textures;
    private string[] texturespaths;

    public GameObject menuParentPrefab;
    private GameObject menuParent;
    //private bool neverOpened;

    public GameObject thumbnailButton;
    public GameObject quadImage;
    public GameObject thumbnailLocation; // an empty object for determining the next shortcut button location

    private Vector3 newLinePos;
    private Vector3 buttonPos;
    private Vector3 startPos;
    private int newButtonID;
    private string buttonID;
    List<GameObject> thumbnailList;
   
    public bool autoOpen = true;

    public GameObject locationPlaceholder;

    private void Start()
    {
        Debug.Assert(Instance == null, "Expected this object to be a singleton");
        Instance = this;

        menuParent = new GameObject("MENU");
        //neverOpened = true;
        
        newButtonID = 0;

        buttonPos = thumbnailLocation.transform.position;
        startPos = buttonPos;
        newLinePos = buttonPos;

        imageListStart = 0;
        imageListEnd = 9;
        Refresh();

        leftButton = nav.transform.Find("ImageMenuLeftButton").gameObject;
        rightButton = nav.transform.Find("ImageMenuRightButton").gameObject;

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

    bool isClosed = true;

    public void Refresh()
    {
        LoadAllTextures();
        //imageListStart = 0;
        //imageListEnd = 9;
        AdjustListEndpt();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
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

    // Loads all assets in the "Resources/Textures" folder
    private void LoadAllTextures()
    {
        //textures = Resources.LoadAll<Texture2D>("CreationMenus/Textures");

        // look at all the textures that have been saved

        List<Texture2D> texturesList = new List<Texture2D>();
        List<string> texturesPathList = new List<string>();

        // First get stuff from FileSync

        // Then get stuff from our local resoucres

        DirectoryInfo di = new DirectoryInfo(FileDriveManager.Instance.StreamingDrivePath()); //new DirectoryInfo(Application.streamingAssetsPath + "/CreationMenus/Textures/");
        GetTexturesFromDirectory(di, ref texturesList, ref texturesPathList, true);

        textures = texturesList.ToArray();
        //Debug.Log("loaded textures");
        //Debug.Log(textures);
        //Debug.Log("number of textures");
        //Debug.Log(textures.Length);
        totalNumPages = (textures.Length - 1) / 9 + 1;
        texturespaths = texturesPathList.ToArray();
    }

    private void GetTexturesFromDirectory(DirectoryInfo di, ref List<Texture2D>texturesList, ref List<string>texturesPathList, bool recursive)
    {
        //Debug.Log("TM: looking for images in " + di.Name);

        string[] fileEnds = new string[] { "JPG", "PNG", "TIF", "TIFF", "GIF" };

        foreach (FileInfo f in di.GetFiles())
        {
            if (fileEnds.Any(x => f.Extension.ToUpper().EndsWith(x)))
            {
                Texture2D txt = new Texture2D(1, 1);
                txt.LoadImage(System.IO.File.ReadAllBytes(f.FullName));
                texturesList.Add(txt);
                texturesPathList.Add(f.FullName);
            }
        }

        // now recursively get into subfolders
        if (recursive)
            foreach (DirectoryInfo d in di.GetDirectories())
            {
                GetTexturesFromDirectory(d, ref texturesList, ref texturesPathList, true);
            }

    }

    // instantiates all thumbnail button objects and adds them to the thumbnailList then calls RearrangeButtons()
    public void OpenMenu()
    {
        PhotonView.Get(this).RPC("RPC_OpenMenu", RpcTarget.AllViaServer);
        UpdateNavLabel();
    }
    [PunRPC]
    private void RPC_OpenMenu() {

        Refresh();

        thumbnailList = new List<GameObject>();

        buttonPos = locationPlaceholder.transform.position;
        startPos = buttonPos;
        newLinePos = buttonPos;

        GameObject newMenuParent = (GameObject)Instantiate(menuParentPrefab, startPos + Vector3.right * .15f + Vector3.up * .15f, Quaternion.Euler(-90, 0, 0));
        newMenuParent.transform.parent = this.transform;

        menuParent = newMenuParent;

        if (totalNumPages > 1)
        {
            nav.SetActive(true);
        }

        for (int i = 0; i < textures.Length; i++)
        {
            if (i >= imageListStart && i < endpt)
            {
                var t = textures[i];
                var path = texturespaths[i];

                GameObject newThumbnail = (GameObject)Instantiate(thumbnailButton, buttonPos, Quaternion.identity);
                newThumbnail.transform.parent = menuParent.transform;
                newThumbnail.name = newButtonID.ToString();
                newButtonID++;

                if (newThumbnail != null)
                {
                    thumbnailList.Add(newThumbnail);
                }

                var iconAndText = newThumbnail.transform.Find("IconAndText");
                var textMeshP = iconAndText.transform.Find("TextMeshPro");
                textMeshP.gameObject.SetActive(true);

                var tmp = textMeshP.gameObject.GetComponent<TextMeshPro>();

                //Debug.Log("Loading image path: " + path);
                var imageName = path.Substring(path.LastIndexOf("\\")+1);
                //var imageName = path.Substring(path.IndexOf("\\Textures") + "\\Textures\\".Length);
                //var imageName2 = path.Substring()
                if (imageName.Length < 10)
                {
                    tmp.text = imageName;
                }
                else
                {
                    tmp.text = imageName.Substring(0, 9) + "...";
                }
                ThumbnailScript newThumbnailScript = newThumbnail.GetComponentInChildren<ThumbnailScript>();
                newThumbnailScript.ChangeThumbnailMaterial((Texture2D)t);

                newThumbnailScript.thumbnailRelPath = path.Substring(FileDriveManager.Instance.StreamingDrivePath().Length);
                newThumbnailScript.myThumbnailManager = this;
            }
        }
        RearrangeButtons();
        menuParent.transform.localRotation = Quaternion.Euler(-90, 0, 0);

        nav.transform.parent = menuParent.transform;
        nav.transform.localPosition = new Vector3(-3.3f, 0.11f, -96.9f);

        isClosed = false;
    }

    public void CloseMenu() { 
        PhotonView.Get(this).RPC("RPC_CloseMenu", RpcTarget.AllViaServer);
    }
    [PunRPC]
    private void RPC_CloseMenu()
    {
        locationPlaceholder.transform.position = menuParent.transform.position - (Vector3.right * .15f + Vector3.up * .15f);

        nav.transform.parent = transform;
        nav.SetActive(false);
        Destroy(menuParent);     
        isClosed = true;
    }

    public void ThumbnailPressPath(string relativeFilePath)
    {
        PhotonView.Get(this).RPC("RPC_ThumbnailPressPath", RpcTarget.AllViaServer, relativeFilePath);
    }
    [PunRPC]
    public void RPC_ThumbnailPressPath(string relativeFilePath) {

#if UNITY_EDITOR
        SceneStepsManager.Instance.CreateAndConfigureNewObjectForScene_Image(startPos + Vector3.left * 0.3f, Quaternion.identity, Vector3.one, relativeFilePath);
#endif
    }

    /***
    // gets the assigned texture from the thumbnail button and instantiates the new image quad prefab and changes the material texture
    public void ThumbnailPress(Texture texture)
    {
        GameObject newImage = PhotonView.Instantiate(quadImage, startPos + Vector3.left * 0.3f, Quaternion.identity);
        newImage.GetComponent<Renderer>().material.mainTexture = texture;
    }
    ***/


    //rearranges the thumbnail buttons
    public void RearrangeButtons()
    {
        buttonPos = startPos;
        newLinePos = startPos;
        Debug.Assert(thumbnailList.Count > 0, "Thumbnail Manger: Assumes there is at least one image available");
        thumbnailList[0].transform.position = buttonPos;

        if (thumbnailList.Count > 0)
        {
            for (int i = 1; i < thumbnailList.Count; i++)
            {
                if (i % 3 != 0)
                {
                    buttonPos += Vector3.right * .15f;
                    thumbnailList[i].transform.position = buttonPos;
                }

                if (i % 3 == 0)
                {
                    buttonPos = newLinePos + Vector3.down * .15f;
                    newLinePos = buttonPos;
                    thumbnailList[i].transform.position = newLinePos;
                }
            }
        }
    }

    public void PressLeftButton()
    {
        if (currentPageNum > 0)
        {
            CloseMenu();
            currentPageNum -= 1;
            imageListStart -= 9;
            imageListEnd -= 9;
            AdjustListEndpt();           
            OpenMenu();
            if (currentPageNum == 0)
            {
                leftButton.SetActive(false);
            }

            if (currentPageNum < totalNumPages - 1)
            {
                rightButton.SetActive(true);
            }
        }
    }

    public void PressRightButton()
    {
        if (currentPageNum < totalNumPages - 1)
        {
            CloseMenu();
            currentPageNum += 1;
            imageListStart += 9;
            imageListEnd += 9;
            AdjustListEndpt();
            OpenMenu();
            if (currentPageNum >= totalNumPages - 1)
            {
                rightButton.SetActive(false);
            }

            if (currentPageNum > 0)
            {
                leftButton.SetActive(true);
            }
        }
    }

    void UpdateNavLabel()
    {
        TextMeshPro textmesh = nav.transform.Find("IconAndText/TextMeshPro").GetComponent<TextMeshPro>();

        textmesh.SetText("Page " + (currentPageNum+1).ToString() + " of " + totalNumPages.ToString());
    }

    void AdjustListEndpt()
    {
        if (textures.Length < imageListEnd)
        {
            endpt = textures.Length;
        }
        else
        {
            endpt = imageListEnd;
        }
    }
}
