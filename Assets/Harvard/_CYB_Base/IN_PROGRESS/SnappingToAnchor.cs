using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnappingToAnchor : MonoBehaviour
{
    public GameObject[] anchors;

    public GameObject[] snapPoints;

    void Start()
    {
        
    }

    void Update()
    {
        anchors = GameObject.FindGameObjectsWithTag("SnapAnchor");
        
        foreach (var anchor in anchors) {

            foreach (var snap in snapPoints)
            {
                if ((snap.transform.position - anchor.transform.position).magnitude < 0.1f)
                {
                    Quaternion roffset = this.gameObject.transform.rotation * Quaternion.Inverse(snap.transform.rotation);
                    //Quaternion.FromToRotation(this.gameObject.transform.rotation.eulerAngles, snap.transform.rotation.eulerAngles);
                    this.gameObject.transform.rotation =  roffset * anchor.transform.rotation;

                    Vector3 poffset = snap.transform.position - this.gameObject.transform.position;
                    this.gameObject.transform.position = anchor.transform.position - poffset;


                        //Quaternion.FromToRotation(anchor.transform.rotation.eulerAngles, roffset.eulerAngles) * anchor.transform.rotation;
                    return;
                }
            }
        }
    }
}
