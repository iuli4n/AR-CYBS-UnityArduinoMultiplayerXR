using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeSensorDataBroadcaster : MonoBehaviour {

    // visualizations to update
    public SensorVisualization[] visualizations;

    // values used for faking the data
    public float fake_currentData = 0;
    public float fake_currentMax = 1f;

    public float fakeDataMultiplier = 1f;

    void Start () {
		
	}
	
	void Update () {
        // generate fake data
        fake_currentMax = 1f * fakeDataMultiplier;
        fake_currentData = fakeDataMultiplier*( fake_currentMax / 25f + fake_currentData+0.001f);
        if (fake_currentData > fake_currentMax)
        {
            fake_currentData = fake_currentMax-1f;
            fake_currentMax += 0.5f;
            if (fake_currentMax > 7)
            {
                fake_currentMax = 1;
            }
        }

        BroadcastSensorData(fake_currentData);

    }

    void BroadcastSensorData(float val)
    {
        foreach (SensorVisualization vis in visualizations)
        {
            vis.UpdateVisualization(new DataFloat(val, Time.time));
        }
    }
}
