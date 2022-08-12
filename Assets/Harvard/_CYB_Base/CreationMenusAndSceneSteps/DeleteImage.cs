using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteImage : MonoBehaviour
{
    //deletes the parent image
    private GameObject parentImage;
    private void Start()
    {
        parentImage = this.transform.parent.gameObject;
    }
    public void DeleteParentImage()
    {
        Destroy(parentImage);
    }
}
