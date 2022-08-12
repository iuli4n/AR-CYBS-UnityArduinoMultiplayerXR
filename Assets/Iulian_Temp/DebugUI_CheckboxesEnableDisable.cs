using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI_CheckboxesEnableDisable : MonoBehaviour
{
    public string[] names;
    public GameObject[] objects;
    private bool[] isEnabled;

    public bool menuEnabled;

    // Start is called before the first frame update
    void Start()
    {
        isEnabled = new bool[names.Length];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnGUI()
    {
        if (Input.GetKeyDown("/"))
        {
            menuEnabled = !menuEnabled;
        }

        if (menuEnabled)
        {
            GUILayout.BeginVertical();

            GUILayout.Label(".");
            GUILayout.Label(".");
            GUILayout.Label(".");
            GUILayout.Label(".");
            GUILayout.Label(".");

            for (int i=0; i<names.Length; i++)
            {
                if (objects[i].activeSelf != GUILayout.Toggle(objects[i].activeSelf, names[i]))
                {
                    objects[i].SetActive(!objects[i].activeSelf);
                }
            }

            GUILayout.EndVertical();
        }
    }
}
