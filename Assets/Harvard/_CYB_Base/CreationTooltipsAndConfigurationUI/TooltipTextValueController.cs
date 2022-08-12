using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Experimental.UI;

public class TooltipTextValueController : MonoBehaviour
{
    public AtomicDataModelString myModel;
    public PhotonView pv;
    public GameObject tooltip;

    //public string initialText;
    public string initialValue;

    private void Start()
    {
        StartCoroutine(DelayedInitialize());
        
    }
    IEnumerator DelayedInitialize()
    {
        yield return new WaitForSeconds(1);

        SetTooltipValue(initialValue);
    }

    void Update()
    {
        
    }
    public void SetTooltipValue(string newFloat)
    {
        myModel.Value = newFloat;
    }

}
