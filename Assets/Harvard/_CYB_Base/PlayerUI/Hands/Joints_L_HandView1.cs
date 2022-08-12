using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joints_L_HandView1 : MonoBehaviour
{
    public JointsModelBothHandsNetworking myDataModel;
    public GameObject L_HandPivotOnWrist;

    /* SET ON INITIALIZE */
    public GameObject  L_mainJntObj,
    L_indexDistalJointObj, L_indexKnuckleObj, L_indexMiddleJointObj, 
    L_middleDistalJointObj, L_middleKnuckleObj, L_middleMiddleJointObj, 
    L_pinkyDistalJointObj,L_pinkyKnuckleObj, L_pinkyMiddleJointObj,
    L_ringDistalJointObj, L_ringKnuckleObj, L_ringMiddleJointObj,
    L_thumbDistalJointObj, L_thumbMetacarpalJointObj, L_thumbProximalJointObj;

    public float handDistanceMultiplier;
    public Transform cubeRot;


    // Start is called before the first frame update
    void Start()
    {
        myDataModel.onPoseUpdate += UpdateLeftHandModel;

        handDistanceMultiplier = 0f;
        

        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if(child.gameObject.name == "MainL_JNT")
            {
                L_mainJntObj = child.gameObject;
            }

            if (child.gameObject.name == "PointL_JNT3")
            {
                L_indexDistalJointObj = child.gameObject;
            }

            if (child.gameObject.name == "PointL_JNT2")
            {
                L_indexMiddleJointObj = child.gameObject;
            }

            if (child.gameObject.name == "PointL_JNT1")
            {
                L_indexKnuckleObj = child.gameObject;
            }

            if (child.gameObject.name == "MiddleL_JNT3")
            {
                L_middleDistalJointObj = child.gameObject;
            }

            if (child.gameObject.name == "MiddelL_JNT2")
            {
                L_middleMiddleJointObj = child.gameObject;
            }

            if (child.gameObject.name == "MiddleL_JNT1")
            {
                L_middleKnuckleObj = child.gameObject;
            }

            if (child.gameObject.name == "PinkyL_JNT3")
            {
                L_pinkyDistalJointObj = child.gameObject;
            }

            if (child.gameObject.name == "PinkyL_JNT2")
            {
                L_pinkyMiddleJointObj = child.gameObject;
            }

            if (child.gameObject.name == "PinkyL_JNT1")
            {
                L_pinkyKnuckleObj = child.gameObject;
            }

            if (child.gameObject.name == "RingL_JNT3")
            {
                L_ringDistalJointObj = child.gameObject;
            }

            if (child.gameObject.name == "RingL_JNT2")
            {
                L_ringMiddleJointObj = child.gameObject;
            }

            if (child.gameObject.name == "RingL_JNT1")
            {
                L_ringKnuckleObj = child.gameObject;
            }

            if (child.gameObject.name == "ThumbL_JNT3")
            {
                L_thumbDistalJointObj = child.gameObject;
            }

            if (child.gameObject.name == "ThumbL_JNT2")
            {
                L_thumbProximalJointObj = child.gameObject;
            }

            if (child.gameObject.name == "ThumbL_JNT1")
            {
                L_thumbMetacarpalJointObj = child.gameObject;
            }
        }
    }

    private void UpdateLeftHandModel()
    {


        L_HandPivotOnWrist.transform.position = myDataModel._WristPos + Vector3.forward * handDistanceMultiplier;
        L_HandPivotOnWrist.transform.rotation = myDataModel._WristRot; // * cubeRot.rotation; //* Quaternion.Euler(0f, -90f, 90f);
            
        //L_mainJntObj.transform.position = myDataModel._WristPos + Vector3.up * handDistanceMultiplier;
        //L_mainJntObj.transform.rotation = myDataModel._WristRot * Quaternion.Euler(0f, -90f, 90f);

        
        L_indexKnuckleObj.transform.rotation = myDataModel._IndexKnuckle * Quaternion.Euler(0f, -90f, 90f);
        L_indexMiddleJointObj.transform.rotation = myDataModel._IndexMiddleJoint * Quaternion.Euler(0f, -90f, 90f);
        L_indexDistalJointObj.transform.rotation = myDataModel._IndexDistalJoint * Quaternion.Euler(0f, -90f, 90f);


        L_middleKnuckleObj.transform.rotation = myDataModel._MiddleKnuckle * Quaternion.Euler(0f, -90f, 90f);
        L_middleMiddleJointObj.transform.rotation = myDataModel._MiddleMiddleJoint * Quaternion.Euler(0f, -90f, 90f);
        L_middleDistalJointObj.transform.rotation = myDataModel._MiddleDistalJoint * Quaternion.Euler(0f, -90f, 90f);

        L_ringKnuckleObj.transform.rotation = myDataModel._RingKnuckle * Quaternion.Euler(0f, -90f, 90f);
        L_ringMiddleJointObj.transform.rotation = myDataModel._RingMiddleJoint * Quaternion.Euler(0f, -90f, 90f);
        L_ringDistalJointObj.transform.rotation = myDataModel._RingDistalJoint * Quaternion.Euler(0f, -90f, 90f);


        L_pinkyKnuckleObj.transform.rotation = myDataModel._PinkyKnuckle * Quaternion.Euler(0f, -90f, 90f);
        L_pinkyMiddleJointObj.transform.rotation = myDataModel._PinkyMiddleJoint * Quaternion.Euler(0f, -90f, 90f);
        L_pinkyDistalJointObj.transform.rotation = myDataModel._PinkyDistalJoint * Quaternion.Euler(0f, -90f, 90f);


        L_thumbMetacarpalJointObj.transform.rotation = myDataModel._ThumbMetacarpalJoint * Quaternion.Euler(90f, -90f, 90f);
        L_thumbProximalJointObj.transform.rotation = myDataModel._ThumbProximalJoint * Quaternion.Euler(90f, -90f, 90f);
        L_thumbDistalJointObj.transform.rotation = myDataModel._ThumbDistalJoint * Quaternion.Euler(90f, -90f, 90f);
            
            /*
            L_thumbMetacarpalJointObj.transform.rotation = myDataModel._ThumbMetacarpalJoint * cubeRot.rotation;
            L_thumbProximalJointObj.transform.rotation = myDataModel._ThumbProximalJoint * cubeRot.rotation;
            L_thumbDistalJointObj.transform.rotation = myDataModel._ThumbDistalJoint * cubeRot.rotation;
            */
    }


    private void OnDestroy()
    {
        myDataModel.onPoseUpdate -= UpdateLeftHandModel;
    }
}