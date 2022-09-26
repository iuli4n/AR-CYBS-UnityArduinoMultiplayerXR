using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlourishTwinkle : FlourishBehavior
{
    ParticleSystem particles;

    private void Awake()
    {
        particles = transform.GetComponent<ParticleSystem>();
    }

    override public void Activate()
    {
        particles.Play();
    }

    override public void Deactivate()
    {
        particles.Stop();
    }
}


