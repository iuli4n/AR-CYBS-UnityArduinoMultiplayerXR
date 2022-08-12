using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTheCamera : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Camera.main.transform.position;
        this.transform.rotation = Camera.main.transform.rotation;
        this.transform.position = this.transform.position - this.transform.forward;
    }
}
