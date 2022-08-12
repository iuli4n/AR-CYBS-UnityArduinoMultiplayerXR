using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

public class ChannelSliderController : MonoBehaviour
{
    PinchSlider slider;

    public AtomicDataModel myModel;

    public TextMeshPro valueLabel;

    bool activated = false;

    // Start is called before the first frame update
    void Awake()
    {
        slider = transform.Find("Slider").GetComponent<PinchSlider>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddModel(AtomicDataModel model)
    {
        myModel = model;
        myModel.OnDataUpdated += OnSliderDataUpdated;
        activated = true;
    }

    void OnSliderDataUpdated(float val)
    {
        float newval = val / 1024;
        if (newval != slider.SliderValue)
        {
            slider.SliderValue = newval;
        }
        valueLabel.SetText(string.Format("{0:0.00}",val));
    }

    public void SendNewSliderValueToDataSwitch()
    {
        if (activated)
        {
            float sliderval = 1024f * slider.SliderValue;
            myModel.Value = sliderval;
        }
          
    }
}
