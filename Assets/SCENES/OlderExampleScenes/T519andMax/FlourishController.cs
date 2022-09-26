using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality;

public enum FlourishType { Light, Twinkle}
public class FlourishController : MonoBehaviour
{
    public FlourishType type = FlourishType.Light;
    // Start is called before the first frame update
    Light light;
    GameObject halo;
    ParticleSystem particles;
    public float maxLightIntensity;
    public GameObject bulb;
    public bool renderMarker;
    bool markerShowing;
    public bool activated;
    bool switchOn;


    void Start()
    {
        //activated = false;
        light = transform.GetComponent<Light>();
        halo = transform.Find("HaloObject").gameObject;
        particles = transform.GetComponent<ParticleSystem>();
        if (activated)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateActivation();
        UpdateRenderedMarker();
    }

    void UpdateRenderedMarker()
    {
        if (renderMarker && !markerShowing)
        {
            bulb.SetActive(true);
            markerShowing = true;
        }
        else if (!renderMarker && markerShowing)
        {
            bulb.SetActive(false);
            markerShowing = false;
        }
    }

    void UpdateActivation()
    {
        if (activated && !switchOn)
        {
            Activate();
            switchOn = true;
        }
        else if (!activated && switchOn)
        {
            Deactivate();
            switchOn = false;
        }
    }

    public void Activate()
    {
        switch (type)
        {
            case FlourishType.Light: ActivateLight(); break;
            case FlourishType.Twinkle: particles.Play(); break;
            default: Debug.Log("Activate"); break;
        }
    }

    public void Deactivate()
    {
        switch (type)
        {
            case FlourishType.Light: DeactivateLight(); break;
            case FlourishType.Twinkle: particles.Stop(); break;
            default: Debug.Log("Deactivate"); break;
        }
    }

    void DeactivateLight()
    {
        light.intensity = 0;
        halo.SetActive(false);
        //bulb.SetActive(false);
    }

    void ActivateLight()
    {
        light.intensity = maxLightIntensity;
        halo.SetActive(true);
        //bulb.SetActive(true);
    }

}
