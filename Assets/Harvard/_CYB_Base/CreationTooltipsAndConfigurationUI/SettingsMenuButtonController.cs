using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Experimental.UI;

public class SettingsMenuButtonController : MonoBehaviour
{
    public SettingsType settingsType;
    bool pressedToggle;
    Transform targetTransform;
    
    // data channel
    string channelName;

    // rename tooltip
    GameObject tooltipRename;
    GameObject pivot;
    Text renameTextField;
    MRTKUGUIInputField inputField;
    TooltipTextValueController valueController;

    bool renameIsLaunched;

    //move tooltip anchor
    Vector3 savedAnchorPos;
    BoxCollider tooltipBoxCollider;
    GameObject anchor;
    GameObject anchorButtons;
    BoxCollider anchorCollider;
    BoundsControl anchorBoundsControl;
    bool isMovingAnchor;


    MeshRenderer buttonsMeshRenderer;

    public SettingsMenuController settingsMenuController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // "Return" and "Escape" keys used to submit or cancel the tooltip changes
        if (Input.GetKeyDown(KeyCode.Return) && renameIsLaunched)
        {
            SubmitRenameTooltip();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && renameIsLaunched)
        {
            EndRenameTooltip();
        }

        if (Input.GetKeyDown(KeyCode.Return) && isMovingAnchor)
        {
            SubmitMoveAnchor();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isMovingAnchor)
        {
            EndMoveAnchor();
        }
    }

    public void SetButtonType(SettingsMenuButton newButtonObj, Transform target)
    {
        settingsType = newButtonObj.type;
        targetTransform = target;

        if (settingsType == SettingsType.DataChannel)
        {
            channelName = newButtonObj.text;
        }
        else if (settingsType == SettingsType.RenameTooltip)
        {
            tooltipRename = targetTransform.Find("TooltipRename").gameObject;
            renameTextField = targetTransform.Find("TooltipRename/Canvas/MRKeyboardInputField/Text").GetComponent<Text>();
            inputField = targetTransform.Find("TooltipRename/Canvas/MRKeyboardInputField").GetComponent<MRTKUGUIInputField>();
            pivot = targetTransform.Find("TooltipChild/Pivot").gameObject;
            valueController = targetTransform.GetComponentsInChildren<TooltipTextValueController>()[0];
        }
        else if (settingsType == SettingsType.MoveTooltipAnchor)
        {
            tooltipBoxCollider = targetTransform.GetComponent<BoxCollider>();
            anchor = targetTransform.Find("TooltipChild/Anchor").gameObject;
            anchorButtons = anchor.transform.Find("Buttons").gameObject;
            anchorCollider = anchor.GetComponent<BoxCollider>();
            anchorBoundsControl = anchor.GetComponent<BoundsControl>();
        }
    }

    public void DoButtonAction()
    {
        pressedToggle = !pressedToggle;

        if (settingsType == SettingsType.Other)
        {
            DefaultButtonAction();
        }
        else if (settingsType == SettingsType.DataChannel)
        {
            ChangeChannel();
        }
        else if (settingsType == SettingsType.RenameTooltip)
        {
            LaunchRenameTooltip();
        }
        else if (settingsType == SettingsType.MoveTooltipAnchor)
        {
            MoveTooltipAnchor();
        }
    }

    public void DefaultButtonAction()
    {
        Debug.Log("button action");
    }

    public void ChangeChannel()
    {
        AtomicDataSwitch targetAtomicDataSwitch = targetTransform.GetComponentsInChildren<AtomicDataSwitch>()[0];
        targetAtomicDataSwitch.SetCurrentChannel(channelName);
    }

    public void LaunchRenameTooltip()
    {
        //disable the existing tooltip display
        pivot.SetActive(false);

        settingsMenuController.backplate.SetActive(false);

        //tooltipRename.active = true;
        tooltipRename.SetActive(true);
        renameTextField.text = valueController.myModel.Value.ToString();

        inputField.ActivateInputField();

        // place the tooltip rename field over the existing text display on the tooltip
        GameObject contentParent = targetTransform.Find("TooltipChild/Pivot/ContentParent").gameObject;
        tooltipRename.transform.position = new Vector3(contentParent.transform.position.x, contentParent.transform.position.y, contentParent.transform.position.z - .015f);
        tooltipRename.transform.rotation = contentParent.transform.rotation;
        tooltipRename.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        // add listeners to rename submit and cancel buttons
        Interactable submitRenameButtonInteractable = tooltipRename.transform.Find("Buttons").Find("submit1").GetComponent<Interactable>();
        submitRenameButtonInteractable.OnClick.AddListener(SubmitRenameTooltip);
        Interactable cancelRenameButtonInteractable = tooltipRename.transform.Find("Buttons").Find("cancel1").GetComponent<Interactable>();
        cancelRenameButtonInteractable.OnClick.AddListener(EndRenameTooltip);

        renameIsLaunched = true;
        settingsMenuController.buttonCollection.transform.localScale = Vector3.zero;
        settingsMenuController.menuLaunchButton.SetActive(false);
    }

    void SubmitRenameTooltip()
    {
        string text = renameTextField.text;
        Debug.Log("grabbing typed text " + text);

        // send new value to MVC
        valueController.SetTooltipValue((text));

        TooltipManager tooltipManager = targetTransform.GetComponent<TooltipManager>();
        tooltipManager.AdjustTooltipCanvasLength();

        EndRenameTooltip();
    }

    void EndRenameTooltip()
    {
        settingsMenuController.buttonCollection.transform.localScale = Vector3.one;
        settingsMenuController.menuLaunchButton.SetActive(true);
        settingsMenuController.backplate.SetActive(true);
        tooltipRename.SetActive(false);
        pivot.SetActive(true);
        renameIsLaunched = false;
    }

    public void MoveTooltipAnchor()
    {
        EnableMoveTooltip();
    }

    void EnableMoveTooltip()
    {
        settingsMenuController.backplate.SetActive(false);
        // add listeners to rename submit and cancel buttons
        anchorButtons.SetActive(true);
        Interactable submitAnchorButtonInteractable = anchor.transform.Find("Buttons").Find("submit1").GetComponent<Interactable>();
        submitAnchorButtonInteractable.OnClick.AddListener(SubmitMoveAnchor);
        Interactable cancelAnchorButtonInteractable = anchor.transform.Find("Buttons").Find("cancel1").GetComponent<Interactable>();
        cancelAnchorButtonInteractable.OnClick.AddListener(EndMoveAnchor);

        settingsMenuController.buttonCollection.transform.localScale = Vector3.zero;
        settingsMenuController.menuLaunchButton.SetActive(false);

        isMovingAnchor = true;

        savedAnchorPos = anchor.transform.position;

        tooltipBoxCollider.enabled = false;
        anchorCollider.enabled = true;
        anchorBoundsControl.enabled = true;
    }

    void EndMoveTooltip()
    {
        settingsMenuController.backplate.SetActive(true);
        anchorButtons.SetActive(false);
        settingsMenuController.buttonCollection.transform.localScale = Vector3.one;
        settingsMenuController.menuLaunchButton.SetActive(true);
        isMovingAnchor = false;

        tooltipBoxCollider.enabled = true;
        anchorCollider.enabled = false;
        anchorBoundsControl.enabled = false;
    }

    void CancelMoveTooltip()
    {
        anchor.transform.position = savedAnchorPos;
    }

    void SubmitMoveAnchor()
    {
        EndMoveTooltip();
    }

    void EndMoveAnchor()
    {
        CancelMoveTooltip();
        EndMoveTooltip();
    }
}
