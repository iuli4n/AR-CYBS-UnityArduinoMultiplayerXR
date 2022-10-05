using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_GUISwitcher : MonoBehaviour
{
    // switches between different GUIs

    public DebugUI_CalibrateKinectHL gui_calibration;
    public FileDriveManager gui_files;
    public EditingManager gui_editing;
    public SceneStepsManager gui_scenes;
    ///public GameObject simulatedSensors;
    ///
     //referencing menus and data channel 
    public GameObject channelcontrol;
    public GameObject objMenu;
    public GameObject imgMenu;
    public GameObject spawnCylinder;


    //For enabling/disabling menus 
    public bool channelValue = false;
    public bool objValue = false;
    public bool imjValue = false;
    public bool showAndroidGUI = false; 
    // Start is called before the first frame update
    void Start()
    {

        initMobileObjects();
        
    }
    //initializes Game Objects
    public void initMobileObjects()
    {
        channelcontrol = GameObject.Find("ManualDataChannel");
        objMenu = GameObject.Find("ObjectMenu");
        imgMenu = GameObject.Find("ImageMenu");
        spawnCylinder = GameObject.Find("SpawnCylinder");
    }
    //Creates additional Buttons for Mobile UI 
    public void MobileGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("                                             " +
            "                                                                           ");
        GUIGameobjectButtonSelector("POINT", ref gui_editing.showGUI);
        GUIGameobjectButtonSelector("SCENE", ref gui_scenes.showGUI);
        GUIGameobjectButtonSelector("CALIB", ref gui_calibration.showGUI);
        GUIGameobjectButtonSelector("FILES", ref gui_files.debug_gui);
        GUILayout.Label("                        ");
        //Additional Code for Enabling Disbling Menus and Data channel
        if (GUILayout.Button("View Channels", GUILayout.Width(Screen.width / 10), GUILayout.Height(Screen.height / 10)))
        {
            channelValue = !channelValue;
            channelcontrol.SetActive(channelValue);
        }

        if (GUILayout.Button("Object Menu", GUILayout.Width(Screen.width / 10), GUILayout.Height(Screen.height / 10)))
        {
            objValue = !objValue;
            objMenu.SetActive(objValue);
            spawnCylinder.SetActive(objValue);
        }
        if (GUILayout.Button("Image Menu", GUILayout.Width(Screen.width / 10), GUILayout.Height(Screen.height / 10)))
        {
            imjValue = !imjValue;
            imgMenu.SetActive(imjValue);
            spawnCylinder.SetActive(imjValue);
        }

        GUILayout.EndHorizontal();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    string GUIButtonSelector(string name, bool enabled)
    {
        return (enabled ? "[" : " ") + name + (enabled ? "]" : " ");
    }
    void GUIGameobjectButtonSelector(string name, ref bool enabled)
    {
        string s = GUIButtonSelector(name, enabled);

        if (GUILayout.Button(s, GUILayout.Width(Screen.width / 10), GUILayout.Height(Screen.height / 10)))
        {
            enabled = !enabled;
        }
    }
    
    void OnGUI()
    {
        //Creates Mobile UI or runs in PC. showAndroidGUI is used only for debugging
        if (Application.platform == RuntimePlatform.Android || showAndroidGUI == true)
        {
            MobileGUI();
            Debug.Log("Andorid enabled DebugSwitcher");
        }
        else
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("                                                                        ");
            GUIGameobjectButtonSelector("POINT", ref gui_editing.showGUI);
            GUIGameobjectButtonSelector("SCENE", ref gui_scenes.showGUI);
            GUIGameobjectButtonSelector("CALIB", ref gui_calibration.showGUI);
            GUIGameobjectButtonSelector("FILES", ref gui_files.debug_gui);

            /**
            if (GUILayout.Button(GUIButtonSelector("SIM", simulatedSensors.activeInHierarchy)))
            {
                simulatedSensors.SetActive(!simulatedSensors.activeInHierarchy);
            }
            ***/

            GUILayout.EndHorizontal();
        }

        
    }
}
