/// Parts of this code are taken from: http://answers.unity3d.com/questions/1092655/serial-port-on-unity-1.html


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using System.IO.Ports;

public class SerialCommunicator : MonoBehaviour
{

    public delegate void SerialGotNewData(string newData);
    public SerialGotNewData OnGetNewData;

    private SerialPort mySerial;


    public string portName = "COM7";
    public int portSpeed = 57600;

    // Use this for initialization
    void Start()
    {

    }

    public void OnApplicationQuit()
    {
        if (mySerial != null && mySerial.IsOpen)
        {
            mySerial.Close();
            mySerial = null;
        }
    }

    public void Open()
    {
        mySerial = new SerialPort(portName, portSpeed);
        mySerial.Open();

        // cler input buffer from previous garbage
        DiscardInputBuffer();
    }

    public void DiscardInputBuffer()
    {
        byte[] buf = new byte[10000];
        mySerial.DiscardInBuffer();
        //mySerial.ReadExisting();
        mySerial.Read(buf, 0, 10000);
    }

    private void Update()
    {
        if (mySerial != null && mySerial.IsOpen)
        {
            ///ISSUES: string allread = s_serial.ReadExisting();

            //mySerial.ReadTimeout = 1;
            string allread = "";
            try
            {
                bool stillgo = true;
                while (stillgo)
                {
                    char c = (char)mySerial.ReadByte();
                    //if (char.IsControl(c)) c = '/';

                    if (c == '\n')
                    {
                        stillgo = false;
                    }
                    allread += c;
                }
            }
            catch (Exception e)
            {
                // done !
                // todo: catch timetout exception because that's what we care about
            }

            if (allread.Length > 0)
            {
                //Debug.Log("allread: " + allread);
                OnGetNewData(allread);
            }
            else
            {
                Debug.Log("allread =0");
            }

        }
    }
    

    static string[] GetPortNames()
    {

        return System.IO.Ports.SerialPort.GetPortNames();

    }

}

#else

// Serial stuff is disabled when deploying to Hololens

public class SerialCommunicator : MonoBehaviour {

    public delegate void SerialGotNewData(string newData);
    public SerialGotNewData OnGetNewData;

    public void Open() {
    }
}
#endif