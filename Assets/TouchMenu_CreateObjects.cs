using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMenu_CreateObjects : MonoBehaviour
{
    public bool showGUI = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        /*** TODO: REMOVE THIS WHOLE CLASS BECAUSE NOT USED ANYMORE
         * 
         * 
        GUILayout.BeginVertical();

        GUILayout.Label("");

        if (GUILayout.Button("SensorAndBarVisualizers"))
        {
            string prefabName = "SensorAndBarVisualizers";
            if (!Application.isPlaying || SceneStepsManager.Instance == null) { Debug.LogError("Cannot use this menu because project is not running"); return; }
            GameObject.Find("AREA_CRAFTING/CreationMenus/ObjectMenu/3DObjectManager").GetComponent<CreationObjectManager>().ExecuteButtonPress(prefabName);
        }

        if (GUILayout.Button("Arrow"))
        {
            string prefabName = "Arrow";
            if (!Application.isPlaying || SceneStepsManager.Instance == null) { Debug.LogError("Cannot use this menu because project is not running"); return; }
            GameObject.Find("AREA_CRAFTING/CreationMenus/ObjectMenu/3DObjectManager").GetComponent<CreationObjectManager>().ExecuteButtonPress(prefabName);
        }

        GUILayout.EndVertical();
        ***/
    }
}
