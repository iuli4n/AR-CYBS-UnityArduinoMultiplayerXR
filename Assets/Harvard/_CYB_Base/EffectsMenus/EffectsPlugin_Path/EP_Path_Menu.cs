using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EP_Path_Menu : AEffect4PluginMenu
{
    public GameObject myMenu;

    // toggle on/off
    public GameObject button;
    GameObject buttonToggleOff, buttonToggleOn;
    bool isOn;

    // positions
    public GameObject posCanvas;
    public GameObject posListButton;
    GameObject posListButtonOff, posListButtonOn;

    public GameObject posDrawingButton;
    GameObject posDrawingButtonOff, posDrawingButtonOn;


    // rotations
    public GameObject rotCanvas;
    public GameObject rotNoneButton;
    GameObject rotNoneButtonOff, rotNoneButtonOn;

    public GameObject rotForwardButton;
    GameObject rotForwardButtonOff, rotForwardButtonOn;

    public GameObject rotListButton;
    GameObject rotListButtonOff, rotListButtonOn;

    // slider value
    public GameObject sliderCanvas;
    public TextMeshPro sliderValLabel;
    //public GameObject sliderValLabelObject;
    public PinchSlider slider;
    public GameObject sliderObject;


    // menu size & position
    public float menuScale;
    public float menuShift;

    bool activated = false;

    string positionMode = "points";

    public void Activate()
    {
        activated = true;
    }

    override public void OpenMenu(Transform parentLocation)
    {
        if (activated)
        {
            myMenu.transform.parent = parentLocation;
            myMenu.transform.localPosition = new Vector3(menuShift, 0, 0);
            myMenu.transform.localScale = menuScale * Vector3.one;
            myMenu.SetActive(true);
        }

    }
    override public void HideMenu()
    {
        myMenu.SetActive(false);
    }
    override public void Refresh(float currentSensorValue)
    {
        // should update the UI to match the current model but we have nothing to do
    }

    // Start is called before the first frame update
    void Start()
    {
        isOn = false;
        buttonToggleOff = transform.Find("MenuGUI/Toggle/Backplate Plus Button/BackplateNormal").gameObject;
        buttonToggleOn = transform.Find("MenuGUI/Toggle/Backplate Plus Button/BackplateToggle").gameObject;

        posListButtonOff = transform.Find("MenuGUI/PositionList/Backplate Plus Button/BackplateNormal").gameObject;
        posListButtonOn = transform.Find("MenuGUI/PositionList/Backplate Plus Button/BackplateToggle").gameObject;

        posDrawingButtonOff = transform.Find("MenuGUI/PositionDrawing/Backplate Plus Button/BackplateNormal").gameObject;
        posDrawingButtonOn = transform.Find("MenuGUI/PositionDrawing/Backplate Plus Button/BackplateToggle").gameObject;

        rotNoneButtonOff = transform.Find("MenuGUI/NoRotation/Backplate Plus Button/BackplateNormal").gameObject;
        rotNoneButtonOn = transform.Find("MenuGUI/NoRotation/Backplate Plus Button/BackplateToggle").gameObject;

        rotForwardButtonOff = transform.Find("MenuGUI/Forward/Backplate Plus Button/BackplateNormal").gameObject;
        rotForwardButtonOn = transform.Find("MenuGUI/Forward/Backplate Plus Button/BackplateToggle").gameObject;

        rotListButtonOff = transform.Find("MenuGUI/RotationList/Backplate Plus Button/BackplateNormal").gameObject;
        rotListButtonOn = transform.Find("MenuGUI/RotationList/Backplate Plus Button/BackplateToggle").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Toggle()
    {
        isOn = !isOn;
        buttonToggleOn.SetActive(isOn);
        buttonToggleOff.SetActive(!isOn);

        posCanvas.SetActive(isOn);
        posListButton.SetActive(isOn);
        posDrawingButton.SetActive(isOn);

        rotCanvas.SetActive(isOn);
        rotNoneButton.SetActive(isOn);
        rotForwardButton.SetActive(isOn);
        rotListButton.SetActive(isOn && (positionMode == "points"));
        rotCanvas.transform.Find("RotLabelList").gameObject.SetActive(positionMode == "points");
        sliderObject.SetActive(isOn && (positionMode == "points"));
        sliderCanvas.SetActive(isOn && (positionMode == "points"));
    }

    public void SetPosModeList()
    {
        posListButtonOff.SetActive(false);
        posListButtonOn.SetActive(true);
        posDrawingButtonOff.SetActive(true);
        posDrawingButtonOn.SetActive(false);

        rotListButton.SetActive(true);
        rotCanvas.transform.Find("RotLabelList").gameObject.SetActive(true);
        sliderObject.SetActive(true);
        sliderCanvas.SetActive(true);

        positionMode = "points";
    }

    public void SetPosModeDrawing()
    {
        posListButtonOff.SetActive(true);
        posListButtonOn.SetActive(false);
        posDrawingButtonOff.SetActive(false);
        posDrawingButtonOn.SetActive(true);

        rotListButton.SetActive(false);
        rotCanvas.transform.Find("RotLabelList").gameObject.SetActive(false);
        sliderObject.SetActive(false);
        sliderCanvas.SetActive(false);

        positionMode = "drawing";

        //if (rotListButtonOn.activeSelf) { SetRotModeNone(); }
    }

    public void SetRotModeNone()
    {
        rotNoneButtonOff.SetActive(false);
        rotNoneButtonOn.SetActive(true);
        rotForwardButtonOff.SetActive(true);
        rotForwardButtonOn.SetActive(false);
        rotListButtonOff.SetActive(true);
        rotListButtonOn.SetActive(false);
    }

    public void SetRotModeForward()
    {
        rotNoneButtonOff.SetActive(true);
        rotNoneButtonOn.SetActive(false);
        rotForwardButtonOff.SetActive(false);
        rotForwardButtonOn.SetActive(true);
        rotListButtonOff.SetActive(true);
        rotListButtonOn.SetActive(false);
    }

    public void SetRotModeList()
    {
        rotNoneButtonOff.SetActive(true);
        rotNoneButtonOn.SetActive(false);
        rotForwardButtonOff.SetActive(true);
        rotForwardButtonOn.SetActive(false);
        rotListButtonOff.SetActive(false);
        rotListButtonOn.SetActive(true);
    }

    public void SetValText(float v)
    // always be updated when the model changes
    {
        sliderValLabel.SetText("Smoothness: " + Mathf.Round(v*100f).ToString() + "%");
    }
}
