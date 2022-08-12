using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ElectronicsDataPipeSender : MonoBehaviour
{
    //public SensorDataBufferedListModel model;

    public AtomicDataModel sensorModel1;
    public AtomicDataModel sensorModel2;
    public AtomicDataModel sensorModel3;
    public AtomicDataModel sensorModel4;


    public float broadcastInterval = 0.1f;
    public float aggregateInterval = 0.05f;

    public float timeToAggregate = 0;

    private float[] currentSensorValues = new float[5];

    public bool isDirty = false;


    // Start is called before the first frame update
    void Start()
    {
        //model.syncInterval = aggregateInterval;
    }

    public void SetCurrentSensorValue(int sensorID, float sensorValue)
    {
        if (sensorID > 4) { Debug.LogError("ElectronicDataPipeSender can't support more than 4 sensors right now, discarding sensor."); return; } // tracking only a few sensors for now

        currentSensorValues[sensorID - 1] = sensorValue;
        isDirty = true;
    }

    void Update()
    {
        if (!isDirty || timeToAggregate >= Time.time)
            return;
        timeToAggregate = Time.time + aggregateInterval;

        if (!(sensorModel1 || sensorModel2 || sensorModel3 || sensorModel4))
        {
            // nothing to do here
            return;
        }

        if (!PlayersManager.Instance.IsPlayerReady())
        {
            Debug.LogWarning("Sensor data is generated but photon is not ready, so ignoring this update...");
            return;
        }

        if (sensorModel1) sensorModel1.Value = currentSensorValues[0];
        if (sensorModel2) sensorModel2.Value = currentSensorValues[1];
        if (sensorModel3) sensorModel3.Value = currentSensorValues[2];
        if (sensorModel4) sensorModel4.Value = currentSensorValues[3];


        //Tuple<float, float[]> e = new Tuple<float, float[]>(Time.time, (float[])currentSensorValues.Clone());
        //model.myList.Add(e);

        isDirty = false;

        // broadcast happens separately through the model itself
    }

}
