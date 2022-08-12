using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerInteractionConfigurer : MonoBehaviour
{
    // DESCRIPTION: Attached to any object that the user should be moving/scaling using pointers. 
    // This configures the object's BoundsControl/ObjectManipulator so it looks properly for the currently available interactions.
    //
    // Details: When a player hovers their pointer near an object (be it through a ray or hand), ObjectManipulator will
    // cause the BoundsControl to become visible, and it will look like it's configured here.


    // TODO: This currently is used in limited ways (ex: only for manipulating endpoints of paths).
    //       But it should be used with any player created objects (the code in PlayerCreatedObject for manipulating bounds should be moved here).
    // TODO: Might be good if this class was in charge of registering to the EditingManager (currently done by PCO)


    public bool initializeOnStart = false; // true if you want this to run automatically
    public bool disableAfterInitialization = false; // true for components that should only be manipulable later

    public bool isSpecialObject = false; // whether this is a special object like path endpoints
    public bool canScale = true; // whether the current object can be scaled

    public bool nowallowMoving, nowallowScaling, nowallowScalingShowHandles;
    public bool nowallowingHighlight, nowallowingUnhighlight;
    void Start()
    {
        if (initializeOnStart)
            AttachAndInitialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void AttachAndInitialize()
    {
        // Attach ObjectManipulator and BoundsControl if needed 

        if (!this.gameObject.GetComponent<ObjectManipulatorIuli>())
        {
            this.gameObject.AddComponent<ObjectManipulatorIuli>();
        }
        
        if (!this.gameObject.GetComponent<NearInteractionGrabbable>())
        {
            this.gameObject.AddComponent<NearInteractionGrabbable>();
        }

        if (!this.gameObject.GetComponent<BoundsControl>())
        {
            this.gameObject.AddComponent<BoundsControl>();
        }

        



        // configure BoundsControl

        var b = this.gameObject.GetComponent<BoundsControl>();
        if (true /*!debug_dontEnableBounds*/)
        {
            b.enabled = true;

            b.CalculationMethod = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.BoundsCalculationMethod.ColliderOverRenderer;
            b.BoundsControlActivation = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.BoundsControlActivationType.ActivateByProximityAndPointer;

            if (canScale)
            {
                b.ScaleHandlesConfig.ScaleBehavior = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.HandleScaleMode.NonUniform;
            } else
            {
                b.ScaleHandlesConfig.ShowScaleHandles = false;
            }
            b.RotationHandlesConfig.ShowHandleForX = false;
            b.RotationHandlesConfig.ShowHandleForY = false;
            b.RotationHandlesConfig.ShowHandleForZ = false;
            b.LinksConfig.ShowWireFrame = false;

            b.HandleProximityEffectConfig.ProximityEffectActive = true;
            b.HandleProximityEffectConfig.ObjectMediumProximity = 0.1f;
            b.HandleProximityEffectConfig.ObjectCloseProximity = 0.03f;
            b.HandleProximityEffectConfig.FarScale = 0.01f;
            b.HandleProximityEffectConfig.MediumScale = 0.5f;
            b.HandleProximityEffectConfig.CloseScale = 2f;

            /*** 
            // If bounds should be looking at another object, configure it here.
             
            if (this.gameObject.GetComponent<PlayerCreatedPrefab>())
            {
                BoxCollider bc = this.gameObject.GetComponent<PlayerCreatedPrefab>().myPrefabContainer.GetComponentInChildren<BoxCollider>();
                if (bc != null && bc.enabled)
                {
                    b.BoundsOverride = bc;
                }
            }
            if (this.gameObject.GetComponent<PlayerCreatedModel>())
            {
                BoxCollider bc = this.gameObject.GetComponent<PlayerCreatedModel>().myModelContainer.GetComponentInChildren<BoxCollider>();
                if (bc != null && bc.enabled)
                {
                    b.BoundsOverride = bc;
                }
            }
            ***/

            //was this.gameObject.GetComponent<ObjectManipulatorIuli>().OnHoverEntered.AddListener((x) => { this.gameObject.GetComponent<BoundsControl>().LinksConfig.ShowWireFrame = true; });
            //was this.gameObject.GetComponent<ObjectManipulatorIuli>().OnHoverExited.AddListener((x) => { this.gameObject.GetComponent<BoundsControl>().LinksConfig.ShowWireFrame = false; });


            this.gameObject.GetComponent<ObjectManipulatorIuli>().OnHoverEntered.AddListener((x) => {
                this.gameObject.GetComponent<BoundsControl>().LinksConfig.ShowWireFrame =
                    this.gameObject.GetComponent<PointerInteractionConfigurer>()?.nowallowingHighlight ?? false;
            });
            this.gameObject.GetComponent<ObjectManipulatorIuli>().OnHoverExited.AddListener((x) => {
                this.gameObject.GetComponent<BoundsControl>().LinksConfig.ShowWireFrame =
                    (!this.gameObject.GetComponent<PointerInteractionConfigurer>()?.nowallowingUnhighlight) ?? false;
            });
        }

        /*
        // If this object has Effects, will want to disable the Effects if manipulating

        this.gameObject.GetComponent<BoundsControl>().ScaleStarted.AddListener(() => { this.gameObject.GetComponent<Effect4Gen>()?.DisableEffects(); });
        this.gameObject.GetComponent<BoundsControl>().ScaleStopped.AddListener(() => { this.gameObject.GetComponent<Effect4Gen>()?.ResetBasesAndEnable(); });

        this.gameObject.GetComponent<ObjectManipulatorIuli>().OnManipulationStarted.AddListener((x) => { this.gameObject.GetComponent<Effect4Gen>()?.DisableEffects(); });
        this.gameObject.GetComponent<ObjectManipulatorIuli>().OnManipulationEnded.AddListener((x) => { this.gameObject.GetComponent<Effect4Gen>()?.ResetBasesAndEnable(); });
        
        */


        if (disableAfterInitialization)
        {
            SetActiveManipulation(false);
        }
    }

    void SetActiveManipulation(bool isActive)
    {
        this.gameObject.GetComponent<BoxCollider>().enabled = isActive;
        this.gameObject.GetComponent<BoundsControl>().enabled = isActive;
        this.gameObject.GetComponent<NearInteractionGrabbable>().enabled = isActive;
    }





    public void ConfigureInteraction(bool allowMove, bool allowScaling, bool allowScalingShowHandles, bool allowingHighlight, bool allowingUnhighlight)
    {
        nowallowMoving = allowMove;
        nowallowScaling = allowScaling;
        nowallowScalingShowHandles = allowScalingShowHandles;
        nowallowingHighlight = allowingHighlight;
        nowallowingUnhighlight = allowingUnhighlight;

        var b = this.GetComponent<BoundsControl>();

        if (!nowallowScaling)
        {
            // TODO: CHECK: if allowMoving but not scaling, does ProximityEffectActive still exist ?

            //Debug.Log("---");
            //Debug.Log(b);
            //Debug.Log(b.ScaleHandlesConfig);
            b.ScaleHandlesConfig.ShowScaleHandles = false;
            b.HandleProximityEffectConfig.ProximityEffectActive = false;

        }
        else
        {
            //Debug.Log("---");
            //Debug.Log(b);
            //Debug.Log(b.ScaleHandlesConfig);
            b.ScaleHandlesConfig.ShowScaleHandles = canScale && allowScalingShowHandles;
            b.HandleProximityEffectConfig.ProximityEffectActive = allowScalingShowHandles;
        }

        // if allowing highlight but not unhighlight it means these objects should be highlighted all the time
        if (nowallowingHighlight && !nowallowingUnhighlight)
        {
            this.gameObject.GetComponent<BoundsControl>().LinksConfig.ShowWireFrame = true;
        }


        // for objects that can't move/scale, turn off the interaction
        if (!nowallowMoving && !nowallowScaling)
        {
            if (this.GetComponent<ObjectManipulatorIuli>())
                this.GetComponent<ObjectManipulatorIuli>().enabled = false;
            if (this.GetComponent<BoundsControl>())
                this.GetComponent<BoundsControl>().enabled = false; // BUG? this might cause box bounds to change size 
        } else
        {
            if (this.GetComponent<ObjectManipulatorIuli>())
                this.GetComponent<ObjectManipulatorIuli>().enabled = true;
            if (this.GetComponent<BoundsControl>())
                this.GetComponent<BoundsControl>().enabled = true; 
        }
    }
}
