using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EP_HistoricalDelay_Manager : AEffect4PluginManager
{
    [SerializeField]
    private EP_HistoricalDelay_Menu myMenu;
    [SerializeField] 
    private EP_HistoricalDelay_Model myModel;

    bool menuGUIOn;
    bool savedMenuGUIOn;

    public override AEffect4PluginModel GetModel() { return myModel; }
    public override AEffect4PluginMenu GetMenu() { return myMenu; }



    public override void OnCreated()
    {
        
    }
    public override void OnEnable()
    {
        // we'' record history for 5 seconds max, sampled every 50ms
        InitializeHistorical(5 * 1000f, 50f);
    }
    public override void OnDisable()
    {
        // nothing to do
    }

    public override void ToggleMenuGUI()
    {
        menuGUIOn = !menuGUIOn;
    }

    public override string GetPluginName()
    {
        return "Channel Delay";
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (savedMenuGUIOn != menuGUIOn)
        {
            savedMenuGUIOn = menuGUIOn;
            if (menuGUIOn)
            {
                myMenu.Activate();
                myMenu.OpenMenu(this.transform);
            }
            else { myMenu.HideMenu(); }
        }
    }



    float lastSeenValue = -1;
    
    public override void RespondToNewValue(
        float currentSensorValue, float oldSensorValue,
        Vector3 baseLocalPos, Quaternion baseLocalRot, Vector3 baseLocalScale,
        ref float newSensorValue,
        ref Vector3 newLocalPos, ref Quaternion newLocalRot, ref Vector3 newLocalScale)
    {
        
        if (!myModel.isEnabled)
            return;

        // record the current value in our history buffer (indirectly via FixedUpdate)
        lastSeenValue = currentSensorValue;

        // return a previously recorded value
        newSensorValue = GetHistoricalValue(myModel.delayMillis);
    }



    #region HistoricalRecords

    protected bool HIST_isSampling = false;
    private int HIST_NUMSAMPLES = 1; // how many samples we will store
    private float[] HIST_historicalSamples; // record of all the samples

    [SerializeField] 
    private int HIST_circularCurrentIndex = 0;
    [SerializeField] 
    private float HIST_forcedDelayBetweenSamples = 10f; // in milliseconds
    [SerializeField] 
    private float HIST_totalDelayBetweenSamples;
    
    private float HIST_nextSampleTime = 0;

    protected void InitializeHistorical(float maxDurationMils, float forcedDelayBetweenSamplesMils)
    {
        // assumes Time.deltaTime doesn't change

        HIST_forcedDelayBetweenSamples = forcedDelayBetweenSamplesMils;
        HIST_totalDelayBetweenSamples = HIST_forcedDelayBetweenSamples + Time.deltaTime;

        HIST_NUMSAMPLES = (int)(maxDurationMils / HIST_totalDelayBetweenSamples);

        HIST_historicalSamples = new float[HIST_NUMSAMPLES];
        HIST_isSampling = true;
        //HIST_nextSampleTime = 0;
    }

    public virtual float GetHistoricalValue(float millisecondsAgo)
    {
        if (!HIST_isSampling)
        {
            Debug.LogWarning("HistoricalDelay: Getting historical value of this type of model is not supported.");
            return lastSeenValue;
        }

        int i = (int)(millisecondsAgo / HIST_totalDelayBetweenSamples);
        if (i >= HIST_NUMSAMPLES)
        {
            // problem:
            Debug.LogWarning("HistoricalDelay: Trying to get a historical value beyond our recording range.");
            return lastSeenValue;
        }

        return HIST_historicalSamples[(HIST_circularCurrentIndex + i) % HIST_NUMSAMPLES];
    }

    private void RecordCurrentValueInHistory(float val)
    {
        HIST_circularCurrentIndex = (HIST_circularCurrentIndex - 1) % HIST_NUMSAMPLES;
        if (HIST_circularCurrentIndex < 0) HIST_circularCurrentIndex += HIST_NUMSAMPLES;
        //Debug.Log("Recording at " + HIST_circularCurrentIndex);
        HIST_historicalSamples[HIST_circularCurrentIndex] = val;
    }

    void FixedUpdate()
    {
        //Debug.Log("HD: next update: "+HIST_nextSampleTime);
        HIST_isSampling = myModel.isEnabled;

        if (!HIST_isSampling)
            return;

        if (Time.time < HIST_nextSampleTime)
            return;

        if (HIST_NUMSAMPLES <= 1)
        {
            // we're not initialized yet
            return;
        }

        HIST_nextSampleTime = Time.time + HIST_forcedDelayBetweenSamples / 1000f;

        // record a sample right now
        RecordCurrentValueInHistory(lastSeenValue);
    }

    #endregion
}
