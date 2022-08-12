using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToVector3Model : MonoBehaviour
{
    public GameObject moveThis;

    [SerializeField]
    AtomicDataModelVector3 v3model;
    [SerializeField]
    AtomicDataModelBool enablemodel;

    
    void Awake()
    {
        v3model = this.GetComponent<AtomicDataModelVector3>();
        enablemodel = this.GetComponent<AtomicDataModelBool>();
    }

    void Update()
    {
        if (!enablemodel)
        {
            return;
        }

        if (v3model.photonView.AmOwner)
        {
            v3model.Value = moveThis.transform.position;
            //Debug.Log("m2vm: setting model to my position "+v3model.Value);

        }
        else
        {
            //Debug.Log("m2vm: getting model into my position " + v3model.Value);

            moveThis.transform.position = v3model.Value;
        }
    }
}
