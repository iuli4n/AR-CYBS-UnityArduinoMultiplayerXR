using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Experimental.UI;

public class ProjectMenuController : MonoBehaviour
{
    //public GameObject projectMenuprefab;
    public GameObject toggleButtonprefab;
    GameObject menu;
    string[] projectNames;
    int numButtonRows;

    [SerializeField]
    GameObject nav;
    GameObject leftButton;
    GameObject rightButton;

    [SerializeField]
    GameObject actions;

    int projectListStart;
    int projectListEnd;
    int endpt;

    public string selectedProjectName;

    public GameObject DialogPrefab;


    private Object[] objects;
    public GameObject menuParentPrefab;

    // TODO: Make this private and fix the hand menu so it doesn't need it
    GameObject _menuParent;
    // TODO: Make this private and fix the hand menu so it doesn't need it
    public bool neverOpened;
    public GameObject referenceSize;

    //public GameObject addButton;
    public GameObject locationPlaceholder; // an empty object for determining the next shortcut button location

    private Vector3 newLinePos;
    private Vector3 buttonPos;
    private Vector3 startPos;
    private int newButtonID;
    List<GameObject> thumbnailList;
    List<GameObject> objectList;
    bool isClosed = true;
    int totalNumPages;

    GameObject projectRename;
    Text renameTextField;
    MRTKUGUIInputField inputField;

    public Dictionary<string, ProjectMenuButtonController> buttonLookup;

    // Start is called before the first frame update
    void Start()
    {
        //projectNames = new string[] { "ProjectA", "ProjectB", "ProjectC", "ProjectD", "ProjectE", "ProjectF", "ProjectG", "ProjectH", "ProjectI", "ProjectJ", "ProjectK", "ProjectL", "Project M" };
        projectNames = SceneStepsManager.Instance.GetProjectNames();
        totalNumPages = (projectNames.Length - 1) / 9 + 1;
        //Debug.Log("Project names:");
        //foreach(string name in projectNames)
        //{
        //    Debug.Log("PROJECT: " + name);
        //}
        //Debug.Log(projectNames.ToString());

        LoadAllObjects();
        projectListStart = 0;
        projectListEnd = 9;
        AdjustListEndpt();

        //  menuParent = new GameObject("MENU");
        //neverOpened = true;
        //thumbnailList = new List<GameObject>();
        //objectList = new List<GameObject>();
        newButtonID = 0;

        buttonPos = locationPlaceholder.transform.position;
        startPos = buttonPos;
        newLinePos = buttonPos;

        leftButton = nav.transform.Find("ProjectMenuLeftButton").gameObject;
        rightButton = nav.transform.Find("ProjectMenuRightButton").gameObject;

        projectRename = transform.Find("ProjectMenuActions/ProjectRename").gameObject;
        renameTextField = transform.Find("ProjectMenuActions/ProjectRename/Canvas/MRKeyboardInputField/Text").GetComponent<Text>();
        inputField = transform.Find("ProjectMenuActions/ProjectRename/Canvas/MRKeyboardInputField").GetComponent<MRTKUGUIInputField>();

        
    }

    private void Update()
    {
        // "P" keypress opens the PROJECT MENU
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isClosed)
            {
                OpenMenu();
            }
            else
            {
                CloseMenu();
            }
        }
    }


    // Loads all objects in the "Resources/Objects" folder
    public void LoadAllObjects()
    {
        objects = Resources.LoadAll("Objects", typeof(GameObject));
        //Debug.Log("TOTAL NUM OBJECTS" + objects.Length.ToString());
    }

    // Instantiates all menu buttons and associated 3D objects then calls RearrangeButtons()
    public void OpenMenu()
    {
        buttonLookup = new Dictionary<string, ProjectMenuButtonController>();

        thumbnailList = new List<GameObject>();
        //objectList = new List<GameObject>();
        if (totalNumPages > 1)
        {
            nav.SetActive(true);
        }
        
        actions.SetActive(true);

        // TODOIRNOW: make this networked
        // TODOIRNOW: maybe this isn't needed ?
        GameObject newMenuParent = (GameObject)Instantiate(menuParentPrefab, startPos + Vector3.right * .15f + Vector3.up * .15f, Quaternion.Euler(-90, 0, 0));
        _menuParent = newMenuParent;

        for (int i = 0; i < objects.Length; i++)
        {
            if (i >= projectListStart && i < endpt)
            {
                //Debug.Log("About to load project num: " + i.ToString());
                string tempName = projectNames[i];//"project " + i.ToString();

                Object o = objects[i];
                GameObject newButton = (GameObject)Instantiate(toggleButtonprefab, Vector3.zero, Quaternion.identity);
                ProjectMenuButtonController buttonController = newButton.transform.GetComponent<ProjectMenuButtonController>();
                newButton.transform.parent = _menuParent.transform;
                buttonController.projectMenuController = this;
                buttonController.buttonName = tempName;
                buttonLookup.Add(tempName, buttonController);
                
                newButton.name = tempName;

                if (newButton != null)
                {
                    thumbnailList.Add(newButton);
                }

                //GameObject newObject = (GameObject)Instantiate(o, buttonPos + Vector3.forward * .05f, Quaternion.identity);
                //newObject.transform.parent = newButton.transform;
                //ResizeObject(newObject, 0.05f);
                //objectList.Add(newObject);

                var textMeshP = newButton.transform.Find("IconAndText/TextMeshPro");
                textMeshP.gameObject.SetActive(true);

                var tmp = textMeshP.gameObject.GetComponent<TextMeshPro>();

                if (tempName.Length < 10)
                {
                    tmp.text = tempName;
                }
                else
                {
                    tmp.text = tempName.Substring(0, 9) + "...";
                }

            }
        }
        RearrangeButtons();
        UpdatePageNumberLabel();

        isClosed = false;
    }

    public void CloseMenu()
    {
        nav.SetActive(false); 
        actions.SetActive(false);
        Destroy(_menuParent);
        isClosed = true;
    }

    void RefreshMenu()
    {
        CloseMenu();
        OpenMenu();
    }

    public void RearrangeButtons()
    {
        buttonPos = startPos;
        newLinePos = startPos;
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
    // Resizes the created object using the referenceSize object
    public void ResizeObject(GameObject obj, float scaleFactor)
    {
        Vector3 sizeCalculated = referenceSize.GetComponent<Renderer>().bounds.size;
        obj.transform.localScale = sizeCalculated * scaleFactor;
    }

    public void PressLeftButton()
    {
        if (projectListStart > 0)
        {
            CloseMenu();
            projectListStart -= 9;
            projectListEnd -= 9;
            AdjustListEndpt();

            OpenMenu();

            if (projectListStart == 0)
            {
                leftButton.SetActive(false);
            }

            if (projectListEnd < projectNames.Length)
            {
                rightButton.SetActive(true);
            }
        }

        UpdatePageNumberLabel();
    }
    
    public void PressRightButton()
    {
        if (projectListEnd < projectNames.Length)
        {
            CloseMenu();
            projectListStart += 9;
            projectListEnd += 9;
            AdjustListEndpt();

            OpenMenu();

            if (projectListEnd >= projectNames.Length)
            {
                rightButton.SetActive(false);
            }

            if (projectListStart > 0)
            {
                leftButton.SetActive(true);
            }
        }

        UpdatePageNumberLabel();
    }

    void AdjustListEndpt()
    {
        if (projectNames.Length < projectListEnd)
        {
            endpt = projectNames.Length;
        }
        else
        {
            endpt = projectListEnd;
        }
    }

    void UpdatePageNumberLabel()
    {
        TextMeshPro textmesh = nav.transform.Find("IconAndText/TextMeshPro").GetComponent<TextMeshPro>();
        textmesh.SetText("Page " + (projectListStart / 9 + 1).ToString() + " of " + (totalNumPages).ToString());
    }
    
    public void CreateNewProject()
    {
        Debug.Log("pressed CREATE new project button");
    }

    public void LoadProject()
    {
        if (selectedProjectName != "")
        {
            Debug.Log("pressed LOAD project button for selected project: " + selectedProjectName);
            SceneStepsManager.Instance.OpenProject(selectedProjectName);
        } 
    }


    public void DeleteProject()
    {
        if (selectedProjectName != "")
        {
            Debug.Log("pressed DELETE project button for selected project: " + selectedProjectName);
           
            string title = "Delete Project?";
            string message = "Are you sure you want to delete the project " + selectedProjectName + "?";
            
            Dialog d = Dialog.Open(DialogPrefab, DialogButtonType.Yes | DialogButtonType.No, title, message, true);
            if (d != null)
            {
                d.OnClosed += OnDeleteDialog;
            }

            //SceneStepsManager.Instance.
        }
    }

    private void OnDeleteDialog(DialogResult obj)
    {
        if (obj.Result == DialogButtonType.Yes)
        {
            Debug.Log("OK LETS DELETE FOR REAL");
        }
        else if (obj.Result == DialogButtonType.No)
        {
            Debug.Log("NOT DELETING");
        }
    }

    public void RenameProject()
    {
        if (selectedProjectName != "")
        {
            Debug.Log("pressed RENAME project button for selected project: " + selectedProjectName);

            string title = "Rename Project";
            string message = "Enter the new name for " + selectedProjectName + ":";

            Dialog d = Dialog.Open(DialogPrefab, DialogButtonType.OK | DialogButtonType.Cancel, title, message, true);
            if (d != null)
            {
                d.OnClosed += OnRenameDialog;
            }

            ActivateProjectRename(d);
        }
    }

    private void OnRenameDialog(DialogResult obj)
    {
        
        string text = renameTextField.text;
        Debug.Log("grabbing typed text " + text);
        DeactivateProjectRename();
        if (obj.Result == DialogButtonType.OK)
        {
            Debug.Log("OK LETS RENAME FOR REAL");
        }
        else if (obj.Result == DialogButtonType.Cancel)
        {
            Debug.Log("NOT RENAMING");           
        }
    }

    public void DuplicateProject()
    {
        if (selectedProjectName != "")
        {
            Debug.Log("pressed DUPLICATE project button for selected project: " + selectedProjectName);

            string title = "Duplicate Project";
            string message = "Enter the new name for the copy of " + selectedProjectName + ":";

            Dialog d = Dialog.Open(DialogPrefab, DialogButtonType.OK | DialogButtonType.Cancel, title, message, true);
            if (d != null)
            {
                d.OnClosed += OnDuplicateDialog;
            }

            ActivateProjectRename(d);
        }
    }

    private void OnDuplicateDialog(DialogResult obj)
    {

        string text = renameTextField.text;
        Debug.Log("grabbing typed text " + text);
        DeactivateProjectRename();

        if (obj.Result == DialogButtonType.OK)
        {
            Debug.Log("OK LETS DUPLICATE FOR REAL");
        }
        else if (obj.Result == DialogButtonType.Cancel)
        {
            Debug.Log("NOT DUPLICATING");
        }
    }

    void ActivateProjectRename(Dialog d)
    {
        projectRename.SetActive(true);

        projectRename.transform.parent = d.transform;
        projectRename.transform.localPosition = new Vector3(0f, 0f, - 0.01f); //Vector3.zero;
        projectRename.transform.localScale = 0.1f * Vector3.one;

        renameTextField.text = selectedProjectName;

        inputField.ActivateInputField();
    }

    void DeactivateProjectRename()
    {
        projectRename.SetActive(false);
        projectRename.transform.parent = transform.Find("ProjectMenuActions");
    }

}

