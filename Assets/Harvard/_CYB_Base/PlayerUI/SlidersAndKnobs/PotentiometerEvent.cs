using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PotentiometerEvent : MonoBehaviour
{
    public TMP_Text PotentiometerText;
    public Image progress;
    public AtomicDataModel PotentiometerData;

    private float dialFillAmount = 200f; // Currently hard coded to the value of Kyle's step motor

    // Start is called before the first frame update
    void Start()
    {
        PotentiometerData.OnDataUpdated += UpdateDial;
    }

    void UpdateDial(float val)
    {
        PotentiometerText.text = val.ToString();
        progress.fillAmount = (((val * 100f) / dialFillAmount)) / 100f; // Formula for adjusting the dial fill
    }

}
