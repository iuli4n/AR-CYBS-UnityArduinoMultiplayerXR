using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is an adapter (connecting an Atomic Data Model data from 3 sensors to SensorVisualizations) and a recorder (keeps a history of all the recent changes to the model)
public class TEMP_SensorDataHistoryRecorder : MonoBehaviour
{
    // time interval of saved data (and maximum slider time)
    public float MAX_TIME = 4f;


    // an array of queues, one per sensor; each queue contains a list of received datapoints that contain value&timestamp
    private Queue<DataFloat>[] qHistory;

    // the sensor models that will be monitored
    public AtomicDataModel[] sensorModels;

    // the visualizations that will be updated
    public List<SensorVisualization> sensorVisualizations = new List<SensorVisualization>(); // TODO: does this need to be multiple ?

    // Start is called before the first frame update
    void Start()
    {
        qHistory = new Queue<DataFloat>[5];

        for (int i = 0; i < sensorModels.Length; i++)
        {
            //Debug.Log("Adding listener for model " + i);
            int icopy = i;
            sensorModels[i].OnDataUpdated += (newval) =>
            {
                BroadcastAndRecordSensorData(icopy+1, new DataFloat(newval, Time.time));
            };
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    // TODO: Attach this to each chart
    // When the data changes, this tells each visualization to update and it holds a record of the sensor values
    public void BroadcastAndRecordSensorData(int sensorID, DataFloat val)
    {
        //Debug.Log("Sensor updated: " + sensorID);

        foreach (SensorVisualization vis in sensorVisualizations)
        {
            if (vis.sensorID == sensorID)
                vis.UpdateVisualization(val);
        }

        //Debug.Log("qhist: " + sensorID);

        if (qHistory[sensorID] == null)
            qHistory[sensorID] = new Queue<DataFloat>();
        qHistory[sensorID].Enqueue(val);

        // TODO: shouldn't be here, should be on a tick
        DequeueOld(qHistory[sensorID], MAX_TIME);
    }

    // TODO performance: does this get called often?
    // this makes a new queue every time, should just give the real queue and assume readonly
    public Queue<DataFloat> QueueHistory(int sensorID)
    {
        Queue<DataFloat> qNew = new Queue<DataFloat>();

        if (qHistory[sensorID] != null)
        {
            foreach (DataFloat d in qHistory[sensorID])
                qNew.Enqueue(d);
        }

        return qNew;
    }

    public void DequeueOld(Queue<DataFloat> q, float time)
    {
        DataFloat recent = new DataFloat(0f, Time.time);
        foreach (DataFloat d in q) recent = d; // TODO performance: this is basically recent = q.last

        while (q.Count != 0 && recent.T - q.Peek().T > time) q.Dequeue();
    }

}
