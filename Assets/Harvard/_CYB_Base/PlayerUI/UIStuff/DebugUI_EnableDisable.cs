using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI_EnableDisable : MonoBehaviour
{
    public string keyCode = "K";
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public GameObject target;
    public GameObject target2;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            if (target)
                target.SetActive(!target.active);
            if (target2)
                target2.SetActive(!target2.active);

        }
    }
}
