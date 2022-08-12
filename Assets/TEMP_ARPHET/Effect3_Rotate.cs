using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect3_Rotate : MonoBehaviour
{
    public AtomicDataSwitch model;

    public bool hack_nomovement = false;
    public bool flippedRotation;

    private float currentAngle;

    private Vector3 originalScale;

    public bool hideWhenNotRotating = true;

    public float scale = 2f;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = this.transform.localScale;
        currentAngle = this.transform.localEulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        /**
        if (hideWhenNotRotating)
        {
            if (Mathf.Abs(usm.fluxSimpleSmooth) < 0.01f)
            {
                this.transform.localScale = new Vector3(.0001f, .0001f, .0001f);
                return;
            }
            else
            {
                this.transform.localScale = originalScale;
            }
        }
        ***/

        // HACK THRESHOLD
        if (model.Value < 30)
        {
            currentAngle += (flippedRotation ? -1 : 1) * scale * model.Value / 100f * Time.deltaTime * 50f;
        }

        transform.localRotation = Quaternion.Euler(0, currentAngle, model.Value < 0 ? 180f : 0f);
        //transform.Rotate(0, usm.fluxSimple * 1f, 0, Space.Self);
    }
}
