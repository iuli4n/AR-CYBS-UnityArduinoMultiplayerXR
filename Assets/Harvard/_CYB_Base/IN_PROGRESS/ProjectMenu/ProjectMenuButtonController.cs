using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ProjectMenuActionType { LOAD, CREATE, DELETE, DUPLICATE }

public class ProjectMenuButtonController : MonoBehaviour
{

    //public GameObject projectMenu;
    public ProjectMenuController projectMenuController;
    public string buttonName;
    //public ProjectMenuActionType actionType;

    bool pressedToggle;
    GameObject backplateNormal;
    GameObject backplateToggle;

    
    // Start is called before the first frame update
    void Start()
    {
        backplateNormal = transform.Find("Backplate Plus Button/BackplateNormal").gameObject;
        backplateToggle = transform.Find("Backplate Plus Button/BackplateToggle").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DoButtonAction()
    {      
        if (!pressedToggle)
        {          
            ToggleOn();
        }
        else
        {    
            ToggleOff();
        }
    }

    public void ToggleOn()
    {
        if (projectMenuController.selectedProjectName != "")
        {
            projectMenuController.buttonLookup[projectMenuController.selectedProjectName].ToggleOff();
        }

        Debug.Log("currently selected project is" + buttonName);
        projectMenuController.selectedProjectName = buttonName;

        pressedToggle = true;
        backplateNormal.SetActive(false);
        backplateToggle.SetActive(true);
    }

    public void ToggleOff()
    {
        pressedToggle = !pressedToggle;
        backplateNormal.SetActive(true);
        backplateToggle.SetActive(false);
        projectMenuController.selectedProjectName = "";
    }
}
