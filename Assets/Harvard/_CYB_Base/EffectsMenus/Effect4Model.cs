using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect4Model : MonoBehaviourPun, IPunObservable
{

    //EffectsMenu effectsMenu;

    public List<AEffect4PluginModel> pluginsModels = new List<AEffect4PluginModel>();

    // flags to enable/disable effects

    public bool vis_enabled; // visualization
    public bool rx_enabled;  // rotation x
    public bool ry_enabled;  // rotation y
    public bool rz_enabled;  // rotation z
    
    public bool sxEnabledA, sxEnabledB; // scale x left/right
    public bool syEnabledA, syEnabledB; // scale y up/down
    public bool szEnabledA, szEnabledB; // scale z in/out


    // Visual active effects config

    public float vis_thresh;
    public bool vis_visibleBelow = false;
    public bool vis_visibleAbove = true;

    // Rotation effects config

    public Vector3 r_base; // this typically holds the object's original rotation
    public float rx_mult = 1;
    public float ry_mult = 1;
    public float rz_mult = 1;


    // Scale effects

    public Vector3 s_base; // this typically ohlds the object's original scale
    public float sx_mult = 1;
    public float sy_mult = 1;
    public float sz_mult = 1;
    
    /*
    void Start()
    {
        effectsMenu = GameObject.Find("EffectsMenuManager").GetComponent<EffectsMenu>();
        Debug.Log("HERE is the effects menu");
        Debug.Log(effectsMenu);
    }
    */

    /**
    public void LoadFromPrefabModel(Effect4Model otherModel)
    {
        PhotonStream stream = new PhotonStream(true, null);
        List<object> streamData = new List<object>();
        //stream  SetWriteStream(streamData);

        // put all the other data into this stream
        otherModel.OnPhotonSerializeView(stream, new PhotonMessageInfo());

        // read the data from the stream
        this.OnPhotonSerializeView(stream, new PhotonMessageInfo());
    }



    void PMRW(PhotonStream stream, object writeData, Action<object> readAction)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(writeData);
        }
        else
        {
            var obj = stream.ReceiveNext();
            readAction(obj);
        }
    }

    ***/

    #region IPunObservable implementation

    // Photon will call this to synchronize the model data
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        


        // TODO:PERF: THIS USES LOTS OF RESOURCES, USE A FLAG FOR CHANGES !

        // Note, this is called both when reading or writing


        if (stream.IsWriting)
        {
            //if (needsNetworkSync)
            {
                
                stream.SendNext(vis_enabled);
                stream.SendNext(vis_thresh);
                stream.SendNext(vis_visibleAbove);
                stream.SendNext(vis_visibleBelow);
                // switch ? stream.SendNext(vis_model);

                stream.SendNext(r_base);
                stream.SendNext(rx_enabled);
                stream.SendNext(rx_mult);
                stream.SendNext(ry_enabled);
                stream.SendNext(ry_mult);
                stream.SendNext(rz_enabled);
                stream.SendNext(rz_mult);

                stream.SendNext(s_base);
                stream.SendNext(sxEnabledA);
                stream.SendNext(sxEnabledB);
                stream.SendNext(sx_mult);
                stream.SendNext(syEnabledA);
                stream.SendNext(syEnabledB);
                stream.SendNext(sy_mult);
                stream.SendNext(szEnabledA);
                stream.SendNext(szEnabledB);
                stream.SendNext(sz_mult);

                //needsNetworkSync = false;
            }

            foreach (var pluginModel in pluginsModels)
            {
                pluginModel.DoPhotonSerialize(true, stream);
            }
        }
        else
        {
            /***
             * TODOIRNOW: Continue. Also document possible performance issue with using this
            PMW(stream, vis_enabled, (x) => vis_enabled = (bool)x) ;
            PMW(stream, vis_thresh, (x) => vis_thresh = (float)x);
            PMW(stream, vis_visibleAbove, (x) => vis_visibleAbove = (bool)x);
            PMW(stream, vis_visibleAbove, (x) => vis_visibleAbove = (bool)x);
            
            PMW(stream, r_base, (x) => r_base = (Vector3)x);
            
            PMW(stream, rx_enabled, (x) => rx_enabled = (bool)x);
            PMW(stream, rx_mult, (x) => rx_mult = (float)x);
            ***/


            this.vis_enabled = (bool)stream.ReceiveNext();
            this.vis_thresh = (float)stream.ReceiveNext();
            this.vis_visibleAbove = (bool)stream.ReceiveNext();
            this.vis_visibleBelow = (bool)stream.ReceiveNext();
            // switch ? stream.SendNext(vis_model);

            this.r_base = (Vector3)stream.ReceiveNext();
            this.rx_enabled = (bool)stream.ReceiveNext();
            this.rx_mult = (float)stream.ReceiveNext();
            this.ry_enabled = (bool)stream.ReceiveNext();
            this.ry_mult = (float)stream.ReceiveNext();
            this.rz_enabled = (bool)stream.ReceiveNext();
            this.rz_mult = (float)stream.ReceiveNext();

            this.s_base = (Vector3)stream.ReceiveNext();
            this.sxEnabledA = (bool)stream.ReceiveNext();
            this.sxEnabledB = (bool)stream.ReceiveNext();
            this.sx_mult = (float)stream.ReceiveNext();
            this.syEnabledA = (bool)stream.ReceiveNext();
            this.syEnabledB = (bool)stream.ReceiveNext();
            this.sy_mult = (float)stream.ReceiveNext();
            this.szEnabledA = (bool)stream.ReceiveNext();
            this.szEnabledB = (bool)stream.ReceiveNext();
            this.sz_mult = (float)stream.ReceiveNext();

            /*
            Debug.Log("GOT NEW MODEL PARAMS");
            effectsMenu.RefreshMenuWhenTargetModelChanges(transform);
            */

            //needsArduinoSync = true;

            //OnDataUpdated?.Invoke(_value);

            foreach (var pluginModel in pluginsModels)
            {
                pluginModel.DoPhotonSerialize(false, stream);
            }
        }

    }

    #endregion //NetworkedModel


}