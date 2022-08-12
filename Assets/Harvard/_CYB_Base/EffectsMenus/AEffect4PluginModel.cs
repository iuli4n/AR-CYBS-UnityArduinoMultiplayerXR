using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Base model for an effects plugin.
public abstract class AEffect4PluginModel : MonoBehaviour
{
    abstract public void DoPhotonSerialize(bool isWriting, PhotonStream stream); // called whenever the model needs to be written/read from photon
}