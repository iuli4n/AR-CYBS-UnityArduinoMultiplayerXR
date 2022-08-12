using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!COLLIDED WITH:" + other.name);
        Destroy(other.gameObject); 
    }
}
