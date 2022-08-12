using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface of a plugin menu that will be shown/hidden in the Effects Menu as a tab
public abstract class AEffect4PluginMenu : MonoBehaviour
{
    abstract public void OpenMenu(Transform parentLocation); // menu is about to be shown at that location
    abstract public void HideMenu(); // menu is about to be hidden
    abstract public void Refresh(float currentSensorValue); // menu should be refreshed
}