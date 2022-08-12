using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AtomicModelLabel : MonoBehaviour
{
    public AtomicDataModelString model;
    private TextMeshPro text;

    // Start is called before the first frame update
    void Start()
    {
        text = this.gameObject.GetComponent<TextMeshPro>();
        model.OnDataUpdated += (x) => { text.SetText(x); };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
