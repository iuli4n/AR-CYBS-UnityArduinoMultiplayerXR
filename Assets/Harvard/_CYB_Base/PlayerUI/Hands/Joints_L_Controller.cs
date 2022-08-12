using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joints_L_Controller : MonoBehaviour
{
    public JointsModelBothHandsNetworking myDataModel;

    private float nextUpdate;
    private void Update()
    {
        if (Time.time < nextUpdate) return; nextUpdate = Time.time + 0.1f;

        UpdateLeftHandJointsData();
    }

    public void UpdateLeftHandJointsData()
    {
        var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();

        if (handJointService != null)
        {
            myDataModel.DisablePoseUpdates();

            myDataModel._IndexDistalJoint = handJointService.RequestJointTransform(TrackedHandJoint.IndexDistalJoint, Handedness.Left).rotation;
            myDataModel._IndexKnuckle = handJointService.RequestJointTransform(TrackedHandJoint.IndexKnuckle, Handedness.Left).rotation;
            myDataModel._IndexMiddleJoint = handJointService.RequestJointTransform(TrackedHandJoint.IndexMiddleJoint, Handedness.Left).rotation;

            myDataModel._MiddleDistalJoint = handJointService.RequestJointTransform(TrackedHandJoint.MiddleDistalJoint, Handedness.Left).rotation;
            myDataModel._MiddleKnuckle = handJointService.RequestJointTransform(TrackedHandJoint.MiddleKnuckle, Handedness.Left).rotation;
            myDataModel._MiddleMiddleJoint = handJointService.RequestJointTransform(TrackedHandJoint.MiddleMiddleJoint, Handedness.Left).rotation;


            myDataModel._PinkyDistalJoint = handJointService.RequestJointTransform(TrackedHandJoint.PinkyDistalJoint, Handedness.Left).rotation;
            myDataModel._PinkyKnuckle = handJointService.RequestJointTransform(TrackedHandJoint.PinkyKnuckle, Handedness.Left).rotation;
            myDataModel._PinkyMiddleJoint = handJointService.RequestJointTransform(TrackedHandJoint.PinkyMiddleJoint, Handedness.Left).rotation;

            myDataModel._RingDistalJoint = handJointService.RequestJointTransform(TrackedHandJoint.RingDistalJoint, Handedness.Left).rotation;
            myDataModel._RingKnuckle = handJointService.RequestJointTransform(TrackedHandJoint.RingKnuckle, Handedness.Left).rotation;
            myDataModel._RingMiddleJoint = handJointService.RequestJointTransform(TrackedHandJoint.RingMiddleJoint, Handedness.Left).rotation;

            myDataModel._ThumbDistalJoint = handJointService.RequestJointTransform(TrackedHandJoint.ThumbDistalJoint, Handedness.Left).rotation;
            myDataModel._ThumbMetacarpalJoint = handJointService.RequestJointTransform(TrackedHandJoint.ThumbMetacarpalJoint, Handedness.Left).rotation;
            myDataModel._ThumbProximalJoint = handJointService.RequestJointTransform(TrackedHandJoint.ThumbProximalJoint, Handedness.Left).rotation;

            myDataModel._WristRot = handJointService.RequestJointTransform(TrackedHandJoint.Wrist, Handedness.Left).rotation;
            myDataModel._WristPos = handJointService.RequestJointTransform(TrackedHandJoint.Wrist, Handedness.Left).position;

            if (HandJointUtils.FindHand(Handedness.Left) == null)
            {
                myDataModel._WristPos = new Vector3(999f, 999f, 999f);
            }

            myDataModel.EnablePoseUpdates();
        }

        
    }
}
