using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCreatedImage : MonoBehaviour
{
    [SerializeField]
    AtomicDataModelString myPathModel;
    public GameObject currentInstantiatedTexture;

    // this object needs to be initialized every time because textures arent serialized. so just imagine it's not initialized
    bool initialized = false;


    private void Start()
    {
        if (!initialized)
            Initialize();

        if (myPathModel.Value != null && myPathModel.Value.Length > 0)
        {
            SetAndRefresh(myPathModel.Value);
        }
    }

    private void Initialize()
    {
        currentInstantiatedTexture = this.gameObject.transform.Find("TextureQuad").gameObject;
        myPathModel = this.gameObject.transform.Find("ImageInfo").GetComponent<AtomicDataModelString>();
        initialized = true;
    }

    public string GetImagePath()
    {
        return myPathModel.Value;
    }
    public void SetAndRefresh(string imageFilename)
    {
        if (!initialized) 
            Initialize(); 

        myPathModel.Value = imageFilename;
        Refresh();
    }
    public void Refresh()
    {
        if (!myPathModel.Value.StartsWith("/") && !myPathModel.Value.StartsWith("\\"))
        {
            myPathModel.Value = "/" + myPathModel.Value;
        }
        //Debug.Log("PCImage loading path " + myPathModel.Value);

        string filepath = FileDriveManager.Instance.StreamingDrivePath() + myPathModel.Value;
        Texture2D txt = new Texture2D(1, 1);
        txt.LoadImage(System.IO.File.ReadAllBytes(filepath));
        currentInstantiatedTexture.GetComponent<Renderer>().material.mainTexture = txt;
    }
}
