using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Model of a graph's settings metadata.
/// </summary>
public class SettingsModel : MonoBehaviour
{
    // universal constants
    public static int NUM_SENSORS  = 3;
    public static int NUM_SETTINGS = 3;
    public static float NULL_CURSOR = -100f;

    public PhotonView pv;

    // delegates with different parameters
    public delegate void OnChange();
    public delegate void OnChangeInt(int i);
    public delegate void OnChangeInt2(int i1, int i2);
    public delegate void OnChangePreset(SettingsPreset s);

    // handles sensor toggle event
    public event OnChangeInt OnToggleSensor;
    public bool[] sensors = new bool[NUM_SENSORS + 1];

    // handles play/pause change event
    public event OnChange OnTogglePP;
    public bool play;

    // handles auto-scale change event
    public event OnChange OnToggleAS;
    public bool autoScale;

    // handles X scale change event
    public event OnChange OnChangeX;
    public float xScale;

    // handles Y range change events
    public event OnChange OnChangeY;
    public float yMid;
    public float yScale;

    // handles settings presets save/load events
    public static event OnChange       OnSaveSettings;
    public        event OnChangePreset OnLoadSettings;
    private bool savedSettings;
    private int  preset;

    // universal list of settings presets
    public static List<SettingsPreset> setts = new List<SettingsPreset>();

    // default settings preset (i.e. "Reset Settings")
    public static SettingsPreset defaultSettings =
        new SettingsPreset(false,
                           4,
                           8,
                           DataBroadcaster.MAX_TIME/2,
                           new bool[4] { false, true, false, false });
   
    // handles snapshot/duplication events
    public event OnChangeInt  OnSnapshot;
    public event OnChangeInt2 OnDuplicate;
    private List<int> numSnapshot;
    private List<int> numDuplicate;

    // handles deletion events
    public event OnChange OnDelete;
    private bool deleted;

    // handles movement events
    public bool isMoving;
    //public event OnChange OnMove;

    // handles cursor lock events
    public event OnChange OnChangeCursor;
    public bool cursorLocked1;
    public bool cursorLocked2;
    public float cursorX1;
    public float cursorX2;

    // handles cursor toggle events
    public event OnChange OnToggleCursor;
    public bool cursorToggled1;
    public bool cursorToggled2;






    private void OnLocalModelChanged()
    {
        if (!pv.AmOwner)
            pv.RequestOwnership();
    }
    private int TEMP_uuid = 1000;
    private string GenerateUniqueID()
    {
        //TODO:KYLE: This is a placeholder. The old modelnetworking used to generate unique IDs for the models
        return "" +TEMP_uuid++;
    }

    public SettingsModel()
    {
        sensors        = (bool[])defaultSettings.sensors.Clone();
        play           = true;
        autoScale      = defaultSettings.autoScale;
        xScale         = defaultSettings.xScale;
        yMid           = defaultSettings.yMid;
        yScale         = defaultSettings.yScale;
        savedSettings  = false;
        preset         = -1;
        numSnapshot    = new List<int>();
        numDuplicate   = new List<int>();
        deleted        = false;
        isMoving       = false;
        cursorLocked1  = false;
        cursorLocked2  = false;
        cursorX1       = 0;
        cursorX2       = 0;
        cursorToggled1 = false;
        cursorToggled2 = false;
    }

    /// <summary>
    /// Method to set toggle sensor value
    /// </summary>
    /// <param name="id">sensor to be toggled</param>
    /// <param name="toggled">on/off</param>
    public void ToggleSensor(int id, bool toggled)
    {
        this.sensors[id] = toggled;

        OnLocalModelChanged();
        OnToggleSensor(id);
    }

    /// <summary>
    /// Method to toggle play/pause
    /// </summary>
    /// <param name="play">on/off</param>
    public void TogglePP(bool play)
    {
        this.play = play;

        OnLocalModelChanged();
        OnTogglePP();
    }

    /// <summary>
    /// Method to toggle auto-scale
    /// </summary>
    /// <param name="autoScale">on/off</param>
    public void ToggleAS(bool autoScale)
    {
        this.autoScale = autoScale;

        OnLocalModelChanged();
        OnToggleAS();
    }

    /// <summary>
    /// Method to change X scale
    /// </summary>
    /// <param name="x">scale (seconds)</param>
    public void ChangeX(float x)
    {
        this.xScale = x;

        OnLocalModelChanged();
        OnChangeX();
    }

    /// <summary>
    /// Method to change Y middle value
    /// </summary>
    /// <param name="y">middle (Y-axis units)</param>
    public void ChangeYMid(float y)
    {
        if (!autoScale)
        {
            this.yMid = y;

            OnLocalModelChanged();
            OnChangeY();
        }
    }

    /// <summary>
    /// Method to change Y range size
    /// </summary>
    /// <param name="y">range (Y-axis units)</param>
    public void ChangeYScale(float y)
    {
        if (!autoScale)
        {
            this.yScale = y;

            OnLocalModelChanged();
            OnChangeY();
        }
    }

    /// <summary>
    /// Method to replace universal settings presets
    /// </summary>
    /// <param name="setts2">new presets</param>
    public void SaveSettings(List<SettingsPreset> setts2)
    {
        this.savedSettings = true;

        setts = new List<SettingsPreset>();
        foreach (SettingsPreset s in setts2) setts.Add(s);

        OnLocalModelChanged();
        OnSaveSettings();
    }

    /// <summary>
    /// Method to load settings preset from universal list
    /// </summary>
    /// <param name="i">index into list</param>
    public void LoadSettings(int i)
    {
        this.preset = i;

        OnLocalModelChanged();
        OnLoadSettings(setts[i]);
    }

    /// <summary>
    /// Method to load default settings preset
    /// </summary>
    public void ResetSettings()
    {
        this.preset = NUM_SETTINGS;

        OnLocalModelChanged();
        OnLoadSettings(defaultSettings);
    }

    /// <summary>
    /// Method to create static snapshot of graph (with new model ID)
    /// </summary>
    public void Snapshot()
    {
        int modelID = int.Parse(GenerateUniqueID());
        this.numSnapshot.Add(modelID);

        OnLocalModelChanged();
        OnSnapshot(modelID);
    }

    /// <summary>
    /// Method to create dynamic snapshot of graph (with new model ID)
    /// </summary>
    /// <param name="id">if non-zero, only sensor enabled on new graph</param>
    public void Duplicate(int id)
    {
        int modelID = int.Parse(GenerateUniqueID());
        this.numDuplicate.Add(modelID);
        this.numDuplicate.Add(id);

        OnLocalModelChanged();
        OnDuplicate(modelID, id);
    }

    /// <summary>
    /// Method to delete graph
    /// </summary>
    public void Delete()
    {
        this.deleted = true;

        OnLocalModelChanged();
        //TODO:KYLE: was: SendToServerNow();
        OnDelete();
    }

    /// <summary>
    /// Method to set if the graph is moving
    /// </summary>
    /// <param name="move">if graph moving</param>
    public void Move(bool move)
    {
        this.isMoving = move;

        OnLocalModelChanged();
        //OnMove();
    }

    /// <summary>
    /// Method to change cursor lock
    /// </summary>
    /// <param name="c1">cursor 1 locked</param>
    /// <param name="c2">cursor 2 locked</param>
    /// <param name="x1">cursor 1 x position</param>
    /// <param name="x2">cursor 2 x position</param>
    public void ChangeCursor(bool c1, bool c2, float x1, float x2)
    {
        this.cursorLocked1 = c1;
        this.cursorLocked2 = c2;
        this.cursorX1      = x1;
        this.cursorX2      = x2;

        OnLocalModelChanged();
        OnChangeCursor();
    }

    /// <summary>
    /// Method to toggle cursor
    /// </summary>
    /// <param name="isCursor1">cursor 1 to toggle (vs. cursor2)</param>
    /// <param name="toggled">cursor on/off</param>
    public void ToggleCursor(bool isCursor1, bool toggled)
    {
        if (isCursor1) cursorToggled1 = toggled;
        else           cursorToggled2 = toggled;

        OnLocalModelChanged();
        OnToggleCursor();
    }

    // unique identifiers preceding values in stream
    private const int fieldID_sensors        = 0;
    private const int fieldID_play           = 1;
    private const int fieldID_autoScale      = 2;
    private const int fieldID_xScale         = 3;
    private const int fieldID_yMid           = 4;
    private const int fieldID_yScale         = 5;
    private const int fieldID_preset_list    = 6;
    private const int fieldID_preset         = 7;
    private const int fieldID_numSnapshot    = 8;
    private const int fieldID_numDuplicate   = 9;
    private const int fieldID_deleted        = 10;
    private const int fieldID_isMoving       = 11;
    private const int fieldID_cursorLocked1  = 12;
    private const int fieldID_cursorLocked2  = 13;
    private const int fieldID_cursorX1       = 14;
    private const int fieldID_cursorX2       = 15;
    private const int fieldID_cursorToggled1 = 16;
    private const int fieldID_cursorToggled2 = 17;
    
    /**
    public override void SerializeModelFull(Stream outStream, NetworkedModelFieldSerializer formatter)
    {
        // 0
        formatter.Serialize(outStream, fieldID_sensors);
        formatter.Serialize(outStream, this.sensors);

        // 1
        formatter.Serialize(outStream, fieldID_play);
        formatter.Serialize(outStream, this.play);

        // 2
        formatter.Serialize(outStream, fieldID_autoScale);
        formatter.Serialize(outStream, this.autoScale);

        // 3
        formatter.Serialize(outStream, fieldID_xScale);
        formatter.Serialize(outStream, this.xScale);

        // 4
        formatter.Serialize(outStream, fieldID_yMid);
        formatter.Serialize(outStream, this.yMid);

        // 5
        formatter.Serialize(outStream, fieldID_yScale);
        formatter.Serialize(outStream, this.yScale);

        // 6
        formatter.Serialize(outStream, fieldID_preset_list);
        formatter.Serialize(outStream, this.savedSettings);
        if (savedSettings)
        {
            formatter.Serialize(outStream, SettingsModel.setts.Count);
            for (int i = 0; i < setts.Count; i++)
            {
                formatter.Serialize(outStream, setts[i].autoScale);
                formatter.Serialize(outStream, setts[i].yMid);
                formatter.Serialize(outStream, setts[i].yScale);
                formatter.Serialize(outStream, setts[i].xScale);
                formatter.Serialize(outStream, setts[i].sensors);
            }
        }
        this.savedSettings = false;

        // 7
        formatter.Serialize(outStream, fieldID_preset);
        formatter.Serialize(outStream, this.preset);
        this.preset = -1;

        // 8
        formatter.Serialize(outStream, fieldID_numSnapshot);
        formatter.Serialize(outStream, this.numSnapshot.Count);
        for (int i = 0; i < numSnapshot.Count; i++) formatter.Serialize(outStream, numSnapshot[i]);
        this.numSnapshot = new List<int>();

        // 9
        formatter.Serialize(outStream, fieldID_numDuplicate);
        formatter.Serialize(outStream, this.numDuplicate.Count / 2);
        for (int i = 0; i < numDuplicate.Count; i++) formatter.Serialize(outStream, numDuplicate[i]);
        this.numDuplicate = new List<int>();

        // 10
        formatter.Serialize(outStream, fieldID_deleted);
        formatter.Serialize(outStream, this.deleted);

        // 11
        formatter.Serialize(outStream, fieldID_isMoving);
        formatter.Serialize(outStream, this.isMoving);

        // 12
        formatter.Serialize(outStream, fieldID_cursorLocked1);
        formatter.Serialize(outStream, this.cursorLocked1);

        // 13
        formatter.Serialize(outStream, fieldID_cursorLocked2);
        formatter.Serialize(outStream, this.cursorLocked2);

        // 14
        formatter.Serialize(outStream, fieldID_cursorX1);
        formatter.Serialize(outStream, this.cursorLocked1 ? this.cursorX1 : NULL_CURSOR);

        // 15
        formatter.Serialize(outStream, fieldID_cursorX2);
        formatter.Serialize(outStream, this.cursorLocked2 ? this.cursorX2 : NULL_CURSOR);

        // 16
        formatter.Serialize(outStream, fieldID_cursorToggled1);
        formatter.Serialize(outStream, this.cursorToggled1);

        // 17
        formatter.Serialize(outStream, fieldID_cursorToggled2);
        formatter.Serialize(outStream, this.cursorToggled2);
    }

    public override void DeserializeModelFull(Stream inStream, NetworkedModelFieldSerializer formatter)
    {
        bool[] oldSensors   = this.sensors;
        bool   oldPlay      = this.play;
        bool   oldAutoScale = this.autoScale;
        float  oldXScale    = this.xScale;
        float  oldYMid      = this.yMid;
        float  oldYScale    = this.yScale;
        //bool oldIsMoving  = this.isMoving;
        bool   oldLocked1   = this.cursorLocked1;
        bool   oldLocked2   = this.cursorLocked2;
        float  oldCursorX1  = this.cursorX1;
        float  oldCursorX2  = this.cursorX2;
        bool   oldCToggled1 = this.cursorToggled1;
        bool   oldCToggled2 = this.cursorToggled2;

        Debug.Assert(fieldID_sensors == (int)formatter.Deserialize(inStream));
        this.sensors = (bool[])formatter.Deserialize(inStream);
        
        Debug.Assert(fieldID_play == (int)formatter.Deserialize(inStream));
        this.play = (bool)formatter.Deserialize(inStream);
        
        Debug.Assert(fieldID_autoScale == (int)formatter.Deserialize(inStream));
        this.autoScale = (bool)formatter.Deserialize(inStream);
        
        Debug.Assert(fieldID_xScale == (int)formatter.Deserialize(inStream));
        this.xScale = (float)formatter.Deserialize(inStream);

        Debug.Assert(fieldID_yMid == (int)formatter.Deserialize(inStream));
        this.yMid = (float)formatter.Deserialize(inStream);

        Debug.Assert(fieldID_yScale == (int)formatter.Deserialize(inStream));
        this.yScale = (float)formatter.Deserialize(inStream);

        Debug.Assert(fieldID_preset_list == (int)formatter.Deserialize(inStream));
        if ((bool)formatter.Deserialize(inStream))
        {
            List<SettingsPreset> setts2 = new List<SettingsPreset>();

            int num = (int)formatter.Deserialize(inStream);
            for (int i = 0; i < num; i++)
            {
                bool autoScale2 = (bool)  formatter.Deserialize(inStream);
                float yMid2     = (float) formatter.Deserialize(inStream);
                float yScale2   = (float) formatter.Deserialize(inStream);
                float xScale2   = (float) formatter.Deserialize(inStream);
                bool[] sensors2 = (bool[])formatter.Deserialize(inStream);

                setts2.Add(new SettingsPreset(autoScale2, yMid2, yScale2, xScale2, sensors2));
            }

            setts = setts2;
            OnSaveSettings();
        }

    Debug.Assert(fieldID_preset == (int)formatter.Deserialize(inStream));
        int preset = (int)formatter.Deserialize(inStream);
        if (preset != -1) OnLoadSettings(preset != NUM_SETTINGS ? setts[preset] : defaultSettings);

    Debug.Assert(fieldID_numSnapshot == (int)formatter.Deserialize(inStream));
        int snapshot = (int)formatter.Deserialize(inStream);
        for (int i = 0; i < snapshot; i++) OnSnapshot((int)formatter.Deserialize(inStream));

    Debug.Assert(fieldID_numDuplicate == (int)formatter.Deserialize(inStream));
        int duplicate = (int)formatter.Deserialize(inStream);
        for (int i = 0; i < duplicate; i++) OnDuplicate((int)formatter.Deserialize(inStream),
                                                        (int)formatter.Deserialize(inStream));

    Debug.Assert(fieldID_deleted == (int)formatter.Deserialize(inStream));
        this.deleted = (bool)formatter.Deserialize(inStream);
        if (deleted) OnDelete();

    Debug.Assert(fieldID_isMoving == (int)formatter.Deserialize(inStream));
        this.isMoving = (bool)formatter.Deserialize(inStream);

    Debug.Assert(fieldID_cursorLocked1 == (int)formatter.Deserialize(inStream));
        this.cursorLocked1 = (bool)formatter.Deserialize(inStream);

    Debug.Assert(fieldID_cursorLocked2 == (int)formatter.Deserialize(inStream));
        this.cursorLocked2 = (bool)formatter.Deserialize(inStream);

        
    Debug.Assert(fieldID_cursorX1 == (int)formatter.Deserialize(inStream));
        float x1 = (float)formatter.Deserialize(inStream);
        this.cursorX1 = this.cursorLocked1 ? x1: this.cursorX1;

        
        Debug.Assert(fieldID_cursorX2 == (int)formatter.Deserialize(inStream));
        float x2 = (float)formatter.Deserialize(inStream);
        this.cursorX2 = this.cursorLocked2 ? x2: this.cursorX2;


        Debug.Assert(fieldID_cursorToggled1 == (int)formatter.Deserialize(inStream));
        this.cursorToggled1 = (bool)formatter.Deserialize(inStream);

    Debug.Assert(fieldID_cursorToggled2 == (int)formatter.Deserialize(inStream));
        this.cursorToggled2 = (bool)formatter.Deserialize(inStream);

        // figure out if anything has actually changed and trigger events
        for (int i = 0; i < this.sensors.Length; i++)
        {
            if (this.sensors[i] != oldSensors[i]) OnToggleSensor(i);
        }
        if (oldPlay       != this.play)           OnTogglePP();
        if (oldAutoScale  != this.autoScale)      OnToggleAS();
        if (oldXScale     != this.xScale)         OnChangeX();
        if (oldYMid       != this.yMid ||
            oldYScale     != this.yScale)         OnChangeY();
        //if (oldIsMoving != this.isMoving)       OnMove();
        if (oldLocked1    != this.cursorLocked1 ||
            oldLocked2    != this.cursorLocked2 ||
            oldCursorX1   != this.cursorX1      ||
            oldCursorX2   != this.cursorX2)       OnChangeCursor();
        if (oldCToggled1  != this.cursorToggled1 ||
            oldCToggled2  != this.cursorToggled2) OnToggleCursor();
    }

    ***/
}
