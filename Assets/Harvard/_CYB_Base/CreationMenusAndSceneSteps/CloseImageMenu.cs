using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseImageMenu : MonoBehaviour
{
    // Start is called before the first frame update

    ThumbnailManager imageMenu;
    void Start()
    {
        imageMenu = GameObject.Find("ThumbnailManager").GetComponent<ThumbnailManager>();
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
