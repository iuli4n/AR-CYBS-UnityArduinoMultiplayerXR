using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseObjectMenu : MonoBehaviour
{
    //private GameObject buttonParent;

    CreationObjectManager objectMenu;

    private void Start()
    {
        //buttonParent = transform.parent.gameObject;
        objectMenu = GameObject.Find("3DObjectManager").GetComponent<CreationObjectManager>();
    }

    public void CloseMenu()
    {
        //buttonParent.SetActive(false);
        objectMenu.CloseMenu();
    }
}
