using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGUI_Electronics : MonoBehaviour
{
    public FakeSensorDataBroadcaster sensorDataBroadcaster;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        if (Input.GetKey(KeyCode.Alpha7))
        {
            sensorDataBroadcaster.fakeDataMultiplier = 5f;
        }
        if (Input.GetKey(KeyCode.Alpha8))
        {
            sensorDataBroadcaster.fakeDataMultiplier = 1.3f;
        }
        if (Input.GetKey(KeyCode.Alpha9))
        {
            sensorDataBroadcaster.fakeDataMultiplier = 0f;
        }
    }
}
