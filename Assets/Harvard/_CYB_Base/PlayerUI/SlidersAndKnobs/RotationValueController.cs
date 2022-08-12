using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RotationValueController : MonoBehaviour
{
    // updates the rotation value model when the knob is rotating
    public AtomicDataModel myRotValModel;
    public PhotonView pv;
    public GameObject knob;
    bool isRotating;

    [SerializeField]
    float eulerAngZ;

    public int initialAngle;

    private void Start()
    {
        if (initialAngle != 0)
        {
            StartCoroutine(DelayedInitialize());
        }
    }
    IEnumerator DelayedInitialize()
    {
        yield return new WaitForSeconds(1);

        KnobRotStart();
        myRotValModel.Value = (int)initialAngle;
        KnobRotStop();
    }

    void Update()
    {
        if(isRotating)
        {
            eulerAngZ = knob.transform.localEulerAngles.z;
            myRotValModel.Value = (int)eulerAngZ;
        }

    }

    // bounds control events call these functions when the interaction is started and ended
    public void KnobRotStart()
    {
        isRotating = true;
        if (!pv.AmOwner)
            pv.RequestOwnership();
    }

    public void KnobRotStop()
    {
        isRotating = false;
    }
}
