using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuButtonChangeLabel : MonoBehaviour
{
    TextMeshPro textMeshP;
    public CreationObjectManager myObjectManager;
    string label;
    

    // Start is called before the first frame update
    void Start()
    {
        textMeshP = GetComponent<TextMeshPro>();
        //Debug.Log(textMeshP.text);
        //textMeshP.text = "Object Name";

    }

    public void ChangeText(string name)
    {
        label = name;

        textMeshP.text = label;
        //Debug.Log("*******************************" + name);


    }
}
