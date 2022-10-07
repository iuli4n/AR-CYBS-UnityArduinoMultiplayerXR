using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlourishBehavior : MonoBehaviour
{

    public bool activated;
    bool switchOn;
    // Start is called before the first frame update
    public void Start()
    {
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
    public void Update()
    {
        UpdateActivation();
    }

    public abstract void Activate();
    public abstract void Deactivate();

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
}
