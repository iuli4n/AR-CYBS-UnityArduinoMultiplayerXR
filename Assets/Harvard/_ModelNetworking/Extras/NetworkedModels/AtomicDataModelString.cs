using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomicDataModelString : GenericAtomicDataModel<string>
{
#if UNITY_EDITOR
    public bool unityeditor_constantoverride = false;
    public string unityeditor_newstring;

    private void Update()
    {
        if (unityeditor_constantoverride && !Value.Equals(unityeditor_newstring))
        {
            Debug.LogWarning("IULIAN REMEMBER: UNITY EDITOR OVERRIDING STRING VALUE !");
            Value = unityeditor_newstring;
        }
    }
#endif
}
