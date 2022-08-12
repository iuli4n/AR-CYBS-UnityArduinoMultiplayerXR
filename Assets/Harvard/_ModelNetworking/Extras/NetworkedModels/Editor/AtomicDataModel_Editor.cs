using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/**
[CustomEditor(typeof(AtomicDataModel))]
class AtomicDataModel_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("REQUEST OWNERSHIP"))
        {
            if (!pv.AmOwner)
                pv.RequestOwnership();
        }
    }
}
**/
