using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EP_Path_Model : AEffect4PluginModel
{
    public bool isEnabled = false;
    public bool rotateObjectToDirectionOfPath;
    public bool rotateObjectBetweenInterpolatedRots;
    public List<Vector3> pointList;
    public List<Vector3> rotList;

    private void Start()
    {

    }

    override public void DoPhotonSerialize(bool isWriting, PhotonStream stream)
    {
        if (isWriting)
        {
            stream.SendNext(isEnabled);
            stream.SendNext(rotateObjectToDirectionOfPath);
            stream.SendNext(rotateObjectBetweenInterpolatedRots);

            stream.SendNext(pointList.Count);
            foreach(Vector3 point in pointList)
            {
                stream.SendNext(point.x);
                stream.SendNext(point.y);
                stream.SendNext(point.z);
            }
            foreach (Vector3 point in rotList)
            {
                stream.SendNext(point.x);
                stream.SendNext(point.y);
                stream.SendNext(point.z);
            }
        }
        else
        {
            isEnabled = (bool)stream.ReceiveNext();
            rotateObjectToDirectionOfPath = (bool)stream.ReceiveNext();
            rotateObjectBetweenInterpolatedRots = (bool)stream.ReceiveNext();

            pointList = new List<Vector3>();
            int numPoints = (int)stream.ReceiveNext();
            for (int i = 0; i < numPoints; i++)
            {
                float x = (float)stream.ReceiveNext();
                float y = (float)stream.ReceiveNext();
                float z = (float)stream.ReceiveNext();
                pointList.Add(new Vector3(x, y, z));
            }
            rotList = new List<Vector3>();
            int numRots = (int)stream.ReceiveNext();
            for (int i = 0; i < numRots; i++)
            {
                float x = (float)stream.ReceiveNext();
                float y = (float)stream.ReceiveNext();
                float z = (float)stream.ReceiveNext();
                rotList.Add(new Vector3(x, y, z));
            }
        }
    }
}
