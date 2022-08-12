using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;


public class TooltipManager : MonoBehaviourPunCallbacks
{
    PhotonView photonView;
    // Start is called before the first frame update
    // format buttons
    [SerializeField]
    private GameObject tooltipBackplate;

    [SerializeField]
    private AtomicDataModelString tooltipValueModel;

    private GameObject labelTMP;
    TextMeshPro tmp;

    public float widthMultiplier;
    public float heightMultiplier;

    float lenTooltipLabel;
    float tooltipWidth;
    string teststring = "poloisrt";
    int numNewlines;
    string[] lines;

    string storedVal;
    void Start()
    {
        tooltipBackplate = gameObject.transform.Find("TooltipChild/Pivot/ContentParent/TipBackground").gameObject;
        tooltipValueModel = gameObject.transform.Find("TooltipMVC/TooltipTextValueModel").gameObject.GetComponent<AtomicDataModelString>();
        labelTMP = gameObject.transform.Find("TooltipChild/Pivot/ContentParent/Label").gameObject;

        photonView = gameObject.GetComponent<PhotonView>();
        tmp = labelTMP.GetComponent<TextMeshPro>();
        tooltipValueModel.Value = "tooltip";
        storedVal = tooltipValueModel.Value;
    }

    // Update is called once per frame
    void Update()
    {
        string tempVal = tooltipValueModel.Value;
        if (tempVal != storedVal)
        {
            storedVal = tempVal;
            Transform parent = tooltipBackplate.transform.parent;
            tooltipBackplate.transform.SetParent(null);
            tooltipBackplate.transform.localScale = new Vector3(widthMultiplier * tmp.renderedWidth, heightMultiplier * tmp.renderedHeight, 1);
            tooltipBackplate.transform.SetParent(parent, true);
        }
    }

    public void AdjustTooltipCanvasLength()
    {
        /*
        lines = tooltipValueModel.Value.Split('\n');
        Debug.Log("LINES:");
        Debug.Log(lines);
        numNewlines = lines.Length;
        tooltipWidth = 0;
        foreach(string l in lines)
        {
            if (l.Length > tooltipWidth)
            {
                tooltipWidth = l.Length;
            }
        }
        lenTooltipLabel = tooltipWidth;// Mathf.Sqrt((float)tooltipWidth);       
        tooltipBackplate.transform.localScale = new Vector3(lenTooltipLabel * .05f, numNewlines*0.027f, 1);
        */
        

    }
    public void RequestOwnershipForThisObject(ManipulationEventData data)
    {
        photonView.RequestOwnership();
    }

}
