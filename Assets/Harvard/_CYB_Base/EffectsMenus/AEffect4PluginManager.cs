using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface for managing an Effects Plugin. This is called when the plugin starts/stops, and when it runs on an object.
public abstract class AEffect4PluginManager : MonoBehaviour
{
    abstract public AEffect4PluginModel GetModel(); // Return the model managed by this plugin
    abstract public AEffect4PluginMenu GetMenu(); // Return the menu managed by this plugin



    abstract public void OnCreated(); // This is called once per lifetime of the object; if it's saved/loaded from disk it won't be called
    abstract public void OnEnable(); // This is called every time the object script is enabled (because it's waking up from disk, or because it's enabled/disabled in the editor)
    abstract public void OnDisable(); // Called every time the object is disabled

    abstract public void ToggleMenuGUI(); // open/close the menu GUI

    abstract public string GetPluginName();


    // Called whenever the effect runs
    abstract public void RespondToNewValue(
        // current and previous values of the sensor
        float currentSensorValue, float oldSensorValue,
        // the starting transform of the object to be changed
        Vector3 baseLocalPos, Quaternion baseLocalRot, Vector3 baseLocalScale,
        
        // output: new sensor value (only change it if needed)
        ref float newSensorValue,
        // output: new position/rotation/scale (only change them if needed)
        ref Vector3 newLocalPos, ref Quaternion newLocalRot, ref Vector3 newLocalScale);
}