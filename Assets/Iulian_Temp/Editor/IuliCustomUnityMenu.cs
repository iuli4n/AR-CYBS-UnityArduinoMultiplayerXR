using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IuliCustomUnityMenu : MonoBehaviour
{
    [MenuItem("Iulian/Create: ARROW")]
    private static void Create_Arrow()
    {
        string prefabName = "Arrow";
        
        if (!Application.isPlaying || SceneStepsManager.Instance == null) { Debug.LogError("Cannot use this menu because project is not running"); return; }
        GameObject.Find("AREA_CRAFTING/CreationMenus/ObjectMenu/3DObjectManager").GetComponent<CreationObjectManager>().ExecuteButtonPress(prefabName);
    }

    [MenuItem("Iulian/Create: TOOLTIP")]
    private static void Create_Tooltip()
    {
        string prefabName = "__TooltipCPP";

        if (!Application.isPlaying || SceneStepsManager.Instance == null) { Debug.LogError("Cannot use this menu because project is not running"); return; }
        GameObject.Find("AREA_CRAFTING/CreationMenus/ObjectMenu/3DObjectManager").GetComponent<CreationObjectManager>().ExecuteButtonPress(prefabName);
    }
    
    [MenuItem("Iulian/Create: SENSORCHART")]
    private static void Create_SensorChart()
    {
        string prefabName = "SensorAndBarVisualizers";

        if (!Application.isPlaying || SceneStepsManager.Instance == null) { Debug.LogError("Cannot use this menu because project is not running"); return; }
        GameObject.Find("AREA_CRAFTING/CreationMenus/ObjectMenu/3DObjectManager").GetComponent<CreationObjectManager>().ExecuteButtonPress(prefabName);
    }
}
