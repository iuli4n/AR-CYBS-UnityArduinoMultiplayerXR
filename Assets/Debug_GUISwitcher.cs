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

    // Start is called before the first frame update
    void Start()
    {
        
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

        if (GUILayout.Button(s))
        {
            enabled = !enabled;
        }
    }
    
    void OnGUI()
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
