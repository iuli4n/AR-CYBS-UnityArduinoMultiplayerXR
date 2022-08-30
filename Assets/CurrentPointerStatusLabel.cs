using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentPointerStatusLabel : MonoBehaviour
{
    public TMPro.TextMeshPro text;

    // Start is called before the first frame update
    void Start()
    {
        TEMP_Debug_Pointers.Instance.OnPointerSwitched += (x,y) => { text.text = "" + TEMP_Debug_Pointers.Instance.currentPointerType.ToString(); };
    }

    // Update is called once per frame
    void Update()
    {
    }
}
