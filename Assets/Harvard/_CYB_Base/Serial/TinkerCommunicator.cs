using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using WebSocketSharp;
#endif

/// <summary>
/// This is a websocket client that talks to a Gogo board from TinkerInventions, usually running on the local computer.
/// You could modify this to work with other WebSocket data sources.
/// PS: This code for Gogo board actually is made for 2 types of gogo boards, that's why there's functions labeled as _OLD
/// </summary>
public class TinkerCommunicator : MonoBehaviour
{

#if UNITY_EDITOR


    public AtomicDataModel sensorModel1;
    public AtomicDataModel sensorModel2;
    public AtomicDataModel sensorModel3;

    public string ServerIP = "127.0.0.1";
    public string ServerWSPath = "/ws";
    private WebSocket websocket;
    private string rawData;
    
    public float dampening = 0.1f;

    //private GogoDataReader gogoDataReader;


    void Start()
    {

        Debug.Log(Time.time + " TempTinkerCommunicator: About to connect");

        websocket = new WebSocket("ws://" + ServerIP + ServerWSPath); //+ ":8765");
        websocket.OnMessage += OnMessageReceived; //(sender, e) => rawData = e.Data;
        websocket.OnError += OnError;
        websocket.OnClose += OnClose;

        websocket.Connect();
        websocket.Send("SUBSCRIBE:HOLO");
        websocket.Send("SUBSCRIBE:HOLO");

        isSupposedToBeRunning = true;

        Debug.Log(Time.time + " TempTinkerCommunicator: done");

        // if you want to keep sending data on the port
        // InvokeRepeating("SendProcessedData", 0f, 0.0333f);
    }


    bool isSupposedToBeRunning = false;

    private void OnDestroy()
    {
        isSupposedToBeRunning = false;
        if (websocket.IsAlive)
        {
            websocket.Close();
        }
    }

    void OnError(object sender, WebSocketSharp.ErrorEventArgs e)
    {
        Debug.LogError("WS ERROR: " + e.Message);
        if (!isSupposedToBeRunning)
            return;

        if (websocket.IsAlive)
        {
            websocket.Close();
        }
        websocket.Connect();

    }
    void OnClose(object sender, WebSocketSharp.CloseEventArgs e)
    {
        Debug.LogError("WS OTHER SIDE CLOSED: " + e.Reason);
        if (!isSupposedToBeRunning)
            return;

        if (websocket.IsAlive)
        {
            websocket.Close();
        }
        websocket.Connect();
    }

    public bool previousNeedsProcessing = false;

    public void OnMessageReceived_OLD(object sender, MessageEventArgs e)
    {
        //if (!rawData.Contains("burst"))
        //Debug.LogWarning("FROM TINKER RECEIVED: " + rawData);


        //Debug.LogWarning("TINKER RECEIVED: ");
        rawData = e.Data;


        if (previousNeedsProcessing) return;

        previousNeedsProcessing = true;

        //websocket.WaitTime = new System.TimeSpan(50) ;
    }

    public void OnMessageReceived(object sender, MessageEventArgs e)
    {
        Debug.LogWarning("FROM TINKER RECEIVED: " + e.Data);
        rawData = e.Data;

        if (previousNeedsProcessing) return;
        previousNeedsProcessing = true;

    }



    private void ProcessAndSendToChannel_OLD()
    {
        if (!previousNeedsProcessing) return;

        if (rawData == null)
        {
            previousNeedsProcessing = false;
            return;
        }

        string startstring = "burst::";
        char sep = '-';

        if (rawData.StartsWith(startstring))
        {
            if (rawData.Length < startstring.Length)
            {
                previousNeedsProcessing = false;
                return;
            }

            string data = rawData.Substring(startstring.Length);
            string[] datapoints = data.Split(new char[] { sep });
            short[] toSend = new short[8];
            for (int i = 0; i < datapoints.Length; i++)
                toSend[i] = short.Parse(datapoints[i]);


            // SEND TO CHANNELS

            sensorModel1_Value = toSend[0];
            sensorModel2_Value = toSend[1];
            sensorModel3_Value = toSend[2];

        }

        previousNeedsProcessing = false;
        return;


    }

    private int valueFromBytestrings(string bs1, string bs2)
    {
        short b1 = short.Parse(bs1);
        short b2 = short.Parse(bs2);

        return b1 * 255 + b2;
    }

    private void ProcessAndSendToChannel_GOGO2()
    {
        if (!previousNeedsProcessing) return;

        if (rawData == null)
        {
            previousNeedsProcessing = false;
            return;
        }


        string startstring = "{\"stream\":[0,";
        char sep = ',';

        if (!rawData.StartsWith(startstring))
        {
            Debug.LogWarning("GOGO IGNORING message because doesn't start properly. MSG:"+rawData);
            return;
        }
        else
        {
            string data = rawData.Substring(startstring.Length);
            
            string[] datapoints = data.Split(new char[] { sep });

            int[] toSend = new int[3];

            for (int i = 0; i < toSend.Length; i++)
            {
                toSend[i] = valueFromBytestrings(datapoints[i * 2 + 0], datapoints[i * 2 + 1]);
            }


            // SEND TO CHANNELS

            sensorModel1_Value = toSend[0];
            sensorModel2_Value = toSend[1];
            sensorModel3_Value = toSend[2];
            
        }
        

        previousNeedsProcessing = false;
        return;
    

    } 


    int sensorModel1_Value;
    int sensorModel2_Value;
    int sensorModel3_Value;

    // Update is called once per frame
    void Update()
    {
        if (previousNeedsProcessing) 
            ProcessAndSendToChannel_GOGO2();

        //Debug.Log("C1: " + sensorModel1.Value);
        if (sensorModel1) sensorModel1.Value = (int)Mathf.Lerp(sensorModel1.Value, sensorModel1_Value, dampening);
        if (sensorModel2) sensorModel2.Value = (int)Mathf.Lerp(sensorModel2.Value, sensorModel2_Value, dampening);
        if (sensorModel3) sensorModel3.Value = (int)Mathf.Lerp(sensorModel3.Value, sensorModel3_Value, dampening);
    }

#endif
}
