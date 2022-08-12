using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: FEB 2021: Un-network pin creation (search for photon, create new DropPin_nonPhoton function)

public class DropApin : PenScript
{
    public GameObject pinPrefab;
    private GameObject pinParent;


    public bool touchingOtherPin;
    public bool isDestroyed;
    public bool onAttachable;
    private BoxCollider boxColPin;

    public float intervalTimer;
    public bool intervalStart;



    // Start is called before the first frame update
    void Start()
    {
        touchingOtherPin = false;
        isDestroyed = false;
        onAttachable = false;
        intervalTimer = 0.0f;
    }

    public void DropPin/*fakephoton*/ (GameObject pinPrefab, Vector3 position, Transform parentTransform)
    {
        GameObject newPinObject = Instantiate(pinPrefab, position, Quaternion.identity);
        RPC_ConfigureNewPin_nophoton(newPinObject,
            parentTransform == null ? null : parentTransform.gameObject);
        
        /* FAKEPHOTON
        GameObject newPinObject = PhotonNetwork.Instantiate(pinPrefab.name, position, Quaternion.identity);

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("RPC_ConfigureNewPin", RpcTarget.All,
            newPinObject.GetComponent<PhotonView>().ViewID,
            parentTransform == null ? -1 : parentTransform.gameObject.GetComponent<PhotonView>().ViewID);
        */
    }
    /*
    [PunRPC]
    public void RPC_ConfigureNewPin(int newPinID, int parentID)
    {
        GameObject newPinObject = PhotonNetwork.GetPhotonView(newPinID).gameObject;

        newPinObject.name = "ErasablePin";
        newPinObject.AddComponent<HighlightChangeMaterial>();

        if (parentID != -1)
        {
            newPinObject.transform.parent = PhotonNetwork.GetPhotonView(parentID).gameObject.transform;
        }
    }*/

    public void RPC_ConfigureNewPin_nophoton(GameObject newPin, GameObject parent)
    {
        GameObject newPinObject = newPin;// PhotonNetwork.GetPhotonView(newPinID).gameObject;

        newPinObject.name = "ErasablePin";
        //newPinObject.AddComponent<HighlightChangeMaterial>();

        if (parent != null)
        {
            newPinObject.transform.parent = parent.transform;// PhotonNetwork.GetPhotonView(parentID).gameObject.transform;
        }
    }

    public static void DestroyPin(GameObject pin)
    {
        GameObject.Destroy(pin);
        //FAKEPHOT PhotonNetwork.Destroy(pin);
    }

    // Update is called once per frame
    void Update()
    {
        /*FAKEPHOT
        if (PhotonSimulatorIuli.Instance.USING_REAL_PHOTON && 
            !photonView.IsMine)
        {
            // ignore pen tips that aren't mine
            return;
        }
        */

        if (intervalStart)
        {
            IntervalTimer();
        }

        if (isDestroyed)
        {
            isDestroyed = false;
            intervalStart = true;

        }

        if (touchingOtherPin == false)
        {
            if (Input.GetKeyDown(KeyCode.Space))  //(penTipManager.GetInputMouseButton1Down) //Input.GetKeyDown(KeyCode.Space)) // if (OVRInput.GetDown(OVRInput.Button.One))
            {
                DropPin(pinPrefab, this.transform.position, onAttachable ? pinParent.transform : null);
            }
        }
          
        
    }



    void OnTriggerStay(Collider other)
    {
        /*FAKEPHOT
        if (PhotonSimulatorIuli.Instance.USING_REAL_PHOTON
            && !photonView.IsMine)
        {
            // ignore pen tips that aren't mine
            return;
        }
        */

        if (other.name == "ErasablePin")
        {

            touchingOtherPin = true;
            //Debug.Log("PIN: " + touchingOtherPin);

            if (Input.GetKeyDown(KeyCode.Space)) // if (OVRInput.GetDown(OVRInput.Button.One))
            {
                Debug.Log("DESTROY");
                
                DestroyPin(other.gameObject);
                
                isDestroyed = true;
            }

        }

        if (other.tag == "Attachable")
        {
            onAttachable = true;
            pinParent = other.gameObject;

        }

    }

    private void OnTriggerExit(Collider other)
    {
        /*FAKEPHOT
        if (PhotonSimulatorIuli.Instance.USING_REAL_PHOTON
            && !photonView.IsMine)
        {
            // ignore pen tips that aren't mine
            return;
        }
        */

        touchingOtherPin = false;

        if (other.tag == "Attachable")
        {
            onAttachable = false;
        }

    }


    public void IntervalTimer()
    {
        intervalTimer += Time.deltaTime;
        if (intervalTimer >= 0.05f)
        {
            touchingOtherPin = false;


            intervalTimer = 0.0f;
            intervalStart = false;
        }

    }


}
