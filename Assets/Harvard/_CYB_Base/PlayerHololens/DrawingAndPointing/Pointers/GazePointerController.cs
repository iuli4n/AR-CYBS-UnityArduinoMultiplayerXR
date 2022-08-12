using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazePointerController : MonoBehaviour
{

    public GazePointerModel pointerModel;
    public GameObject anchorObject;


    void Start()
    {

    }

    public float nextUpdateTime = 0;
    void Update()
    {
        if (Time.time < nextUpdateTime) return; else nextUpdateTime = Time.time + 0.05f;

        ChangeStartPosition();
        ChangeDirection();
    }

    public void ChangeStartPosition()
    {
        pointerModel.StartPos = anchorObject.transform.position;
    
    }

    public void ChangeDirection()
    {
        pointerModel.Direction = anchorObject.transform.rotation;

    }
}
