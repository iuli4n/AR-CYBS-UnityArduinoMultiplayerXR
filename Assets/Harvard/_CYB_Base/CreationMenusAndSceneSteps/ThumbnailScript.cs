using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumbnailScript : MonoBehaviour
{
    public string thumbnailRelPath;
    public ThumbnailManager myThumbnailManager;
    
    
    // TODO: Remove this might not be used anymore because using thumbnailRelPath now
    //private Texture assignedTexture;


    public void ChangeThumbnailMaterial(Texture texture)
    {
        GetComponent<Renderer>().material.mainTexture = texture;

        //assignedTexture = texture;
    }

    public void ButtonPress()
    {
        myThumbnailManager.ThumbnailPressPath(thumbnailRelPath);
    }

}
