using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : PenScript
{
    private void Start()
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        if (!this.enabled) 
            return;
        /*FAKEPHOT
        
        if (PhotonSimulatorIuli.Instance.USING_REAL_PHOTON ? !photonView.IsMine : false)
        {
            // ignore pen tips that aren't mine
            return;
        }
        */

        Debug.LogError("Eraser pen tip is not supported anymore. Use TrashScript. Collided with " + other.name);
        return;

        if (other.GetComponent<PlayerCreatedObject>())
        {
            // TODOIRNOW: THIS NEEDS TO BE DONE THROUGH NETWORK
            PhotonView pv = other.gameObject.GetComponent<PhotonView>();
            if (pv)
            {
                // networked object
                SceneStepsManager.Instance.FromClient_DeleteObject(pv);
            }
            else
            {
                Debug.LogWarning("Erased a local object: "+ other.gameObject.name);
                Destroy(other.gameObject);
            }
        }

        else if (other.tag == "DrawParentTag")
        {
            if (Input.GetKeyDown(KeyCode.Space)) // if (OVRInput.GetDown(OVRInput.Button.One))
            {
                Destroy(other.gameObject);
            }

        }

        
            

    }
}
