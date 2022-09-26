using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlourishFrameController : FlourishBehavior
{
    override public void Activate()
    {
        foreach (Transform child in transform)
        {
            FlourishBehavior flourish = child.GetComponent<FlourishBehavior>();
            flourish.Activate();
        }
    }

    override public void Deactivate()
    {
        foreach (Transform child in transform)
        {
            FlourishBehavior flourish = child.GetComponent<FlourishBehavior>();
            flourish.Deactivate();
        }
    }
}
