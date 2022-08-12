using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPulse : MonoBehaviour
{
    public GameObject wave;
    public GameObject transmitter;

    void OnEnable()
    {
        StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine()
    {
        transmitter.GetComponent<Renderer>().material.color = new Color(1f, 129f / 255f, 0);
        yield return new WaitForSeconds(0.1f);
        transmitter.GetComponent<Renderer>().material.color = Color.white;


        var go = Instantiate(wave, transmitter.transform.position + transform.TransformVector(Vector3.forward * 2f), this.transform.rotation);
        go.transform.parent = this.transform;
        go.transform.localScale = wave.transform.localScale;
        yield return new WaitForSeconds(0.7f);

        StartCoroutine(FlashCoroutine());
    }
}
