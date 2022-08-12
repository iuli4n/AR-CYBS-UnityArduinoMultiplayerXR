using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this on any object that should only be active if in UNITY_EDITOR.
/// If using this in a separate project, make sure to edit the project Script Execution Order to ensure this runs before anything else
/// </summary>
public class Debug_DisableIfnotPCEditor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        this.gameObject.SetActive(true);
#else
        this.gameObject.SetActive(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
