using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class PlayerCreatedObject : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    // DESCRIPTION: This script should be attached to any objects that a player can create at runtime. This takes care of configuring that object
    // including setting up ObjectManipulator and BoundsControl and Effects, and connecting them together.

    // TODO / ALSO SEE: PointerInteractionConfigurer because that class should be dealing with the pointer configuration for these objects.


    public bool autoAttachScripts = true; // whether we are attaching scripts
    public bool autoConfigureScripts = true; // whether we are configuring scripts when the object is photon-instantiated
    public bool debug_dontEnableBounds = false;
    public bool isConfigured = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (!this.enabled) 
            return;

        if (autoAttachScripts)
            AttachAllScripts();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isConfigured && autoConfigureScripts)
        {
            ConfigureAllScripts();
            EditingManager.Instance.RegisterPlayerObject(this);
        }

        /*
        if (!isConfigured && PhotonNetwork.InRoom)
        {
            isConfigured = true;
        }
        */
    }


    void RequestOwnershipForThisObject(ManipulationEventData data)
    {
        // NOTE this function doesn't wait to see if ownership is granted or not
        photonView.RequestOwnership();
    }


    private void AttachAllScripts()
    {
        // is not configured for being manipulated - assume it just has a PlayerCreatedObject

        if (!this.gameObject.GetComponent<PointerInteractionConfigurer>())
        {
            var pic = this.gameObject.AddComponent<PointerInteractionConfigurer>();
            
            pic.initializeOnStart = false; // TODO: this should be true and all initialization should be done there not in PlayerCreatedObject
            pic.disableAfterInitialization = false;
        }

        if (!this.gameObject.GetComponent<ObjectManipulatorIuli>())
        {
            this.gameObject.AddComponent<ObjectManipulatorIuli>().enabled = false;
        }


        if (!this.gameObject.GetComponent<AttachToSceneStep>())
        {
            this.gameObject.AddComponent<AttachToSceneStep>().enabled = false;
        }

        /*
        if (!this.gameObject.GetComponent<Effect4Gen>())
        {
            this.gameObject.AddComponent<Effect4Gen>().enabled = false;
        }
        */


        if (!this.gameObject.GetComponent<BoundsControl>())
        {
            this.gameObject.AddComponent<BoundsControl>();
        }
        

        //this.gameObject.AddComponent<ConstraintManager>();
        if (!this.gameObject.GetComponent<NearInteractionGrabbable>())
        {
            this.gameObject.AddComponent<NearInteractionGrabbable>().enabled = false;
        }
    }

    private void ConfigureAllScripts()
    {



        var e = this.gameObject.GetComponent<Effect4Model>();
        if (e)
        {
            e.enabled = true;
        }


        // set up photon
        photonView.OwnershipTransfer = OwnershipOption.Takeover;
        PhotonTransformView ptv = this.gameObject.GetComponent<PhotonTransformView>();
        {
            ptv.m_SynchronizeRotation = true;
            ptv.m_SynchronizePosition = true;
            ptv.m_SynchronizeScale = true;
        }

        // set up bounds and object manipulation

        var b = this.gameObject.GetComponent<BoundsControl>();
        if (!debug_dontEnableBounds) {
            b.enabled = true;

            b.CalculationMethod = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.BoundsCalculationMethod.ColliderOverRenderer; 
            b.BoundsControlActivation = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.BoundsControlActivationType.ActivateByProximityAndPointer;

            b.ScaleHandlesConfig.ScaleBehavior = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.HandleScaleMode.NonUniform;
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

            // NOTE: The boundaries of the BoundsControl may be custom set to colliders by the PlayerCreatedPrefab/Model

            this.gameObject.GetComponent<ObjectManipulatorIuli>().OnHoverEntered.AddListener((x) => {
                this.gameObject.GetComponent<BoundsControl>().LinksConfig.ShowWireFrame =
                    this.gameObject.GetComponent<PointerInteractionConfigurer>()?.nowallowingHighlight ?? false;
            });
            this.gameObject.GetComponent<ObjectManipulatorIuli>().OnHoverExited.AddListener((x) => {
                this.gameObject.GetComponent<BoundsControl>().LinksConfig.ShowWireFrame =
                    (!this.gameObject.GetComponent<PointerInteractionConfigurer>()?.nowallowingUnhighlight) ?? false;
            });

            this.gameObject.GetComponent<BoundsControl>().ScaleStarted.AddListener(() => { this.gameObject.GetComponent<Effect4Gen>()?.DisableEffects(); });
            this.gameObject.GetComponent<BoundsControl>().ScaleStopped.AddListener(() => { this.gameObject.GetComponent<Effect4Gen>()?.ResetBasesAndEnable(); });
            
        }

        this.gameObject.GetComponent<ObjectManipulatorIuli>().OnManipulationStarted.AddListener((x) => { this.gameObject.GetComponent<Effect4Gen>()?.DisableEffects(); });
        this.gameObject.GetComponent<ObjectManipulatorIuli>().OnManipulationEnded.AddListener((x) => { this.gameObject.GetComponent<Effect4Gen>()?.ResetBasesAndEnable(); });

        if (this.gameObject.GetComponent<BoxCollider>())
            this.gameObject.GetComponent<BoxCollider>().enabled = true;

        /*
        if (this.gameObject.GetComponent<AttachToSceneStep>())
            Debug.LogWarning("PlayerCreatedObject.configure not enabling AttachToSceneStep");
        */

        
        if (/*ok to attach to scene*/ this.gameObject.GetComponent<AttachToSceneStep>() && !(TEMP_Debug_DemoSequenceTools.Instance && TEMP_Debug_DemoSequenceTools.Instance.Override_DontAttachToScene))
        {
            this.gameObject.GetComponent<AttachToSceneStep>().enabled = true;
        }

        this.gameObject.GetComponent<NearInteractionGrabbable>().enabled = true;
        this.gameObject.GetComponent<ObjectManipulatorIuli>().enabled = true;

        if ( /*ok to use e4g*/ this.gameObject.GetComponent<Effect4Gen>() && !(TEMP_Debug_DemoSequenceTools.Instance && TEMP_Debug_DemoSequenceTools.Instance.Override_DontEffect4Gen))
        {
            Effect4Gen e4 = this.gameObject.GetComponent<Effect4Gen>();
            
            // if we're a prefab object attach the effected model automatically
            if (!e4.override_effectedModel)
            {
                if (this.gameObject.GetComponent<PlayerCreatedPrefab>())
                {
                    e4.override_effectedModel = this.gameObject.GetComponent<PlayerCreatedPrefab>().myPrefabContainer;
                }
                else if (this.gameObject.GetComponent<PlayerCreatedModel>())
                {
                    e4.override_effectedModel = this.gameObject.GetComponent<PlayerCreatedModel>().myModelContainer;
                }
                else if (this.gameObject.GetComponent<PlayerCreatedImage>())
                {
                    e4.override_effectedModel = this.gameObject.GetComponent<PlayerCreatedImage>().currentInstantiatedTexture;
                }
                else if (this.gameObject.GetComponent<PlayerCreatedDrawing>())
                {
                    e4.override_effectedModel = this.gameObject.GetComponent<PlayerCreatedDrawing>().drawingContainer;
                }
                else
                {
                    Debug.LogWarning("PCO.E4G: Created an object but E4G won't have an override model because I dont know what to assign");
                }
            }

            ///////e4.enabled = true;

        }

        // set up the Object Manipulator so it gets ownership whenever the object gets manipulated
        this.gameObject.GetComponent<ObjectManipulatorIuli>().OnManipulationStarted.AddListener((x) =>
        {
            photonView.RequestOwnership();

            if (this.gameObject.GetComponent<PlayerCreatedImage>())
            {
                this.gameObject.GetComponent<PlayerCreatedImage>().currentInstantiatedTexture.GetPhotonView().RequestOwnership();
            }
            if (this.gameObject.GetComponent<PlayerCreatedModel>())
            {
                this.gameObject.GetComponent<PlayerCreatedModel>().myModelContainer.GetPhotonView().RequestOwnership();
            }
            if (this.gameObject.GetComponent<PlayerCreatedPrefab>())
            {
                this.gameObject.GetComponent<PlayerCreatedPrefab>().myPrefabContainer.GetPhotonView().RequestOwnership();
            }

            //var e = this.gameObject.GetComponent<Effect4Model>();
            //if (e) e.photonView.RequestOwnership();
        });


        if (this.gameObject.GetComponent<PlayerCreatedDrawing>())
        {
            // BUG: disable effects because i think bug
            this.gameObject.GetComponent<Effect4Gen>().enabled = false;

            this.gameObject.GetComponent<PlayerCreatedDrawing>().RecalculateBounds();
        }

        /****
        if (this.gameObject.name.ToUpper().Contains("TOOLTIP") || debug_dontEnableBounds)
        {
            Debug.LogWarning("PlayerCreatedObject.configure removing tooltip bounds control");
            var c = this.gameObject.GetComponent<BoundsControl>();
            if (c) 
                GameObject.Destroy(c);
            
            string name = "rigRoot";
            foreach (Transform t in (this.transform.GetComponentsInChildren<Transform>().Where(t => t.name == name).ToArray()))
            {
                // make sure this is related to bounds
                var c2 = t.gameObject.GetComponent<PointerHandler>();
                if (c2)
                {
                    GameObject.Destroy(t.gameObject);
                }
            }

            // since we've just destroyed boundscontrol, tell the editin manager we won't use this
            EditingManager.Instance.UnRegisterPlayerObject(this);
        }
        *****/

        isConfigured = true;
    }

    private void OnDestroy()
    {
        EditingManager.Instance.UnRegisterPlayerObject(this);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("*** onphotoninstantiate player created object ***");

        object[] instantiationData = info.photonView.InstantiationData;

        int i = 0;
        //int newobjectviewid = (int)instantiationData[0];
        string newObjectName = (string)instantiationData[i++];
        Vector3 localScale = (Vector3)instantiationData[i++];
        Vector3 position = (Vector3)instantiationData[i++];

        //GameObject newObject = this.gameObject;// PhotonView.Find(newobjectviewid).gameObject;
        // PERF: Not sure if this Find is needed anymore because we are that object

        this.gameObject.name = newObjectName;
        this.gameObject.transform.position = position;

        if (localScale.x != Mathf.NegativeInfinity)
        {
            this.gameObject.transform.localScale = localScale;
        }


        // Do custom initialization

        int mediaType = (int)instantiationData[i++];

        if (mediaType == 0)
        {
            // regular prefab object
            
            // Now (this runs on every client) tell the PCP to connect itself to the subobject

            //OLDER: string prefabPath = (string)instantiationData[i++];

            int subObjectPhotonId = (int)instantiationData[i++];
            this.gameObject.GetComponent<PlayerCreatedPrefab>().ConnectToExistingObject(subObjectPhotonId, newObjectName);
        }
        
        if (mediaType == 1)
        {
            // special object: image
            string imagePath = (string)instantiationData[i++];

            this.gameObject.GetComponent<PlayerCreatedImage>().SetAndRefresh(imagePath);
        }

        if (mediaType == 2)
        {
            // special object: raw model
            string drivePath = (string)instantiationData[i++];
            Vector3 modelOffset = (Vector3)instantiationData[i++];

            this.gameObject.GetComponent<PlayerCreatedModel>().SetAndRefresh(drivePath, modelOffset);
        }
        
        if (mediaType == 3)
        {
            // special object: line
            Vector3[] positions = (Vector3[])instantiationData[i++];

            Debug.Log("Instantiating with drawing line positions: " + positions.Length);
            this.gameObject.GetComponent<PlayerCreatedDrawing>().CreateFromPositions(positions);
        }

        if (mediaType == 99)
        {
            // special object without any data
        }

    }

}
