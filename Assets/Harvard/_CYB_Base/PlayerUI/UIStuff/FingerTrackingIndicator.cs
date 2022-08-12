using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** PERFORMANCE PERFORMANCE 
 * PERFORMANCE PERFORMANCE : should use events rather than polling
 * PERFORMANCE PERFORMANCE 
 */
public class FingerTrackingIndicator : MonoBehaviour
{
    public GameObject trackingLossIndicator;

    // Start is called before the first frame update
    void Start()
    {

    }


    float nextUpdateTime;

    // Update is called once per frame
    void Update()
    {
        if (Time.time < nextUpdateTime) return;

        nextUpdateTime = Time.time + 0.01f;

        MixedRealityPose pose;
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out pose))
        {
            trackingLossIndicator.SetActive(false);
        }
        else
        {
            trackingLossIndicator.SetActive(true);
        }
    }
}