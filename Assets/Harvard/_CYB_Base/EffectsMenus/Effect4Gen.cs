using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Effect4Gen : MonoBehaviour
{
    // TODO: change the location of where plugins get created; make an EffectsData child and put the EffectsModel & Plugins in there

    public GameObject[] pluginsToInstantiate_runBefore;
    public GameObject[] pluginsToInstantiate_runAfter;
    [SerializeField]
    bool pluginsAreInstantiated;

    GameObject pluginsRoot;
    [SerializeField]
    public List<AEffect4PluginManager> myPlugins_before = new List<AEffect4PluginManager>();
    [SerializeField]
    public List<AEffect4PluginManager> myPlugins_after = new List<AEffect4PluginManager>();

    public AtomicDataModel genericModel;
    //public bool useGenericModelForAll = true;

    public bool effectsDisabled = false; // if true nothing will run

    public GameObject override_effectedModel;
    public BoundsControl effectedBounds;
    public float override_modelScaleFactor;
    
    Effect4Model myData;
    public PhotonView photonViewOfParent;

    public enum VisibilityEffectType
    {
        DisableRenderer,
        DisableObject
    }
    public VisibilityEffectType visibilityEffectType;
    public GameObject visibilityDisableObject;


    #region InitializationAndEnabling

    public void DisableEffects()
    {
        effectsDisabled = true;
    }
    public void EnableEffects()
    {
        effectsDisabled = false;
    }
    
    // Start is called before the first frame update
    void Start()
    {
    }
    private void Awake()
    {
        pluginsRoot = gameObject.transform.Find("EffectsModel").gameObject;
        myData = this.GetComponentInChildren<Effect4Model>();

        // Instantiate plugins 

        if (!pluginsAreInstantiated)
        {
            foreach (GameObject p in pluginsToInstantiate_runBefore)
            {
                GameObject plugin = PhotonView.Instantiate(p, pluginsRoot.transform);
                // note, at this point it's likely the plugin will be in an object that's disabled (because this Effect4Gen is usually disabled by default)

                AEffect4PluginManager pluginManager = plugin.GetComponent<AEffect4PluginManager>();
                myPlugins_before.Add(pluginManager);
                pluginManager.OnCreated();
                myData.pluginsModels.Add(pluginManager.GetModel());
            }
            foreach (GameObject p in pluginsToInstantiate_runAfter)
            {
                GameObject plugin = PhotonView.Instantiate(p, pluginsRoot.transform);
                // note, at this point it's likely the plugin will be in an object that's disabled (because this Effect4Gen is usually disabled by default)

                AEffect4PluginManager pluginManager = plugin.GetComponent<AEffect4PluginManager>();
                myPlugins_after.Add(pluginManager);
                pluginManager.OnCreated();
                myData.pluginsModels.Add(pluginManager.GetModel());
            }

            pluginsAreInstantiated = true;
        }
    }
    private void OnEnable()
    {
        Initialize();
    }
    private void OnDisable()
    {
        // reset the way the object looks
        bool tmp = effectsDisabled;
        effectsDisabled = true;
        UpdateEverything();
        effectsDisabled = tmp; 
        

        Uninitialize();
    }
    void Uninitialize()
    {
        if (myData != null)
        {
            genericModel.OnDataUpdated -= VisualsNeedUpdate;
            /*
            myData.vis_model.OnDataUpdated -= VisualsNeedUpdate;
            myData.rx_model.OnDataUpdated -= VisualsNeedUpdate;
            myData.ry_model.OnDataUpdated -= VisualsNeedUpdate;
            myData.rz_model.OnDataUpdated -= VisualsNeedUpdate;
            myData.sx_model.OnDataUpdated -= VisualsNeedUpdate;
            myData.sy_model.OnDataUpdated -= VisualsNeedUpdate;
            myData.sz_model.OnDataUpdated -= VisualsNeedUpdate;
            */
        }

    }
    void Initialize() { 
        photonViewOfParent = this.GetComponent<PhotonView>();

        
        
        if (true)
        {
            if (genericModel == null) 
                genericModel = myData.GetComponentInChildren<AtomicDataSwitch>();
            
            /***
            if (!genericModel)
            {
                genericModel = this.gameObject.AddComponent<AtomicDataSwitch>();
                ((AtomicDataSwitch)genericModel).SetCurrentChannel("C2");
            }
            ****/

            /*
            myData.vis_model = genericModel;
            myData.sx_model = genericModel;
            myData.sy_model = genericModel;
            myData.sz_model = genericModel;
            myData.rx_model = genericModel;
            myData.ry_model = genericModel;
            myData.rz_model = genericModel;
            */
        }


        bool tmp = effectsDisabled;
        ResetBasesAndEnable();
        effectsDisabled = tmp;


        /*** only use in OnEnable
        myData.vis_model.OnDataUpdated -= VisualsNeedUpdate;
        myData.rx_model.OnDataUpdated -= VisualsNeedUpdate;
        myData.ry_model.OnDataUpdated -= VisualsNeedUpdate;
        myData.rz_model.OnDataUpdated -= VisualsNeedUpdate;
        myData.sx_model.OnDataUpdated -= VisualsNeedUpdate;
        myData.sy_model.OnDataUpdated -= VisualsNeedUpdate;
        myData.sz_model.OnDataUpdated -= VisualsNeedUpdate;
        */

        genericModel.OnDataUpdated += VisualsNeedUpdate;
        /*
        myData.vis_model.OnDataUpdated += VisualsNeedUpdate;
        myData.rx_model.OnDataUpdated += VisualsNeedUpdate;
        myData.ry_model.OnDataUpdated += VisualsNeedUpdate;
        myData.rz_model.OnDataUpdated += VisualsNeedUpdate;
        myData.sx_model.OnDataUpdated += VisualsNeedUpdate;
        myData.sy_model.OnDataUpdated += VisualsNeedUpdate;
        myData.sz_model.OnDataUpdated += VisualsNeedUpdate;
        */


        
    }

    #endregion

    #region Running

    private bool visualsNeedUpdate = false;

    void VisualsNeedUpdate(float newval)
    {
        //Debug.Log("viscallback called");
        visualsNeedUpdate = true;
    }

    private float nextForcedUpdateTime = 0;
    private float FORCEDUPDATEINTERVAL = 0.1f;


    public int DEBUG_OUTPUT;

    void Update()
    {
        DEBUG_OUTPUT = myData.pluginsModels.Count;

        if (effectsDisabled)
            return;

        if (!photonViewOfParent.IsMine)
            // i'm not in control of this object right now
            return;

        if (Time.time > nextForcedUpdateTime)
        {
            nextForcedUpdateTime = Time.time + FORCEDUPDATEINTERVAL;
            visualsNeedUpdate = true;
        }

        if (visualsNeedUpdate)
        {

            UpdateEverything();

            visualsNeedUpdate = false;
        }
    }


    public float currentSensorValue;
    public float lastSensorValue;

    void UpdateEverything()
    {
        // TODO:PERF: Do this infrequently




        currentSensorValue = modelValue;

        // TODO: get rid of changing the parent, just use override model
        GameObject affectedObject = (override_effectedModel ? override_effectedModel : this.gameObject);

        // all these will be overriden        
        Vector3 newLocalScale = Vector3.one;
        Quaternion newLocalRotation = Quaternion.identity;
        Vector3 newLocalPosition = Vector3.zero;



        // LOOP THROUGH THE BEFORE PLUGINS

        foreach (var plugin in myPlugins_before)
        {
            plugin.RespondToNewValue(

                currentSensorValue, lastSensorValue,
                newLocalPosition, newLocalRotation, newLocalScale,

                ref currentSensorValue,

                // NOTE: the BEFORE effects shouldn't change object transform because they're overwritten by the Normal Effects. 
                ref newLocalPosition, ref newLocalRotation, ref newLocalScale);
        }


        if (UpdateVisiblity(currentSensorValue))
        {
            // EFFECTS ARE ONLY EXECUTED IF VISIBILITY IS ENABLED


            // NORMAL EFFECTS

            UpdateRotation(currentSensorValue, ref newLocalRotation);
            UpdateScale(currentSensorValue, ref newLocalScale, ref newLocalPosition);


            // LOOP THROUGH THE AFTER PLUGINS

            foreach (var plugin in myPlugins_after)
            {
                // Run the plugin
                // The previous step has updated the newLocalPosition/Rotation/Scale, so we'll just pass the same variables as the "base" for the plugin
                // and we pass the same variables by reference for the outputs of the plugin.
                plugin.RespondToNewValue(

                    currentSensorValue, lastSensorValue,
                    newLocalPosition, newLocalRotation, newLocalScale,

                    ref currentSensorValue,
                    ref newLocalPosition, ref newLocalRotation, ref newLocalScale);
            }
        }


        affectedObject.transform.localRotation = newLocalRotation;
        affectedObject.transform.localPosition = newLocalPosition;
        affectedObject.transform.localScale = newLocalScale;

        lastSensorValue = currentSensorValue;
    }

    #endregion

    #region NormalEffects

    float modelValue
    {
        // get a prettier model value that doesn't error divide by 0
        get
        {
            return (genericModel.Value == 0 ? 0.0001f : genericModel.Value);
        }
    }

    public void ResetBasesAndEnable()
    {
        myData.s_base = Vector3.one; // (override_effectedModel ? override_effectedModel : this.gameObject).transform.localScale;
        myData.r_base = Vector3.zero; // (override_effectedModel ? override_effectedModel : this.gameObject).transform.localRotation.eulerAngles;
        EnableEffects();
    }


    bool UpdateVisiblity(float modelValue)
    {
        if (myData.vis_enabled)
        {
            var enabled =
                    !effectsDisabled && myData.vis_thresh < modelValue ?
                        myData.vis_visibleBelow : myData.vis_visibleAbove;

            if (visibilityEffectType == VisibilityEffectType.DisableRenderer)
            {
                this.GetComponent<MeshRenderer>().enabled = enabled;
            } else
            {
                if (visibilityDisableObject == null)
                {
                    if (override_effectedModel)
                    {
                        visibilityDisableObject = override_effectedModel;
                    } 
                    else

                    if (this.gameObject.GetComponent<PlayerCreatedPrefab>())
                    {
                        visibilityDisableObject = this.gameObject.GetComponent<PlayerCreatedPrefab>().myPrefabContainer;
                    } else if (this.gameObject.GetComponent<PlayerCreatedModel>())
                    {
                        visibilityDisableObject = this.gameObject.GetComponent<PlayerCreatedModel>().myModelContainer;
                    } else if (this.gameObject.GetComponent <PlayerCreatedImage>())
                    {
                        visibilityDisableObject = this.gameObject.GetComponent<PlayerCreatedImage>().currentInstantiatedTexture;
                    } else
                    {
                        Debug.LogError("Effect is trying to disable visibility but don't know what object to disable");
                    }
                }
                visibilityDisableObject.SetActive(enabled);
            }
            return enabled;
        } else
        {
            return true;
            // TODO:PERF: if currently disabled, should return false
        }
    }

    void UpdateRotation(float modelValue, ref Quaternion outLocalRotation)
    {
        //if (!(myData.rx_enabled || myData.ry_enabled || myData.rz_enabled))
        //    return;

        /*
        if (model.Value < 30)
        {
            currentAngle += (flippedRotation ? -1 : 1) * scale * model.Value / 100f * Time.deltaTime * 50f;
        }
        */


        float rx = !effectsDisabled && myData.rx_enabled ? myData.rx_mult * modelValue / 2f : 0f;
        float ry = !effectsDisabled && myData.ry_enabled ? myData.ry_mult * modelValue / 2f : 0f;
        float rz = !effectsDisabled && myData.rz_enabled ? myData.rz_mult * modelValue / 2f : 0f;

        outLocalRotation = Quaternion.Euler(rx + myData.r_base.x, ry + myData.r_base.y, rz + myData.r_base.z);
        //(override_effectedModel ? override_effectedModel : this.gameObject).transform.localRotation = Quaternion.Euler(rx + myData.r_base.x, ry + myData.r_base.y, rz + myData.r_base.z);
    }

    // Update is called once per frame
    void UpdateScale(float modelValue, ref Vector3 outLocalScale, ref Vector3 outLocalPosition)
    {
        //if (!(myData.sx_enabled || myData.sy_enabled || myData.sz_enabled)) 
        //    return;

        // TEMPORARY
        //bool myDatasxEnabledA = myData.sxEnabledA || myData.sxEnabledB || myData.sx_enabled;
        //bool myDatasxEnabledB = myData.sxEnabledA || myData.sxEnabledB || myData.sx_enabled;

        float sx = !effectsDisabled && (myData.sxEnabledA || myData.sxEnabledB) ? myData.sx_mult * modelValue / 100f : 1f;
        float sy = !effectsDisabled && (myData.syA || myData.syB) ? myData.sy_mult * modelValue / 100f : 1f;
        float sz = !effectsDisabled && (myData.szA || myData.szB) ? myData.sz_mult * modelValue / 100f : 1f;
        //float sy = !effectsDisabled && myData.sy_enabled ? myData.sy_mult * modelValue / 100f : 1f;
        //float sz = !effectsDisabled && myData.sz_enabled ? myData.sz_mult * modelValue / 100f : 1f;

        /**
        BoxCollider box = this.GetComponent<BoxCollider>();
        Debug.Log("box: " + box);
        if (box != null)
        {
            Vector3 size = box.size;

            Debug.Log("-----"+ size);

        }
        **/

        if (override_modelScaleFactor != 0)
        {
            sx *= override_modelScaleFactor;
            sy *= override_modelScaleFactor;
            sz *= override_modelScaleFactor;
        }
        float pOffsetX = 0;

        if (override_effectedModel && (myData.sxEnabledA ^ myData.sxEnabledB)) // !!! XOR !!!
        {
            // scaling in only one direction
            float os = override_modelScaleFactor == 0 ? 1 : override_modelScaleFactor;

            // ?? BUG ABOUT SCALING ? here is the previous code
            /**
            // HACK:HACK: all objects should be a default size 1.1.1 but they actually aren't so we need to do this instead
            Debug.LogWarning("HACK:HACK: effect for scale is adding multiplier because objects aren't scaled at 1.1.1 by default");
            os *= 1/7f;
            **/

            pOffsetX = sx / 2f * 1f / os * (myData.sxEnabledA ? 1 : -1);
            
            ///was: (override_effectedModel ? override_effectedModel : this.gameObject).transform.localPosition = new Vector3(pOffsetX, 0, 0);

        }


        float pOffsetY = 0;

        if (override_effectedModel && (myData.syA ^ myData.syB)) // !!! XOR !!!
        {
            // scaling in only one direction
            float os = override_modelScaleFactor == 0 ? 1 : override_modelScaleFactor;

            // ?? BUG ABOUT SCALING ? here is the previous code
            /**
            // HACK:HACK: all objects should be a default size 1.1.1 but they actually aren't so we need to do this instead
            Debug.LogWarning("HACK:HACK: effect for scale is adding multiplier because objects aren't scaled at 1.1.1 by default");
            os *= 1 / 7f;
            ***/

            pOffsetY = sy / 2f * 1f / os * (myData.syA ? 1 : -1);

            ///was: (override_effectedModel ? override_effectedModel : this.gameObject).transform.localPosition = new Vector3(pOffsetX, 0, 0);

        }


        float pOffsetZ = 0;

        if (override_effectedModel && (myData.szA ^ myData.szB)) // !!! XOR !!!
        {
            // scaling in only one direction
            float os = override_modelScaleFactor == 0 ? 1 : override_modelScaleFactor;

            // ?? BUG ABOUT SCALING ? here is the previous code
            /**
            // HACK:HACK: all objects should be a default size 1.1.1 but they actually aren't so we need to do this instead
            Debug.LogWarning("HACK:HACK: effect for scale is adding multiplier because objects aren't scaled at 1.1.1 by default");
            os *= 1 / 7f;
            ***/

            pOffsetZ = sz / 2f * 1f / os * (myData.szA ? 1 : -1);

            

            ///was: (override_effectedModel ? override_effectedModel : this.gameObject).transform.localPosition = new Vector3(pOffsetX, 0, 0);

        }
        outLocalPosition = new Vector3(pOffsetX, pOffsetY, pOffsetZ);


        outLocalScale = new Vector3(myData.s_base.x * sx, myData.s_base.y * sy, myData.s_base.z * sz);
        ////was: (override_effectedModel ? override_effectedModel : this.gameObject).transform.localScale = new Vector3(myData.s_base.x * sx, myData.s_base.y * sy, myData.s_base.z * sz);
    }



    #endregion



}

