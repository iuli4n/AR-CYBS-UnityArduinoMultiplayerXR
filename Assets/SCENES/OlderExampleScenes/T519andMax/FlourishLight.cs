using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality;

public class FlourishLight : FlourishBehavior
{
    Light light;
    GameObject halo;
    public float maxLightIntensity;

    private void Awake()
    {
        light = transform.GetComponent<Light>();
        halo = transform.Find("HaloObject").gameObject;
    }

    override public void Activate()
    {
        light.intensity = maxLightIntensity;
        halo.SetActive(true);
    }

    override public void Deactivate()
    {
        light.intensity = 0;
        halo.SetActive(false);
    }
}
