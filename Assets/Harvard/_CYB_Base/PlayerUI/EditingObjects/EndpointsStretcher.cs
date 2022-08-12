using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndpointsStretcher : MonoBehaviour
{
    public GameObject startPoint;
    public GameObject endPoint;

    public bool destroyEndpointsAfterEditing = false;

    public virtual void MoveStartPoint(Vector3 pos)
    {
        startPoint.transform.position = pos;
    }
    public virtual void MoveEndPoint(Vector3 pos)
    {
        endPoint.transform.position = pos;
    }
    public virtual void DoneEditing()
    {
        if (destroyEndpointsAfterEditing)
        {
            // TODO:IR: should be an RPC so that everyone destroys them... or should the endpoints just appear for local users when hand gets close?
            GameObject.Destroy(startPoint.gameObject);
            GameObject.Destroy(endPoint.gameObject);
            GameObject.Destroy(this /*component*/);
        }
    }
}
