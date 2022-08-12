using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TEMP_DebugProjects : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void OnGUI()
    {
        foreach (string name in SceneStepsManager.Instance.GetProjectNames())
        {
            if (GUILayout.Button("Load project: "+name))
            {
                SceneStepsManager.Instance.OpenProject(name);
            }
        }
    }
}
