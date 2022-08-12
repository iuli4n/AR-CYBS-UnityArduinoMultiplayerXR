using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerUIManager : MonoBehaviour
{
    public static LocalPlayerUIManager Instance;

    public GameObject actualPointerTip;

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Assert(!Instance, "Expecting singleton but is not");
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
