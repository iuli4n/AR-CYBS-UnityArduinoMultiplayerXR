using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This just makes a Unity Editor GUI slider that lets you change the model attached.
/// It's suggested to use a data switch for the model, so it's easy to swap its channel.
/// </summary>
public class ManualChannelFromGUI : MonoBehaviour
{
    public AtomicDataModel model;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {

        model.Value = GUILayout.HorizontalScrollbar(model.Value, 0f, 1024f, 10f);
        GUILayout.Label("...............................");
    }
}
