using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseImageMenu : MonoBehaviour
{
    // TODO: This is only used by the UI to trigger the CloseMenu function. Should get rid of this and just wire it directly.

    ThumbnailManager imageMenu;
    void Start()
    {
        imageMenu = GameObject.Find("ImageThumbnailManager").GetComponent<ThumbnailManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseMenu()
    {
        imageMenu.CloseMenu();
    }
}
