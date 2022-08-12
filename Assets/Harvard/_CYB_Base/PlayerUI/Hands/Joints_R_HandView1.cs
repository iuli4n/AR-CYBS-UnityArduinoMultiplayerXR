using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joints_R_HandView1 : MonoBehaviour
{
    public JointsModelBothHandsNetworking myDataModel;
    public GameObject R_HandPivotOnWrist;

    /* SET ON INITIALIZE */
    public GameObject R_mainJntObj,
    R_indexDistalJointObj, R_indexKnuckleObj, R_indexMiddleJointObj, 
    R_middleDistalJointObj, R_middleKnuckleObj, R_middleMiddleJointObj, 
    R_pinkyDistalJointObj,R_pinkyKnuckleObj, R_pinkyMiddleJointObj,
    R_ringDistalJointObj, R_ringKnuckleObj, R_ringMiddleJointObj,
    R_thumbDistalJointObj, R_thumbMetacarpalJointObj, R_thumbProximalJointObj;

    public float handDistanceMultiplier;


    // Start is called before the first frame update
    void Start()
    {
        myDataModel.onPoseUpdate += UpdateRightHandModel;

        handDistanceMultiplier = 0f;

        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if(child.gameObject.name == "MainR_JNT")
            {
                R_mainJntObj = child.gameObject;
            }

            if (child.gameObject.name == "PointR_JNT3")
            {
                R_indexDistalJointObj = child.gameObject;
            }

            if (child.gameObject.name == "PointR_JNT2")
            {
                R_indexMiddleJointObj = child.gameObject;
            }

            if (child.gameObject.name == "PointR_JNT1")
            {
                R_indexKnuckleObj = child.gameObject;
            }

            if (child.gameObject.name == "MiddleR_JNT3")
            {
                R_middleDistalJointObj = child.gameObject;
            }

            if (child.gameObject.name == "MiddelR_JNT2")
            {
                R_middleMiddleJointObj = child.gameObject;
            }

            if (child.gameObject.name == "MiddleR_JNT1")
            {
                R_middleKnuckleObj = child.gameObject;
            }

            if (child.gameObject.name == "PinkyR_JNT3")
            {
                R_pinkyDistalJointObj = child.gameObject;
            }

            if (child.gameObject.name == "PinkyR_JNT2")
            {
                R_pinkyMiddleJointObj = child.gameObject;
            }

            if (child.gameObject.name == "PinkyR_JNT1")
            {
                R_pinkyKnuckleObj = child.gameObject;
            }

            if (child.gameObject.name == "RingR_JNT3")
            {
                R_ringDistalJointObj = child.gameObject;
            }

            if (child.gameObject.name == "RingR_JNT2")
            {
                R_ringMiddleJointObj = child.gameObject;
            }

            if (child.gameObject.name == "RingR_JNT1")
            {
                R_ringKnuckleObj = child.gameObject;
            }

            if (child.gameObject.name == "ThumbR_JNT3")
            {
                R_thumbDistalJointObj = child.gameObject;
            }

            if (child.gameObject.name == "ThumbR_JNT2")
            {
                R_thumbProximalJointObj = child.gameObject;
            }

            if (child.gameObject.name == "ThumbR_JNT1")
            {
                R_thumbMetacarpalJointObj = child.gameObject;
            }
        }
    }

    private void UpdateRightHandModel()
    {
        R_HandPivotOnWrist.transform.position = myDataModel._WristPos + Vector3.forward * handDistanceMultiplier;
        R_HandPivotOnWrist.transform.rotation = myDataModel._WristRot; // * cubeRot.rotation; //* Quaternion.Euler(0f, -90f, 90f);


        //R_mainJntObj.transform.position = myDataModel._WristPos + Vector3.forward * handDistanceMultiplier;
        //R_mainJntObj.transform.rotation = myDataModel._WristRot * Quaternion.Euler(0f, -90f, -90f);


        R_indexKnuckleObj.transform.rotation = myDataModel._IndexKnuckle * Quaternion.Euler(0f, -90f, -90f);
        R_indexMiddleJointObj.transform.rotation = myDataModel._IndexMiddleJoint * Quaternion.Euler(0f, -90f, -90f);
        R_indexDistalJointObj.transform.rotation = myDataModel._IndexDistalJoint * Quaternion.Euler(0f, -90f, -90f);


        R_middleKnuckleObj.transform.rotation = myDataModel._MiddleKnuckle * Quaternion.Euler(0f, -90f, -90f);
        R_middleMiddleJointObj.transform.rotation = myDataModel._MiddleMiddleJoint * Quaternion.Euler(0f, -90f, -90f);
        R_middleDistalJointObj.transform.rotation = myDataModel._MiddleDistalJoint * Quaternion.Euler(0f, -90f, -90f);

        R_ringKnuckleObj.transform.rotation = myDataModel._RingKnuckle * Quaternion.Euler(0f, -90f, -90f);
        R_ringMiddleJointObj.transform.rotation = myDataModel._RingMiddleJoint * Quaternion.Euler(0f, -90f, -90f);
        R_ringDistalJointObj.transform.rotation = myDataModel._RingDistalJoint * Quaternion.Euler(0f, -90f, -90f);


        R_pinkyKnuckleObj.transform.rotation = myDataModel._PinkyKnuckle * Quaternion.Euler(0f, -90f, -90f);
        R_pinkyMiddleJointObj.transform.rotation = myDataModel._PinkyMiddleJoint * Quaternion.Euler(0f, -90f, -90f);
        R_pinkyDistalJointObj.transform.rotation = myDataModel._PinkyDistalJoint * Quaternion.Euler(0f, -90f, -90f);


        R_thumbMetacarpalJointObj.transform.rotation = myDataModel._ThumbMetacarpalJoint * Quaternion.Euler(-90f, -90f, -90f);
        R_thumbProximalJointObj.transform.rotation = myDataModel._ThumbProximalJoint * Quaternion.Euler(-90f, -90f, -90f);
        R_thumbDistalJointObj.transform.rotation = myDataModel._ThumbDistalJoint * Quaternion.Euler(-90f, -90f, -90f);

    }
    private void OnDestroy()
    {
        myDataModel.onPoseUpdate -= UpdateRightHandModel;
    }
}
