using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Controller of a graph's settings metadata.
/// </summary>
public class Settings : MonoBehaviour, IPunObservable
{
    //public TEMP_SensorDataHistoryRecorder sensorHistories;
    public List<SensorVisualization> sensorVisualizations = new List<SensorVisualization>();
    // time interval of saved data (and maximum slider time)
    public float MAX_TIME = 4f;
    // the sensor models that will be monitored
    public AtomicDataModel[] sensorModels;
    // update delay
    public float  updateWait = 0.1f;
    private float updateNext;

    // an array of queues, one per sensor; each queue contains a list of received datapoints that contain value&timestamp
    public Queue<DataFloat>[] qHistory;

    // platform settings
    public bool hololens;
    public bool vuforia;
    public Camera arCamera;
    
    // views for each sensor and overarching model
    public SensorChart_KyleMax[] charts;
    public SettingsModel m;
    
    // pause/play
    public Interactable pausePlayButton;
    private Queue<DataFloat>[] pauseQueues;

    // auto-scale
    public Interactable autoScaleToggle;

    // x scale
    public PinchSlider xSlider;
    public TextMesh xCurrent;
    public TextMesh xMinText;
    public TextMesh xMaxText;

    // y scale
    public Interactable yMidDec;
    public Interactable yMidInc;
    public Interactable yScaleDec;
    public Interactable yScaleInc;
    public TextMesh[] yTicks;
    public float yDelta;
    public TextMesh yMidText;
    public TextMesh yScaleText;

    // settings presets
    public Interactable settSaveButton;
    public Interactable[] settSaves;
    public Interactable settDefault;

    // duplication (static and dynamic)
    public Interactable staticButton;
    public TextMesh     staticTimestampText;
    public Interactable dynamicButton;
    protected bool isSnapshot = false;
    protected bool isStatic   = false;
    public Vector3 snapshotNextLerp;
    public  int lerpSteps;
    private int lerpCurrent;

    // deletion
    public Interactable deleteButton;
    public GameObject   deleteWarning;
    public Interactable deleteNo;
    public Interactable deleteYes;
    protected bool deleting = false;

    // focus areas
    protected bool focus           = false;
    protected bool focusedSettings = false;
    protected bool focusedChannels = false;

    // main control buttons
    public GameObject focusButtons;
    public float focusAngle;
    public float focusDistance;

    // configuration UI
    public GameObject uiGameObject;
    public Interactable configureButton;

    // finger tap support (right hand)
    public BoxCollider chartCollider;
    private bool wasTapping;
    public float fingerDistance;

    // sensor toggles
    public GameObject channelButtons;
    public Interactable[] sensorToggles;
    public Material[] sensorColors;
    public Material[] sensorColorsText;
    public TextMesh[] sensorLabels;

    // manipulation and merging
    public ObjectManipulatorIuli manipulator;
    public Interactable[] sensorGrabs;
    public float mergeDistance;
    public GameObject mergeBox;
    private bool wasJustMoving;
    private Settings mergeSett;

    // graph plane
    public Transform[] planeCorners;
    public Transform planeObject;
    
    // two cursors (on pause) + locking
    private bool cursorsEnabled = false;
    private float lastCursorX;
    public Transform cursorLine1;
    public Transform cursorLine2;
    public Transform[] cursorSpheres1;
    public Transform[] cursorSpheres2;
    private bool[] cursorSpheresActive1;
    private bool[] cursorSpheresActive2;
    public TextMesh[] cursorText2;
    public TextMesh valuesLabel;
    private bool cursorLock1Press = false;
    private bool cursorLock2Press = false;
    public Material[] lockedMaterial;
    public Material unlockedMaterial;
    public GameObject cursorMenu;
    public Interactable[] cursorToggles;

    // frame and materials
    public MeshRenderer backgroundFrame;
    public Material framePlay;
    public Material framePause;
    public Material frameMove;
    public Material frameStatic;

    void Start()
    {
        updateNext = Time.time + updateWait;

        m.OnToggleSensor += SensorToggle;
        m.OnTogglePP     += PausePlay;
        m.OnToggleAS     += ToggleAS;
        m.OnChangeX      += ChangeX;
        m.OnChangeY      += ChangeY;
        SettingsModel.
          OnSaveSettings += SaveSettings;
        m.OnLoadSettings += LoadSettings;
        m.OnSnapshot     += (int mID) =>         Snapshot(true,  mID, 0);
        m.OnDuplicate    += (int mID, int id) => Snapshot(false, mID, id);
        m.OnDelete       += Delete;
        //m.OnMove       += Move;
        m.OnChangeCursor += CursorChange;
        m.OnToggleCursor += CursorToggle;

        if (!isStatic)
        {
            foreach (SensorChart_KyleMax chart in charts)
            {
                if (chart != null)
                    sensorVisualizations.Add(chart);
            }
        }

        pausePlayButton.OnClick.AddListener(() => m.TogglePP(!m.play));

        autoScaleToggle.IsToggled = m.autoScale;
        autoScaleToggle.OnClick.AddListener(() => m.ToggleAS(!m.autoScale));

        xSlider.OnValueUpdated.AddListener(
            (SliderEventData d) => m.ChangeX(d.NewValue * MAX_TIME));

        xMinText.text = "0s";
        xMaxText.text = MAX_TIME.ToString() + "s";

        yMidDec.OnClick.AddListener(() => m.ChangeYMid(m.yMid - m.yScale / 8));
        yMidInc.OnClick.AddListener(() => m.ChangeYMid(m.yMid + m.yScale / 8));
        yScaleDec.OnClick.AddListener(() => m.ChangeYScale(m.yScale * yDelta));
        yScaleInc.OnClick.AddListener(() => m.ChangeYScale(m.yScale / yDelta));

        ChangeY();

        settSaveButton.OnClick.AddListener(
            () => PreSaveSettings(new SettingsPreset(m.autoScale, m.yMid, m.yScale, m.xScale, m.sensors)));
        settSaves[0].OnClick.AddListener(() => m.LoadSettings(2));
        settSaves[1].OnClick.AddListener(() => m.LoadSettings(1));
        settSaves[2].OnClick.AddListener(() => m.LoadSettings(0));
        settDefault.OnClick.AddListener(m.ResetSettings);

        staticButton.OnClick.AddListener(m.Snapshot);
        dynamicButton.OnClick.AddListener(() => m.Duplicate(0));

        m.Move(false);

        deleteButton.OnClick.AddListener(() => deleting = true);
        deleteNo.    OnClick.AddListener(() => deleting = false);
        deleteYes.   OnClick.AddListener(m.Delete);

        configureButton.OnClick.AddListener(() => focusedSettings = !focusedSettings);

        for (int i = 1; i < sensorToggles.Length; i++)
            sensorToggles[i].IsToggled = m.sensors[i];

        sensorToggles[1].OnClick.AddListener(() => m.ToggleSensor(1, !m.sensors[1]));
        sensorToggles[2].OnClick.AddListener(() => m.ToggleSensor(2, !m.sensors[2]));
        sensorToggles[3].OnClick.AddListener(() => m.ToggleSensor(3, !m.sensors[3]));

        foreach (SensorChart_KyleMax chart in charts)
        {
            if (chart != null && !m.sensors[chart.sensorID])
                SensorToggle(chart.sensorID);
        }

        for (int i = 1; i < sensorLabels.Length; i++)
        {
            sensorLabels[i].GetComponent<MeshRenderer>().material =
            cursorText2[i].GetComponent<MeshRenderer>().material = sensorColorsText[i];
        }

        sensorGrabs[1].OnClick.AddListener(() => m.Duplicate(1));
        sensorGrabs[2].OnClick.AddListener(() => m.Duplicate(2));
        sensorGrabs[3].OnClick.AddListener(() => m.Duplicate(3));

        manipulator.OnManipulationStarted.AddListener((ManipulationEventData _) => m.Move(true));
        manipulator.OnManipulationEnded.AddListener((ManipulationEventData _) => m.Move(false));

        //if (vuforia)
        //{
        //    arCamera.GetComponent<VuforiaBehaviour>().enabled = true;
        //    focusDistance *= 2f; focusAngle += 20;
        //}
        GetComponent<BoundingBox>().Active = hololens;

        if (pauseQueues == null) pauseQueues = new Queue<DataFloat>[charts.Length];

        cursorToggles[0].IsToggled = false;
        cursorToggles[1].IsToggled = false;

        cursorToggles[0].OnClick.AddListener(() => ToggleCursor(true));
        cursorToggles[1].OnClick.AddListener(() => ToggleCursor(false));

        if (m.cursorLocked1) CursorLock(true);
        if (m.cursorLocked2) CursorLock(false);

        cursorSpheresActive1 = new bool[] { false, true, true, true };
        cursorSpheresActive2 = new bool[] { false, true, true, true };
        
        for (int i = 1; i < cursorSpheres1.Length; i++)
        {
            cursorSpheres1[i].GetComponent<MeshRenderer>().material =
            cursorSpheres2[i].GetComponent<MeshRenderer>().material = sensorColors[i];
        }

        SetActiveCursors();
        UpdateFocus();
    }

    void Update()
    {
        if (isSnapshot && lerpCurrent < lerpSteps)
        {
            transform.position += snapshotNextLerp / lerpSteps * transform.lossyScale.x;
            lerpCurrent += 1;
        }

        cursorLock1Press |= Input.GetKeyDown(KeyCode.Alpha1);
        cursorLock2Press |= Input.GetKeyDown(KeyCode.Alpha2);

        if (Time.time < updateNext) return;

        bool tapping = false;
        bool tapped  = false;

        if (!manipulator.enabled) m.Move(false);
        else if (!isSnapshot || lerpCurrent >= lerpSteps)
        {
            if (!hololens)
            {
                Vector3 towardsGraph = transform.position - CoreServices.InputSystem.GazeProvider.GazeOrigin;
                focus = focusAngle > Vector3.Angle(towardsGraph, CoreServices.InputSystem.GazeProvider.GazeDirection)
                     && focusDistance > towardsGraph.magnitude / transform.lossyScale.x
                     && mergeSett == null;
                focusedChannels = focus;
            }
            else
            {
                MixedRealityPose pose;
                if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out pose))
                {
                    focus = (fingerDistance > Vector3.Distance(pose.Position, transform.position) / transform.lossyScale.x);
                    focusedChannels = (fingerDistance / 2 > Vector3.Distance(pose.Position, channelButtons.transform.position) / transform.lossyScale.x);

                    tapping = chartCollider.bounds.Contains(pose.Position);
                    if (tapping && !wasTapping && (m.play || !m.cursorToggled1))
                    {
                        m.TogglePP(!m.play);
                        tapped = true;
                    }
                }
                /* else focus = focusedChannels = false; */
            }
        }

        cursorsEnabled = !m.play;
        if (!m.play && focus && m.cursorToggled1)
        {
            bool tap1 =  (!hololens && cursorLock1Press) || (hololens && tapping && !wasTapping && !tapped   && !m.cursorToggled2);
            bool tap2 = ((!hololens && cursorLock2Press) || (hololens && tapping && !wasTapping && !tapped)) &&  m.cursorToggled2;

            if (tap1 && m.cursorToggled1) CursorLock(true);
            if (tap2 && m.cursorToggled1) CursorLock(false);
        }
        wasTapping = tapping;
        cursorLock1Press = cursorLock2Press = false;

        cursorMenu.SetActive(cursorsEnabled && focus);
        cursorsEnabled &= m.cursorToggled1;

        if (!isStatic)
        {
            if (m.autoScale) ChangeY();
            
            if (mergeSett != null) mergeSett.mergeBox.SetActive(false);
            if (m.isMoving)
            {
                mergeSett = null;

                foreach (SensorVisualization v in sensorVisualizations)
                {
                    SensorChart_KyleMax sc = v.gameObject.GetComponent<SensorChart_KyleMax>();
                    if (sc == null || sc.sett == this || sc.sett.isStatic) continue;

                    float dist = Vector3.Distance(transform.position, sc.sett.transform.position);

                    if (dist < mergeDistance * transform.lossyScale.x)
                    {
                        if (mergeSett != null)
                        {
                            float dist2 = Vector3.Distance(transform.position, mergeSett.transform.position);
                            if (dist2 < dist) mergeSett = sc.sett;
                        }
                        else mergeSett = sc.sett;
                    }
                }

                if (mergeSett != null) mergeSett.mergeBox.SetActive(true);
            }
            else if (wasJustMoving && mergeSett != null)
            {
                foreach (SensorChart_KyleMax sensor in charts)
                {
                    if (sensor != null && sensor.gameObject.activeSelf && !mergeSett.m.sensors[sensor.sensorID])
                    mergeSett.m.ToggleSensor(sensor.sensorID, true);
                }
                m.Delete();
            }
            wasJustMoving = m.isMoving;
        }

        backgroundFrame.transform.GetChild(0).GetComponent<MeshRenderer>().material =
        backgroundFrame.material = m.isMoving ? frameMove : (isStatic ? frameStatic : (m.play ? framePlay : framePause));

        float xVal = SettingsModel.NULL_CURSOR;

        if (cursorsEnabled && focus && !m.cursorLocked2)
        {
            Plane plane = new Plane(planeCorners[0].position, planeCorners[1].position, planeCorners[3].position);
            Ray gaze    = Camera.main.ScreenPointToRay(Input.mousePosition);

            bool hasGaze = true;
            if (hololens)
            {
                MixedRealityPose pose;
                if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out pose))
                {
                    gaze = new Ray(Camera.main.transform.position, pose.Position
                                 - Camera.main.transform.position);
                }
                else hasGaze = false;
            }

            float enter;
            if (plane.Raycast(gaze, out enter) && hasGaze)
            {
                Vector3 point = planeObject.InverseTransformPoint(gaze.GetPoint(enter));

                if (-SensorChart_KyleMax.HEIGHT / 2 <= point.x && point.x <= SensorChart_KyleMax.HEIGHT / 2
                 && -SensorChart_KyleMax.HEIGHT / 2 <= point.y && point.y <= SensorChart_KyleMax.HEIGHT / 2)
                {
                    xVal = lastCursorX = point.x;
                }
            }
            else xVal = lastCursorX;
        }

        if (cursorsEnabled) MoveCursor(xVal);
        else
        {
            for (int i = 1; i < charts.Length; i++)
            {
                if (m.sensors[i] && charts[i].qData.Count > 0)
                    charts[i].UpdateLabel(Math.Round(charts[i].GetNth(charts[i].qData.Count - 1).F, 2).ToString());
            }
        }

        SetActiveCursors();
        UpdateFocus();
        
        updateNext = Time.time + updateWait;
    }

    /// <summary>
    /// Method called after a sensor toggle
    /// </summary>
    /// <param name="id">sensor toggled</param>
    public void SensorToggle(int id)
    {
        sensorToggles[id].IsToggled = m.sensors[id];

        int stencil;
        for (stencil = 0; stencil < charts.Length; stencil++)
        {
            if (charts[stencil] != null) break;
        }

        if (m.sensors[id])
        {
            SensorChart_KyleMax chartNew = Instantiate(charts[stencil], charts[stencil].transform.parent);
            if (!chartNew.gameObject.activeSelf)
            {
                chartNew.gameObject.SetActive(true);
                sensorVisualizations.Remove(charts[stencil]);
                Destroy(charts[stencil].gameObject);
                charts[stencil] = null;
            }

            charts[id] = chartNew;
            chartNew.sensorID = id;
            
            chartNew.qData = m.play ? QueueHistory(id) : GetPauseQueue(id);
            DequeueOld(chartNew.qData, m.xScale);

            chartNew.textLabel = sensorLabels[id];
            sensorLabels[id].gameObject.SetActive(true);
            sensorGrabs [id].gameObject.SetActive(true);

            chartNew.ChangeMaterial(sensorColors[id]);
        }
        else
        {
            int count = 0;
            foreach (SensorChart_KyleMax chart in charts) count += (chart != null) ? 1 : 0;

            if (count == 1)
                charts[stencil].gameObject.SetActive(false);
            else
            {
                sensorVisualizations.Remove(charts[id]);
                Destroy(charts[id].gameObject);
                charts[id] = null;
            }

            sensorLabels[id].gameObject.SetActive(false);
            sensorGrabs [id].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Method called after a pause/play
    /// </summary>
    public void PausePlay()
    {
        pausePlayButton.GetComponentInChildren<TextMesh>().text = m.play ? "Pause" : "Play";

        if (!m.play)
        {
            for (int i = 1; i < pauseQueues.Length; i++)
            {
                pauseQueues[i] = QueueHistory(i);

                float curr = 0f; foreach (DataFloat d in pauseQueues[i]) curr = d.F;
                sensorLabels[i].text = "" + Math.Round(curr, 2);
            }
        }
        else cursorsEnabled = false;
    }

    /// <summary>
    /// Method to return queue history from moment paused
    /// </summary>
    /// <param name="id">sensor</param>
    /// <returns>queue history of sensor when paused</returns>
    public Queue<DataFloat> GetPauseQueue(int id)
    {
        Queue<DataFloat> qNew = new Queue<DataFloat>();

        if (pauseQueues[id] != null)
        {
            foreach (DataFloat d in pauseQueues[id])
                qNew.Enqueue(d);
        }

        return qNew;
    }

    /// <summary>
    /// Method called after an auto-scale toggle
    /// </summary>
    public void ToggleAS()
    {
        if (autoScaleToggle.IsToggled != m.autoScale)
            autoScaleToggle.IsToggled =  m.autoScale;

        ChangeY();

        yMidDec   .gameObject.SetActive(!m.autoScale);
        yMidInc   .gameObject.SetActive(!m.autoScale);
        yScaleDec .gameObject.SetActive(!m.autoScale);
        yScaleInc .gameObject.SetActive(!m.autoScale);
        yMidText  .gameObject.SetActive(!m.autoScale);
        yScaleText.gameObject.SetActive(!m.autoScale);
    }

    /// <summary>
    /// Method called after x scale changes
    /// </summary>
    public void ChangeX()
    {
        xCurrent.text = "Time Interval: " + Math.Round(m.xScale, 2) + "s";

        if (Math.Abs(xSlider.SliderValue - m.xScale / MAX_TIME) > 0.01)
                     xSlider.SliderValue = m.xScale / MAX_TIME;

        if (!m.play)
        {
            for (int i = 1; i < m.sensors.Length; i++)
            {
                if (!m.sensors[i]) continue;

                charts[i].qData = GetPauseQueue(i);
                DequeueOld(charts[i].qData, m.xScale);
            }
        }
    }

    /// <summary>
    /// Method called after y range changes
    /// </summary>
    public void ChangeY()
    {
        for (int i = 0; i < yTicks.Length; i++)
            yTicks[i].text = Math.Round(YMin() + i * (YMax() - YMin()) / (yTicks.Length - 1), 2).ToString();
    }

    /// <summary>
    /// Method to compute graph's y lower-bound
    /// </summary>
    /// <returns>minimum y value</returns>
    public float YMin()
    {
        if (!m.autoScale) return m.yMid - m.yScale / 2;

        float min = float.MaxValue;
        foreach (SensorChart_KyleMax chart in charts)
        {
            if (chart != null && chart.qMin < min)
                min = chart.qMin;
        }

        return min;
    }

    /// <summary>
    /// Method to compute graph's y upper-bound
    /// </summary>
    /// <returns>maximum y value</returns>
    public float YMax()
    {
        if (!m.autoScale) return m.yMid + m.yScale / 2;

        float max = float.MinValue;
        foreach (SensorChart_KyleMax chart in charts)
        {
            if (chart != null && chart.qMax > max)
                max = chart.qMax;
        }

        return max;
    }

    /// <summary>
    /// Method to call settings save event
    /// </summary>
    /// <param name="s">settings preset to save</param>
    public void PreSaveSettings(SettingsPreset s)
    {
        if (s.Equals(SettingsModel.defaultSettings)) return;

        List<SettingsPreset> setts2 = new List<SettingsPreset>();
        foreach (SettingsPreset s2 in SettingsModel.setts)
        {
            if (s.Equals(s2)) return;
            setts2.Add(s2);
        }

        setts2.Insert(0, s);
        if (setts2.Count > SettingsModel.NUM_SETTINGS) setts2.RemoveAt(SettingsModel.NUM_SETTINGS);

        m.SaveSettings(setts2);
    }

    /// <summary>
    /// Method called after settings save
    /// </summary>
    public void SaveSettings()
    {
        for (int i = 0; i < SettingsModel.setts.Count; i++)
        {
            int n = SettingsModel.NUM_SETTINGS - i - 1;

            settSaves[n].gameObject.SetActive(true);
            settSaves[n].GetComponentInChildren<TextMesh>().text = SettingsModel.setts[i].ToString();
        }
    }

    /// <summary>
    /// Method called after settings load
    /// </summary>
    /// <param name="s">settings preset loaded</param>
    public void LoadSettings(SettingsPreset s)
    {
        s.UpdateSettings(this);
        ChangeY();
    }

    /// <summary>
    /// Method called after duplication
    /// </summary>
    /// <param name="isStatic">is a static snapshot (vs. dynamic)</param>
    /// <param name="modelID">newly-created model ID</param>
    /// <param name="id">if non-zero, only sensor enabled on new graph</param>
    public void Snapshot(bool isStatic, int modelID, int id)
    {

        // Create a prefab with SettingsModel script attached
        // Photon Instantiate the prefab
        // GetComponent SettingsModel
        GameObject newChart = PhotonNetwork.Instantiate("Chart", new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity, 0);

        //SettingsModel m2 = Instantiate(m, m.transform.parent);
        SettingsModel m2 = newChart.GetComponent<SettingsModel>();
        //TODO:KYLE:was m2.uniqueModelID = modelID.ToString();

        if (!isStatic && id != 0)
        {
            m2.sensors = new bool[SettingsModel.NUM_SENSORS + 1];
            m2.sensors[id] = true;

            m.ToggleSensor(id, false);
        }



        Settings sett2 = m2.gameObject.GetComponent<Settings>();
        sett2.isSnapshot  = true;
        sett2.isStatic    = isStatic;
        sett2.lerpCurrent = 0;
        sett2.sensorModels = sensorModels;
        sett2.sensorVisualizations = sensorVisualizations;
        //sett2.charts = charts;
        
        for (int i = 1; i < m2.sensors.Length; i++)
        {
            if (m2.sensors[i])
            {
                sett2.charts[i].qData = m2.play ? QueueHistory(i) : GetPauseQueue(i);
                DequeueOld(sett2.charts[i].qData, m2.xScale);
            }
        }
        sett2.pauseQueues = (Queue<DataFloat>[])pauseQueues.Clone();
        m2.play &= !isStatic;

        if (isStatic)
        {
            sett2.staticTimestampText.gameObject.SetActive(true);
            sett2.staticTimestampText.text = "Captured " + DateTime.Now;
        }

        sett2.focus = sett2.focusedChannels = sett2.focusedSettings = false;
        sett2.UpdateFocus();

        sett2.cursorsEnabled = false;
        sett2.cursorMenu.SetActive(false);

        m2.cursorLocked1 = false;
        m2.cursorLocked2 = false;
        m2.cursorX1 = 0;
        m2.cursorX2 = 0;
        m2.cursorToggled1 = false;
        m2.cursorToggled2 = false;
    }

    /// <summary>
    /// Method called after deletion
    /// </summary>
    public void Delete()
    {
        if (!isStatic)
        {
            foreach (SensorChart_KyleMax chart in charts)
            {
                if (chart != null) sensorVisualizations.Remove(chart);
            }
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Method to set active cursor-related objects
    /// </summary>
    private void SetActiveCursors()
    {
        for (int i = 1; i < cursorSpheres1.Length; i++)
            cursorSpheres1[i].gameObject.SetActive(cursorsEnabled && m.sensors[i] && cursorSpheresActive1[i]);
        cursorLine1.          gameObject.SetActive(cursorsEnabled);

        for (int i = 1; i < cursorSpheres2.Length; i++)
            cursorSpheres2[i].gameObject.SetActive(cursorsEnabled && m.cursorLocked1 && m.cursorToggled2 && m.sensors[i] && cursorSpheresActive2[i]);
        cursorLine2.          gameObject.SetActive(cursorsEnabled && m.cursorLocked1 && m.cursorToggled2);

        for (int i = 1; i < cursorText2.Length; i++)
            cursorText2[i].gameObject.SetActive(cursorsEnabled && m.cursorLocked1 && m.cursorToggled2 && m.sensors[i]);
        cursorText2[0].    gameObject.SetActive(cursorsEnabled && m.cursorLocked1 && m.cursorToggled2);

        cursorLine1.GetComponent<MeshRenderer>().material = m.cursorLocked1 ? lockedMaterial[0] : unlockedMaterial;
        cursorLine2.GetComponent<MeshRenderer>().material = m.cursorLocked2 ? lockedMaterial[1] : unlockedMaterial;
    
        cursorToggles[0].GetComponentInChildren<TextMesh>().color = m.cursorLocked1 ? lockedMaterial[0].color : unlockedMaterial.color;
        cursorToggles[1].GetComponentInChildren<TextMesh>().color = m.cursorLocked2 ? lockedMaterial[1].color : unlockedMaterial.color;
    }

    /// <summary>
    /// Method to call cursor lock event
    /// </summary>
    /// <param name="isCursor1">cursor 1 to lock (vs. cursor 2)</param>
    public void CursorLock(bool isCursor1)
    {
        bool locked1 = m.cursorLocked1;
        bool locked2 = m.cursorLocked2;
        float x1 = m.cursorX1;
        float x2 = m.cursorX2;

        if (isCursor1)
        {
            locked1 = !locked1;
            locked2 &= locked1;

            if (!locked1 && m.cursorToggled2) m.ToggleCursor(false, false);
        }
        else locked2 = !locked2;

        m.ChangeCursor(locked1, locked2, x1, x2);
    }

    /// <summary>
    /// Method to move local cursor
    /// </summary>
    /// <param name="x">current cursor x position</param>
    private void MoveCursor(float x)
    {
        if (x != SettingsModel.NULL_CURSOR)
        {
            if (!m.cursorLocked1) m.cursorX1 = x;
            else                  m.cursorX2 = x;
        }

        float xCurr;
        for (int c = 0; c < 2; c++)
        {
            bool isCursor1 = (c == 0);
            if (!isCursor1 && !m.cursorLocked1) continue;

            xCurr = isCursor1 ? m.cursorX1 : m.cursorX2;
            if (xCurr == SettingsModel.NULL_CURSOR) continue;

            int[] cursorPos = { 0, -1, -1, -1 };
            Transform   cursorLine     = isCursor1 ? cursorLine1          : cursorLine2;
            Transform[] cursorSpheres  = isCursor1 ? cursorSpheres1       : cursorSpheres2;
            bool[] cursorSpheresActive = isCursor1 ? cursorSpheresActive1 : cursorSpheresActive2;

            cursorLine.localPosition = new Vector3(xCurr, cursorLine.localPosition.y, cursorLine.localPosition.z);

            for (int i = 1; i < charts.Length; i++)
            {
                if (m.sensors[i])
                {
                    int idx = (int)Math.Round((xCurr + SensorChart_KyleMax.HEIGHT / 2) * (charts[i].numPoints - 1) / SensorChart_KyleMax.HEIGHT);
                    cursorPos[i] = idx;
                    DataFloat d = charts[i].GetNth(idx);

                    float xPos = 1f * idx / charts[i].numPoints          * SensorChart_KyleMax.HEIGHT - SensorChart_KyleMax.HEIGHT / 2;
                    float yPos = 1f * (d.F - YMin()) / (YMax() - YMin()) * SensorChart_KyleMax.HEIGHT - SensorChart_KyleMax.HEIGHT / 2;

                    cursorSpheres[i].localPosition = new Vector3(xPos, yPos, cursorSpheres[i].localPosition.z);
                    cursorSpheresActive[i] = -SensorChart_KyleMax.HEIGHT / 2 <= yPos && yPos <= SensorChart_KyleMax.HEIGHT / 2;
                }
            }

            for (int i = 1; i < charts.Length; i++)
            {
                if (m.sensors[i])
                {
                    string s = Math.Round(charts[i].GetNth(cursorPos[i]).F, 2).ToString();

                    if      (isCursor1)       charts[i].UpdateLabel("C1: " + s);
                    else if (m.cursorLocked1) cursorText2[i].text = "C2: " + s;
                }
            }
        }

        if (m.cursorLocked1)
        {
            float xDiff = (m.cursorX2 - m.cursorX1) / SensorChart_KyleMax.HEIGHT * m.xScale;
            cursorText2[0].text = (xDiff >= 0 ? "+" : "") + Math.Round(xDiff, 2) + "s";
        }
    }

    /// <summary>
    /// Method called after cursor lock event
    /// </summary>
    public void CursorChange()
    {
        cursorToggles[1].gameObject.SetActive(m.cursorLocked1);
    }

    /// <summary>
    /// Method to call cursor toggle event
    /// </summary>
    /// <param name="isCursor1">cursor 1 toggled (vs. cursor2)</param>
    public void ToggleCursor(bool isCursor1)
    {
        if (isCursor1)
        {
            if (m.cursorToggled1 && m.cursorToggled2) m.ToggleCursor(false, false);
            if (m.cursorToggled1 && m.cursorLocked1) CursorLock(true);
            m.ToggleCursor(true, !m.cursorToggled1);
        }
        else
        {
            if (m.cursorToggled2 && m.cursorLocked2) CursorLock(false);
            m.ToggleCursor(false, !m.cursorToggled2);
        }
    }

    /// <summary>
    /// Method called after a cursor toggle
    /// </summary>
    public void CursorToggle()
    {
        if (!m.cursorLocked1 && m.cursorToggled1) m.cursorX1 = 0;
        if (!m.cursorLocked2 && m.cursorToggled2) m.cursorX2 = 0;

        cursorToggles[0].IsToggled = m.cursorToggled1;
        cursorToggles[1].IsToggled = m.cursorToggled2;
    }

    /// <summary>
    /// Method to update active objects based on focus
    /// </summary>
    private void UpdateFocus()
    {
        focusButtons.  SetActive(!isStatic && !deleting && focus);
        uiGameObject.  SetActive(!isStatic && !deleting && focusedSettings);
        channelButtons.SetActive(!isStatic && !deleting && focusedChannels);

        deleteWarning.SetActive(deleting);

        configureButton.GetComponentInChildren<TextMesh>().text = focusedSettings ? "Hide Configuration" : "Show Configuration";

        valuesLabel.text = cursorsEnabled ? "Cursor Value(s)" : "Latest Value(s)";
    }

    public void DequeueOld(Queue<DataFloat> q, float time)
    {
        DataFloat recent = new DataFloat(0f, Time.time);
        foreach (DataFloat d in q) recent = d; // TODO performance: this is basically recent = q.last

        while (q.Count != 0 && recent.T - q.Peek().T > time) q.Dequeue();
    }

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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            {
                stream.SendNext(m.autoScale);
                stream.SendNext(m.play);
                stream.SendNext(m.xScale);
                stream.SendNext(m.yMid);
                stream.SendNext(m.yScale);

                stream.SendNext(m.sensors[1]);
                stream.SendNext(m.sensors[2]);
                stream.SendNext(m.sensors[3]);

            }
        }
        else
        {
            m.autoScale = (bool)stream.ReceiveNext();
            m.play = (bool)stream.ReceiveNext();
            m.xScale = (float)stream.ReceiveNext();
            m.yMid = (float)stream.ReceiveNext();
            m.yScale = (float)stream.ReceiveNext();
            m.sensors[1] = (bool)stream.ReceiveNext();
            m.sensors[2] = (bool)stream.ReceiveNext();
            m.sensors[3] = (bool)stream.ReceiveNext();
            if (m.autoScale)
                ToggleAS();
            if (!m.play)
                PausePlay();

            m.ChangeYScale(m.yScale);
            m.ChangeYMid(m.yMid);
            m.ChangeX(m.xScale);

        }
    }
}
