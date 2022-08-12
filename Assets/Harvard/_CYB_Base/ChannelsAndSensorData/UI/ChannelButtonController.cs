using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
public class ChannelButtonController : MonoBehaviour
{
    public AtomicDataModel myModel;

    public TextMeshPro valueLabel;

    //public GameObject buttonPlates;
    GameObject backplateNormalOn;
    GameObject backplateToggleOn;
    GameObject backplateNormalOff;
    GameObject backplateToggleOff;

    bool activated= false;
    // Start is called before the first frame update
    void Start()
    {
        //label.SetText(myModel.CurrentChannel);
        backplateNormalOn = transform.Find("On/Backplate Plus Button/BackplateNormal").gameObject;
        backplateToggleOn = transform.Find("On/Backplate Plus Button/BackplateToggle").gameObject;
        backplateNormalOff = transform.Find("Off/Backplate Plus Button/BackplateNormal").gameObject;
        backplateToggleOff = transform.Find("Off/Backplate Plus Button/BackplateToggle").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            if (myModel.Value < 1f)
            {
                backplateNormalOn.SetActive(true);
                backplateToggleOn.SetActive(false);
                backplateNormalOff.SetActive(false);
                backplateToggleOff.SetActive(true);
            }
            else if (myModel.Value > 1023f)
            {
                backplateNormalOn.SetActive(false);
                backplateToggleOn.SetActive(true);
                backplateNormalOff.SetActive(true);
                backplateToggleOff.SetActive(false);
            }
            else
            {
                backplateNormalOn.SetActive(true);
                backplateToggleOn.SetActive(false);
                backplateNormalOff.SetActive(true);
                backplateToggleOff.SetActive(false);
            }
        }
    }

    public void AddModel(AtomicDataModel model)
    {
        myModel = model;
        myModel.OnDataUpdated += OnButtonDataUpdated;
        activated = true;
    }

    void OnButtonDataUpdated(float val)
    {
        valueLabel.SetText(string.Format("{0:0.00}", val));
    }

    public void SendNewButtonValueToDataSwitch(float settingVal)
    {
        if (activated) { myModel.Value = settingVal; }
    }
}
