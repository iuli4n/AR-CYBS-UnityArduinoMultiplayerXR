using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sensor visualization that graphs the value of a sensor over a short period of time.
/// </summary>
public class SensorChart : SensorVisualization {

    // not used
    //public GameObject thresholdLine;
    //public GameObject thresholdLabel;

    // drawer for the line
    private LineRenderer lineRenderer;


    // data array and configuration
    public const int MAXPOINTS = 50;
    private float currentValue;
    private Queue<float> linkedDataPoints = new Queue<float>();

    // label for displaying the current value
    public TextMesh textLabel_sensorValue;
    // label for displaying the current name of the sensor
    public TextMesh textLabel_sensorName;

    // not implemented
    public bool autoScale = false;

    void Start()
    {
        Initialize();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = MAXPOINTS;
        currentValue = 500;
    }

    private void OnEnable()
    {
        StartCoroutine(UpdatePassively());
    }

    void Update()
    {
    }

    float lastValue = -1;
    IEnumerator UpdatePassively()
    {
        while (true) {
            while (this.isActiveAndEnabled) {
                yield return new WaitForSeconds(0.1f);
                AddDataPoint(currentValue);
                UpdateLabel();
            }
            yield return new WaitForSeconds(1f);
        }
    }


    public override void UpdateVisualization(DataFloat value, string newLabel="")
    {
        //Debug.Log("Visualization updated: " + value.F);
        textLabel_sensorName.text = newLabel;
        currentValue = value.F;
        
    }
    
    private void UpdateLabel()
    {
        textLabel_sensorValue.text = "" + currentValue;
    }

    /// <summary>
    /// Adds the specified value into the queue of y-values.
    /// </summary>
    private void AddDataPoint(float x)
    {
        if (linkedDataPoints.Count >= MAXPOINTS)
        {
            linkedDataPoints.Dequeue();
        }
        linkedDataPoints.Enqueue(x);

        RenderPoints();
    }

    /// <summary>
    /// Updates the line renderer to draw all the data points
    /// </summary>
    void RenderPoints()
    {
        int i = 0;
        foreach (float x in linkedDataPoints)
        {
            lineRenderer.SetPosition(i++, GetRelPosition(i, x));
        }
    }

    /// <summary>
    /// Used for drawing the chart line.
    /// Returns the relative position of the point so the line falls inside the graph and follows the graph as it moves
    /// </summary>
    Vector3 GetRelPosition(int index, float value)
    {
        float relX = (index / (float)MAXPOINTS) * (3.95f * 2) - 3.95f;
        return new Vector3(relX, (value/150f)-3.95f, -0.045f);
    }
    
    private void SetThreshold(object sender, EventArgs e)
    {
        /**
        thresholdLine.transform.localPosition = new Vector3(0f, ScaleDatapoint(currentValue, -RANGE, RANGE), 0f);
        thresholdLabel.transform.localPosition = new Vector3(thresholdLabel.transform.localPosition.x, ScaleDatapoint(currentValue, -RANGE, RANGE), 0f);
        thresholdLabel.GetComponent<TextMesh>().text = currentValue.ToString();
        **/
    }
}
