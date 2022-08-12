using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using TMPro;

public class SliderDataManager : MonoBehaviourPun
{

    public PinchSlider slider;
    public AtomicDataModel dataModel;
    public PhotonView pv;
    
    public float maxValue;
    public TextMeshPro sliderValueText;

    // Start is called before the first frame update
    void Start()
    {
        dataModel.OnDataUpdated += OnDataModelUpdate;
    }

    public void OnSliderUpdate()
    {
        float newVal = Mathf.Round(slider.SliderValue * maxValue);        
        if (newVal == dataModel.Value) return;
        
        if (!pv.AmOwner)
            pv.RequestOwnership();

        dataModel.Value = newVal;
        sliderValueText.text = "" + slider.SliderValue * maxValue;
        //Debug.Log("slider model value changed "+newVal);
    }
    public void OnDataModelUpdate(float newval)
    {
        if (slider.SliderValue == newval / maxValue) return;

        slider.SliderValue = newval / maxValue;
        sliderValueText.text = ""+slider.SliderValue * maxValue;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnGUI()
    {
        /**
        GUILayout.BeginVertical();
        if (GUILayout.Button("SET"))
        {
            dataModel.Value = 0.5f;
        }
        GUILayout.EndVertical();
        **/
    }
}
