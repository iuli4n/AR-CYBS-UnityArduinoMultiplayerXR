using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LEDSliderEvent : MonoBehaviourPun
{
    //public TMP_Text SliderText;
    //private Slider LEDSlider;

    public AtomicDataModel dataModel;
    public Slider slider;
    public PhotonView pv;


    // Start is called before the first frame update
    void Start()
    {
        dataModel.OnDataUpdated += OnDataModelUpdate;
    }

    public void OnSliderUpdate()
    {
        // Check if YOU are the current owner of the slider. If not, request it.
        // If Takeover is assigned in the Photon View, you get automatically assigned.
        if (!pv.AmOwner)
            pv.RequestOwnership();

        dataModel.Value = slider.value;
    }
    public void OnDataModelUpdate(float newval)
    {
        slider.value = newval;
    }
}
