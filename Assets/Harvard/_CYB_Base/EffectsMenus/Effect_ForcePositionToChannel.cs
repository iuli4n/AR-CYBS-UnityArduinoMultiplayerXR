using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_ForcePositionToChannel : MonoBehaviour
{
    public AtomicDataSwitch ad_x, ad_y;

    public float xscale = 1f;
    public float xshift = 0.1f;

    public float yscale = 1f;
    public float yshift = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float zz = this.gameObject.transform.localPosition.z;
        this.gameObject.transform.localPosition = new Vector3(ad_x.Value * xscale - xshift, ad_y.Value * yscale - yshift, zz);
    }
}
