using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonSimulatorIuli : MonoBehaviour
{
    public static PhotonSimulatorIuli Instance; // set on Awake of the main gameobject 

    // set to True if the project is using the real Photon
    // set to False if the project is using simulated Photon (& remove all photon scripts from the project)
    public bool USING_REAL_PHOTON = false;


    void Awake()
    {
        Debug.Assert(PhotonSimulatorIuli.Instance == null, "There can be only one PhotonSimulator object in the scene");
        PhotonSimulatorIuli.Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
