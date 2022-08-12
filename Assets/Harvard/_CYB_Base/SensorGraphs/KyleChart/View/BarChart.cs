using UnityEngine;

/// <summary>
/// Sensor Visualization that graphs the value of a sensor at the current moment.
/// </summary>
public class BarChart : SensorVisualization
{
    public GameObject barFill;                     // object which is scaled
    public GameObject barColorFront, barColorBack; // this is the object which actually is colored

    // settings
    public Settings sett;
    
    void Start()
    {
        sensorID = GetComponent<DataBroadcaster>().sensorID;

        barColorBack. GetComponent<MeshRenderer>().material =
        barColorFront.GetComponent<MeshRenderer>().material = sett.sensorColors[sensorID];
    }
    
    void Update() {}

    public override void UpdateVisualization(DataFloat value, string newLabel)
    {
        SetScale(value.F);
        return;
    }

    /// <summary>
    /// Scales the bar to match the value (0 - 1024)
    /// </summary>
    /// <param name="val"></param>
    public void SetScale(float val)
    {
		Vector3 scl = barFill.transform.localScale;
		scl.z = val/1024f*128f;
		barFill.transform.localScale = scl;
	}
}
