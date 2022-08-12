using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTheFinger : MonoBehaviour
{
    public bool HACK_USELOCALPOS = false;

    public Camera cameraForProjectionTest;
    public bool doingProjectionTest;

    public float extraDistance = 0;


    public Handedness hand = Handedness.Right;
    public TrackedHandJoint joint = TrackedHandJoint.IndexTip;

    public GameObject overrideFingerObject = null; // if not null will use this

    public bool smoothing = false;

    float nextUpdateTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Vector3 destinationPos;
    public Quaternion destinationRot;

    
    // Update is called once per frame
    void Update()
    {

        if (smoothing)
        {
            const float smoothFactor = 10f;
            if (HACK_USELOCALPOS)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, destinationPos, smoothFactor * Time.deltaTime);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, destinationRot, smoothFactor * Time.deltaTime);
            } else
            {
                transform.position = Vector3.Lerp(transform.position, destinationPos, smoothFactor * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, destinationRot, smoothFactor * Time.deltaTime);
            }
            
        } 

        if (Time.time < nextUpdateTime) return;
        nextUpdateTime = Time.time + 0.001f;
        
        if (overrideFingerObject != null)
        {
            destinationPos = overrideFingerObject.transform.position;
            destinationRot = overrideFingerObject.transform.rotation;
        }
        else
        {
            var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
            if (handJointService != null)
            {
                Transform jointTransform = handJointService.RequestJointTransform(joint, hand);
                destinationPos = jointTransform.position + jointTransform.forward * extraDistance;
                destinationRot = jointTransform.rotation;
            }
        }
        
        if (doingProjectionTest)
        {
            // if doing projection test, will take the destination (ex: tip of the finger) and raycast through it and then move it to the raycast point
            

            Ray ray2 = cameraForProjectionTest.ScreenPointToRay(cameraForProjectionTest.WorldToScreenPoint(destinationPos));
            RaycastHit hit2;
            if (Physics.Raycast(ray2, out hit2))
            {
                destinationPos = hit2.point;
            }
            else
            {
                // din't hit anything, move out of the way
                destinationPos = Vector3.one * 2000f;
            }
        }

        if (!smoothing)
        {
            transform.position = destinationPos;
            transform.rotation = destinationRot;
        }
    }
}
