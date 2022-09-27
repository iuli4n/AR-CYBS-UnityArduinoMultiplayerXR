using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
[RequireComponent(typeof(ARPlaneManager))]
public class HidePlanes : MonoBehaviour
{

    /*  void SetAllPlanesActive(bool value)
      {
          foreach (var plane in m_ARPlaneManager.trackables)
              plane.gameObject.SetActive(value);
      }*/

    void Awake()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        Debug.Log("Plane manager object created");
    }

    public ARPlaneManager m_ARPlaneManager;

    // Start is called before the first frame update
    void Start()
    {

        //SetAllPlanesActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        /*  foreach (var plane in m_ARPlaneManager.trackables)
          {
              plane.gameObject.SetActive(false);
              // Debug.Log("Plane Hidden");
          }

          Debug.Log("Plane Hidden");*/
    }

}
