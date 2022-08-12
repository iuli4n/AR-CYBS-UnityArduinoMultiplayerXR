using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HLUI_AlignToFinger : MonoBehaviour
{
    public GameObject newAttachment;
    public GameObject fingerTipAnchor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.X))
        {
            newAttachment.transform.position = fingerTipAnchor.transform.position;
            newAttachment.transform.rotation = fingerTipAnchor.transform.rotation;
        }


    }
}
