using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreationButtonScript : MonoBehaviour
{
    public CreationObjectManager myObjectManager;
    public string _name;

    public void Start()
    {
        _name = gameObject.name;
    }

    public void ButtonPress()
    {
        myObjectManager.ExecuteButtonPress(_name);

    }
}
