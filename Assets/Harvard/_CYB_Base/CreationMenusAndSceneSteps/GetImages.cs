using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetImages : MonoBehaviour
{
    //public GameObject plane_;

    public Texture holo1;
    public Material imageMat;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("*************************                  PRESSED");
            //Texture2D tex = new Texture2D(2, 2);
            //tex.LoadImage(holo1.bytes);
            //buttonMat.SetTexture("Holo1", holo1);

            GetComponent<Renderer>().material = imageMat;
        } 
    }
}
