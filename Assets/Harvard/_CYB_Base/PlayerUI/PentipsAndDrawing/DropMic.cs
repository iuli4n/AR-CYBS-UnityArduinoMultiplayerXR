using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMic : PenScript
{
    public GameObject micPrefab;

    private GameObject pinParent;

   private Quaternion micRotation;


    public bool micDetected;
    private bool onAttachable;


    // Start is called before the first frame update
    void Start()
    {
        micDetected = false;
        onAttachable = false;
        micRotation = new Quaternion(0f, 180f, 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        /*FAKEPHOT
        
        if (!photonView.IsMine)
        {
            // ignore pen tips that aren't mine
            return;
        }
        */

        if (micDetected == false)
        {

            if (Input.GetKeyUp(KeyCode.Space)) // if (OVRInput.GetUp(OVRInput.Button.One))
            {
                //var newMicObject = Instantiate(micPrefab, this.transform.position, Quaternion.identity);
                var newMicObject = Instantiate(micPrefab, this.transform.position, micRotation);
                newMicObject.name = "Microphone";


                if (onAttachable)
                {
                    newMicObject.transform.parent = pinParent.transform;
                }
            }
        }

    }

    void OnTriggerStay(Collider other)
    {
        if (other.name == "Microphone")
        {

            micDetected = true;

        }


        if (other.tag == "Attachable")
        {
            onAttachable = true;
            pinParent = other.gameObject;

        }

    }

    private void OnTriggerExit(Collider other)
    {

        if (other.name == "Microphone")
        {
            micDetected = false;
        }


        if (other.tag == "Attachable")
        {
            onAttachable = false;
        }

    }
}
