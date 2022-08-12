using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect3_Scale : MonoBehaviour
{
    public AtomicDataSwitch model;

    public bool inversed = false;

    public float scale = 2f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(1, 1, (inversed ? -1 : 1) * (Mathf.Abs(scale * model.Value / 100f) < 0.1f ? 0.1f : scale * model.Value / 100f));//> 0? 0.5f + usm.fluxSimpleSmooth * 1f : -0.5f + usm.fluxSimpleSmooth * 1f));
    }
}
