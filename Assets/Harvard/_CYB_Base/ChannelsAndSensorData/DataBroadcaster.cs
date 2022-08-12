using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Broadcasts data to all sensor visualizations in scene.
/// </summary>
public class DataBroadcaster : MonoBehaviour
{
    // time interval of saved data (and maximum slider time)
    public static float MAX_TIME = 4f;

    // TODO: move to a centralized place
    ////public static List<SensorVisualization> visualizations = new List<SensorVisualization>();

    // network: this is a local data queue but matches all in network
    private static Queue<DataFloat>[] qHistory = new Queue<DataFloat>[10];

    public ElectronicsDataPipeSender dataSender;

    public enum DataType
    {
        Spiky, Sine, Square, Increasing
    }

    public int sensorID;
    public DataType dataType;

    public float wavePeriod;
    public float waveMin;
    public float waveMax;

    private float fake_currentData = 0;
    private float fake_currentMax = 1f;

    private float timeOriginal;
    private float timeStamp;

    public AtomicDataSwitch speedSwitch;

    void Start()
    {
        timeOriginal = Time.time;
        timeStamp = timeOriginal;
    }

    void Update()
    {
        switch (dataType)
        {
            case (DataType.Spiky):
                fake_currentData = fake_currentMax / (wavePeriod * 20f) + fake_currentData;
                if (fake_currentData >= fake_currentMax)
                {
                    fake_currentData = fake_currentMax - 1f;
                    fake_currentMax += 0.5f;

                    if (fake_currentMax > waveMax)
                        fake_currentMax = waveMin + 1;
                }
                break;
            case (DataType.Sine):
                fake_currentData = ((float)Math.Sin(2 * Math.PI * (Time.time - timeOriginal) / wavePeriod) + 1) / 2 * (waveMax - waveMin) + waveMin;
                break;
            case (DataType.Square):
                fake_currentData = ((int)(2 * (Time.time - timeOriginal) / wavePeriod) % 2 == 0) ? waveMin : waveMax;
                break;
            case (DataType.Increasing):

                //Debug.Log(speedSwitch.Value);
                wavePeriod = 200f / (speedSwitch.Value + 1); //map this from [0 slow, 1024 fast] to [high period, low period]
                fake_currentData = ((Time.time - timeStamp) / wavePeriod) * (waveMax - waveMin) + waveMin;
                if (fake_currentData >= waveMax) { 
                    fake_currentData = waveMin;
                    timeStamp = Time.time;
                }
                break;
            default:
                fake_currentData = 0f;
                break;
        }

        dataSender.SetCurrentSensorValue(sensorID, fake_currentData);
        //BroadcastSensorData(sensorID, new DataFloat(fake_currentData, Time.time));
    }

    
}
