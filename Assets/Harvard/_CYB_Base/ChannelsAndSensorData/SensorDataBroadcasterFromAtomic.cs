using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is an adapter (connecting an Atomic Data Model data from multiple sensors, to multiple SensorVisualizations) 
public class SensorDataBroadcasterFromAtomic : MonoBehaviour
{
    // the sensor models that will be monitored. The index of these will indicate which sensor # they are for
    public AtomicDataSwitch[] sensorModels;

    // the visualizations that will be updated
    public List<SensorVisualization> sensorVisualizations = new List<SensorVisualization>(); // TODO: does this need to be multiple ?

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < sensorModels.Length; i++)
        {
            int icopy = i;
            sensorModels[i].OnDataUpdated += (newval) =>
            {
                BroadcastSensorData(icopy + 1, new DataFloat(newval, Time.time), sensorModels[icopy].CurrentChannel);
            };
        }
    }


    // Update is called once per frame
    void Update()
    {

    }


    // TODO: Attach this to each chart
    // When the data changes, this tells each visualization to update and it holds a record of the sensor values
    public void BroadcastSensorData(int sensorID, DataFloat val, string sensorLabel)
    {
        //Debug.Log("Sensor updated: " + sensorID);

        foreach (SensorVisualization vis in sensorVisualizations)
        {
            if (vis.sensorID == sensorID)
            {
                vis.UpdateVisualization(val, sensorLabel);
            }
        }

    }


}
