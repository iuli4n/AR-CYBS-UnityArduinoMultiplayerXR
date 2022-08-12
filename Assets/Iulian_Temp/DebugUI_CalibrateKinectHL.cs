using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI_CalibrateKinectHL : MonoBehaviour
{
    public GameObject mrtk_playspace;
    public GameObject hl_fingertip;
    public GameObject virtual_point;

    public string calibrationKey = "z";
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(calibrationKey))
        {
            if (hl_fingertip == null)
            {
                hl_fingertip = PlayersManager.Instance.localPlayerHead.transform.Find("FingertipCollider").gameObject;
            }

            Vector3 v1 = virtual_point.transform.forward;
            Vector3 v2 = hl_fingertip.transform.forward;
            Debug.Log(v1 + "  //  " + v2);
            v1.y = 0;
            v2.y = 0;
            Quaternion rotationVector = Quaternion.FromToRotation(v2, v1);
            Debug.Log(rotationVector);
            mrtk_playspace.transform.rotation *= rotationVector;


            Vector3 moveVector = virtual_point.transform.position - hl_fingertip.transform.position;
            mrtk_playspace.transform.position += moveVector;

            
        }
    }
}
