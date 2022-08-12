using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingPropertiesManager : MonoBehaviour
{
    public AtomicDataModel colorModel;

    private LineRenderer line;

    public Material lineMaterial_color0;
    public Material lineMaterial_color1;
    public Material lineMaterial_color2;
    public Material lineMaterial_color3;


    // Start is called before the first frame update
    void Start()
    {
        line = this.GetComponent<PlayerCreatedDrawing>().drawingLine;
        colorModel.OnDataUpdated += ColorModel_OnUpdated;
    }

    void ColorModel_OnUpdated(float newval)
    {
        if (newval == 0)
            line.material = lineMaterial_color0;
        if (newval == 1)
            line.material = lineMaterial_color1;
        if (newval == 2)
            line.material = lineMaterial_color2;
        if (newval == 3)
            line.material = lineMaterial_color3;

    }

    public void SetColor(int i)
    {
        colorModel.Value = i;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
