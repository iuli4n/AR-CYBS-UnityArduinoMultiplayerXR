using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EP_RandomScale_Menu : AEffect4PluginMenu
{
    public GameObject myMenu;

    // toggle on/off
    public GameObject button;
    GameObject buttonToggleOff, buttonToggleOn;
    bool isOn;

    // slider value
    public TextMeshPro valLabel;
    public GameObject valLabelObject;
    public PinchSlider slider;
    public GameObject sliderObject;
    
    // menu size & position
    public float menuScale;
    public float menuShift;

    bool activated = false;

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
        sliderObject.SetActive(isOn);
        valLabelObject.SetActive(isOn);
    }

    public void SetValText(float v)
        // always be updated when the model changes
    {
        valLabel.SetText("Value: " + Math.Round(v, 2).ToString());
    }
}
