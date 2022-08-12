using UnityEngine;

/// <summary>
/// Parent class for all of the AR sensor visualizations;
/// Provides public method for updating the visualization.
/// </summary>
public abstract class SensorVisualization : MonoBehaviour
{
    /// <summary>
    /// The numerical ID of the data port that this visualization is tied to (1-indexed)
    /// </summary>
    public int sensorID;

    /// <summary>
    /// Method called whenever the data source sends this visualization a new sensor data point;
    /// Each visualization class must implement its own UpdateVisualization method to fit the style of visualization
    /// </summary>
    public abstract void UpdateVisualization(DataFloat value, string sensorLabel="");

    void Start() { Initialize(); }
    protected void Initialize() { }

    /// <summary>
    /// Takes 'number' from range1 and scales it to range2
    /// </summary>
    static protected float ScaleNumber(float number, float range1Min, float range1Max, float range2Min, float range2Max)
    {
        float range1 = range1Max - range1Min;
        float range2 = range2Max - range2Min;
        float percent = (number - range1Min) / range1;
        return /*Mathf.Clamp(*/(range2 * percent) + range2Min/*, range2Min, range2Max)*/;
    }
}
