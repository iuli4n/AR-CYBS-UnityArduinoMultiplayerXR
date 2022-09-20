using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using System;

/// <summary>
/// One instance of the serialmanager exists at all times in the scene.
/// Routes messages from the Arduino to atomic data models
/// </summary>
public class SerialManager : MonoBehaviourPun
{
    public string[] channelsFromArduino;
    public string[] channelsToArduino;
    public ChannelsManager channelsManager;

    public SerialController sc;
    public bool isConnected;

    public float dampening = 0.1f;
    public bool roundToInt = true;

    private int usbConnectFailcount = 0;

    void Start()
    {
        sc = GameObject.Find("SerialController").GetComponent<SerialController>();

        foreach (string channel in channelsToArduino)
        {
            // bind listeners to the models. NOTE: this way doesn't allow a model to change channel at runtime
            AtomicDataModel m = channelsManager.GetModelForChannel(channel);
            //Debug.Log("SerialManager creating listener for channel model " + channel + " "+m);
            if (m != null)
            {
                m.OnDataUpdated += (newval) => { OnUnityModelChanged(channel, newval); };
            } else
            {
                Debug.LogError("SerialManager: Couldn't find channel: " + channel);
            }
        }
    }


    public void OnUnityModelChanged(string channel, float value)
    {
        if (!isConnected) { Debug.LogWarning("SerialManager dropped model update because is not connected."); return; }

        //Debug.Log("Unity model changed: " + channel + " " + value);

        string valueToSend = value.ToString();
        if (roundToInt) valueToSend = Mathf.RoundToInt(value).ToString();

        OnMessageSend(":" + channel + ":" + valueToSend);
    }


    // Invoked when a line of data is received from the serial device.
    public void OnMessageArrived(string message)
    {
        // TODO: Sometimes this receives a lot of messages concatenated together. This methods needs updating to fix that 

        if (message.Contains("ERROR"))
        {
            Debug.Log("Unity received serial message containing error: " + message);
        }

        bool ischannel = false;
        foreach (string channel in channelsFromArduino)
        {
            if (message.StartsWith(":" + channel + ":"))
            {
                ischannel = true;
                
                message = message.Remove(0, 2 + channel.Length);

                // BUG/HACK: this is needed because sometimes messages might get received concatenated together; just look at the first one
                string goodMessage = message;
                int nextstop = message.IndexOf(":");
                if (nextstop != -1)
                { 
                    goodMessage = message.Substring(0, nextstop);
                    Debug.LogError("Serial received concatenated messages: " + message);
                }
                
                float value = float.Parse(goodMessage);

                channelsManager.GetModelForChannel(channel).Value = (int)Mathf.Lerp(
                    value,
                    channelsManager.GetModelForChannel(channel).Value, 
                    dampening);

                //channelsManager.GetModelForChannel(channel).Value = value;

                break;
            }
        }

        if (!ischannel)
        {
            // Received something but not for an input channel; will be ignored
        }

    }

    
    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
        isConnected = success;

        if (!success)
        {
            usbConnectFailcount++;

            if (usbConnectFailcount < 3)
            {
                Debug.LogError("SerialManager failed to connect to serial port ");
            } else if (usbConnectFailcount == 3)
            {
                Debug.LogError("SerialManager giving up on reporting serial port errors.");
            }
        }
        //Debug.Log(success);
    }

    public void OnMessageSend(string message)
    {
        //Debug.Log("Unity model send: "+message);
        sc.SendSerialMessageWithEOL(message);
    }
}
