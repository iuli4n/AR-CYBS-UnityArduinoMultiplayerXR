using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMenuFinder : MonoBehaviour
{
    private CreationObjectManager objectsMenu;
    // Start is called before the first frame update
    void Start()
    {
        GameObject creationMenu = GameObject.Find("AREA_CRAFTING/CreationMenus/ObjectMenu/3DObjectManager");
        if (creationMenu)
        {
            objectsMenu = creationMenu.GetComponent<CreationObjectManager>();
        } else
        {
            Debug.LogWarning("ObjectMenuFinder: 3D object creation menu not found in scene. Is this intentional?");
        }
    }

    public bool IsOpened()
    {
        return !objectsMenu.isClosed;
    }

    public void ObjectMenuToggle()
    {
        if (objectsMenu.neverOpened)
            objectsMenu.OpenMenu();
        else if (objectsMenu.menuParent.activeSelf)
            objectsMenu.CloseMenu();
        else
            objectsMenu.OpenMenu();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
