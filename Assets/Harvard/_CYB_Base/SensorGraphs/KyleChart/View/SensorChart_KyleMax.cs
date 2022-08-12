using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sensor Visualization that graphs the value of a sensor over a short period of time.
/// </summary>
public class SensorChart_KyleMax : SensorVisualization
{
    public static float HEIGHT = /*3.95f*/ 4f * 2;
    public static int MAXPOINTS = 50;

    // settings
    public Settings sett;
    private bool wasPlay = true;

    // data
    public Queue<DataFloat> qData;
    public float qMin;
    public float qMax;

    // renderers
    private LineRenderer lineRenderer;
    private LineRenderer lineRendererBack;
    public int numPoints;
    public Material material;

    private float updateTime = 0f;

    // associated text label on graph
    public TextMesh textLabel;

    void Start()
    {
        Initialize();

        sett.sensorVisualizations.Add(this);

        if (qData == null) qData = new Queue<DataFloat>();
        else numPoints = qData.Count;

        lineRenderer     = transform.GetChild(0).GetComponent<LineRenderer>();
        lineRendererBack = transform.GetChild(1).GetComponent<LineRenderer>();

        if (material != null)
        {
            lineRenderer.    material =
            lineRendererBack.material = material;
        }
        
        UpdateRenderer();

        sett.qHistory = new Queue<DataFloat>[5];

        // Update graph when Atomic Data Model is updated.
        // Not used currently. Left as a reference
        /*
        sett.sensorModels[sensorID - 1].OnDataUpdated += (newval) =>
        {
            BroadcastAndRecordSensorData(sensorID, new DataFloat(newval, Time.time));
        };
        */
    }

    void Update() {
        // Broadcasts Sensor Data at the same rate that the graph updates, even if the value is the same
        updateTime += Time.deltaTime;
        if (updateTime > sett.updateWait)
        {
            BroadcastAndRecordSensorData(sensorID, new DataFloat(sett.sensorModels[sensorID - 1].Value, Time.time));
            updateTime = 0f;
        }
    }

    // When the data changes, this tells each visualization to update and it holds a record of the sensor values
    public void BroadcastAndRecordSensorData(int _sensorID, DataFloat val)
    {
        //Debug.Log("Sensor updated: " + sensorID);

        if (sensorID == _sensorID)
            UpdateVisualization(val);

        //Debug.Log("qhist: " + sensorID);

        if (sett.qHistory[sensorID] == null)
            sett.qHistory[sensorID] = new Queue<DataFloat>();
        sett.qHistory[sensorID].Enqueue(val);

        // TODO: shouldn't be here, should be on a tick
        sett.DequeueOld(sett.qHistory[sensorID], sett.MAX_TIME);
    }

    /// <summary>
    /// Method to add a value to the queue and perform an update
    /// </summary>
    /// <param name="dNew">new data/time point</param>
    public override void UpdateVisualization(DataFloat dNew, string sensorName="")
    {
        //Debug.Log("SensorChart.UpdateVisualization called ! " +dNew.F+" "+dNew.T);
        if (sett.m.play)
        {
            if (!wasPlay)
                qData = sett.QueueHistory(sensorID);

            qData.Enqueue(dNew);
            sett.DequeueOld(qData, sett.m.xScale);

            UpdateLabel(Math.Round(dNew.F, 2).ToString());
        }
        wasPlay = sett.m.play;

        UpdateRenderer();
    }

    /// <summary>
    /// Method used for drawing the chart line
    /// </summary>
    /// <param name="index">index in queue</param>
    /// <param name="value">y-axis value</param>
    /// <param name="isFront">for the front renderer (vs. back)</param>
    /// <returns>the relative position of the point so the line falls inside the graph and follows the graph as it moves</returns>
    private Vector3 GetRelPosition(int index, float value, bool isFront)
    {
        float relX = (index / (float)(numPoints - 1)) * HEIGHT - HEIGHT / 2;
        return new Vector3(isFront ? relX : -relX, value - HEIGHT / 2, 0f);
    }

    /// <summary>
    /// Method to update the chart's associated label
    /// </summary>
    /// <param name="s">text</param>
    public void UpdateLabel(string s)
    {
        if (textLabel == null) return;

        // TASK 7
        textLabel.text = s;
    }

    /// <summary>
    /// Method to update the line renderers
    /// </summary>
    public void UpdateRenderer()
    {
        qMin = -1f;
        qMax = -1f;
        foreach (DataFloat d in qData)
        {
            if (qMin == -1f || d.F < qMin)
                qMin = d.F;
            if (qMax == -1f || d.F > qMax)
                qMax = d.F;
        }

        lineRenderer.positionCount     =
        lineRendererBack.positionCount =
        numPoints = (qData.Count <= 1) ? 2 : qData.Count;

        int i = 0; float val = 0f;
        foreach (DataFloat d in qData)
        {
            // TASK 8
            val = ScaleNumber(d.F, sett.YMin() - 0.01f, sett.YMax() + 0.01f, 0, HEIGHT);

            if (val < 0) val = -0.5f;
            if (val > HEIGHT) val = HEIGHT + 0.5f;

            lineRenderer.    SetPosition(i, GetRelPosition(i,   val, true));
            lineRendererBack.SetPosition(i, GetRelPosition(i++, val, false));
        }

        if (i == 1)
        {
            lineRenderer.    SetPosition(1, GetRelPosition(1, val, true));
            lineRendererBack.SetPosition(1, GetRelPosition(1, val, false));
        }
    }

    /// <summary>
    /// Method to get the n'th data point in the queue
    /// </summary>
    /// <param name="n">index in queue</param>
    /// <returns>n'th data point</returns>
    public DataFloat GetNth(int n)
    {
        if (n < 0 || n >= qData.Count)
        {
            Debug.LogWarning("SensorChart.GetNth ERROR: " + n + " of " + qData.Count);
            return new DataFloat(-1, 0);
        }
        return qData.ToArray()[n];
    }

    /// <summary>
    /// Method to change the line renderers' material
    /// </summary>
    /// <param name="m">material</param>
    public void ChangeMaterial(Material m)
    {
        this.material = m;

        if (lineRenderer     != null) lineRenderer.    material = m;
        if (lineRendererBack != null) lineRendererBack.material = m;
    }

    private void OnDestroy()
    {
        if (sett.sensorVisualizations.Contains(this))
            sett.sensorVisualizations.Remove  (this);
    }

}
