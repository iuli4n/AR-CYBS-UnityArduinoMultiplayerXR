using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class KnobView : MonoBehaviour
{
    // this sciprt updates the knob model rotation when the model is updated
    public AtomicDataModel myRotValModel;
    private TextMeshProUGUI rotValText;

    // Start is called before the first frame update
    void Start()
    {
        rotValText = gameObject.transform.parent.Find("Canvas/knobText").gameObject.GetComponent<TextMeshProUGUI>();

        myRotValModel.OnDataUpdated += UpdateKnobRotation;
        myRotValModel.OnDataUpdated += UpdateKnobText;

        //RotationValueModel.onRotValUpdate += UpdateKnobRotation;
        //RotationValueModel.onRotValUpdate += UpdateKnobText;
    }

    // Update is called once per frame
    public void UpdateKnobRotation(float val)
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, val);
    }

    public void UpdateKnobText(float val)
    {
        rotValText.text = ((int)val).ToString();
    }

    private void OnDestroy()
    {
        myRotValModel.OnDataUpdated -= UpdateKnobRotation;
        myRotValModel.OnDataUpdated -= UpdateKnobText;
    }
}
