
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipView : MonoBehaviour
{
    public AtomicDataModelString textValModel;
    public TextMeshPro tooltipText;
    public TooltipManager tooltipManager;

    // Start is called before the first frame update
    void Start()
    {
        tooltipManager = this.GetComponent<TooltipManager>(); 
       textValModel.OnDataUpdated += UpdateTooltipText;
    }
   
    public void UpdateTooltipText(string val)
    {
        Debug.Log("val" + val.ToString());
        tooltipText.text = val.ToString();

        tooltipManager.AdjustTooltipCanvasLength();

    }

    private void OnDestroy()
    {
        textValModel.OnDataUpdated -= UpdateTooltipText;
    }
}



