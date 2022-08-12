using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_GUISwitcher : MonoBehaviour
{
    // switches between different GUIs
    
    public FileDriveManager gui_files;
    public EditingManager gui_editing;
    public GameObject simulatedSensors;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    string GBSS(string name, bool enabled)
    {
        return (enabled ? "[" : " ") + name + (enabled ? "]" : " ");
    }
    void GBS(string name, ref bool enabled)
    {
        string s = GBSS(name, enabled);

        if (GUILayout.Button(s))
        {
            enabled = !enabled;
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("                                                                        ");
        GBS("FILES", ref gui_files.debug_gui);
        GBS("POINT", ref gui_editing.showGUI);
        if (GUILayout.Button(GBSS("SIM", simulatedSensors.activeInHierarchy)))
        {
            simulatedSensors.SetActive(!simulatedSensors.activeInHierarchy);
        }

        GUILayout.EndHorizontal();
    }
}
