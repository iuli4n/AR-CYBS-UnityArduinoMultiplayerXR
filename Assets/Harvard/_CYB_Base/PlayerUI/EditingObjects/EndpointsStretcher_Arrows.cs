using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndpointsStretcher_Arrows : EndpointsStretcher
{
    public GameObject arrowObject;

    public Vector3 worldStartPoint, worldEndPoint;

    public override void MoveStartPoint(Vector3 pos)
    {
        base.MoveStartPoint(pos);
        worldStartPoint = pos;
    }
    public override void MoveEndPoint(Vector3 pos)
    {
        base.MoveEndPoint(pos);
        worldEndPoint = pos;

        // Now rotate the whole object
        Vector3 se = (worldEndPoint - worldStartPoint) / 2;
        arrowObject.transform.position = worldStartPoint + se;
        arrowObject.transform.localScale = Vector3.one * se.magnitude * 2f * 0.1f;
        // TODO: Forward of arrow isn't actually forward; will need to be locally rotated
        arrowObject.transform.LookAt(worldEndPoint);
        arrowObject.transform.localRotation *= Quaternion.Euler(0, -90f, 0);

    }
    public override void DoneEditing()
    {
        //base.DoneEditing();

        if (destroyEndpointsAfterEditing)
        {
            // TODO:IR: should be an RPC so that everyone destroys them... or should the endpoints just appear for local users when hand gets close?
            GameObject.Destroy(startPoint.gameObject);
            GameObject.Destroy(endPoint.gameObject);

            StartCoroutine(RefreshBounds());
            //GameObject.Destroy(this /*component*/);
        }
        
    }

    IEnumerator RefreshBounds()
    {
        yield return new WaitForSeconds(0.01f);
        arrowObject.GetComponent<BoundsControl>().enabled = false;
        yield return new WaitForSeconds(0.01f);
        arrowObject.GetComponent<BoundsControl>().enabled = true;
        GameObject.Destroy(this);
    }
}
