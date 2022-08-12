using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_ColorFromSensor : MonoBehaviour
{
    public Renderer renderer;
    public AtomicDataSwitch sensorData;

    public Color colorStart = Color.red;
    public Color colorEnd = Color.green;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        renderer.material.color = Color.Lerp(colorStart, colorEnd, sensorData.Value / 1024f);
    }
}