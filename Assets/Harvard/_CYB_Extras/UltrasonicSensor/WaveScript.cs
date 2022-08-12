using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveScript : MonoBehaviour
{
    public float speed;
    private float targetTime;

    void Start()
    {
        speed = 0.5f;
        targetTime = 4f;
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        targetTime -= Time.deltaTime;
        if (targetTime <= 0.0f)
        {
            timerEnded();
        }
    }

    void timerEnded()
    {
        if(this != null)
        {
            Destroy(gameObject);
        }
    }

    bool bouncedOnce = false;

    void OnTriggerEnter(Collider other)
    {

        if(other.tag == "Sensor")
        {
            other.GetComponent<Renderer>().material.color = Color.green;
            StartCoroutine(FlashCoroutine());
          
        } else if (other.tag == "Wave")
        {
            // do nothing
        }

        else
        {
            if (!bouncedOnce)
            {
                this.GetComponent<Renderer>().material.color = Color.green;
                this.transform.Rotate(-180f, 0f, 0f);
                bouncedOnce = true;
            }
        }

        IEnumerator FlashCoroutine()
        {

            this.GetComponent<MeshRenderer>().enabled = false;

            yield return new WaitForSeconds(0.1f);
            other.GetComponent<Renderer>().material.color = Color.white;

            if (this != null)
            {
                Destroy(gameObject);
            }
        }

    }

    public void ContactWithHand()
    {
        this.GetComponent<Renderer>().material.color = Color.green;
        this.transform.Rotate(-180f, 0f, 0f);
    }
}
