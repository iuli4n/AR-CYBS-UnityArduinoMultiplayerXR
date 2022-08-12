
//using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActivePenTip : MonoBehaviour
{
    public PenTipModel penModel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "DrawBtn" ||
            other.name == "Pointer1" ||
            other.name == "Pointer3" ||
            other.name == "NothingBtn"
            )
        {
            penModel.lastTriggerEntered = other.name;
        }
    }
}


/****   OLD STUFF ****************************************************
//using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActivePenTip : MonoBehaviour
{
    public GameObject newDrawingParent;

    public GameObject drawingParentPrefab;
    private Vector3 drawingParentPos;


    public Color colorPicked;
    public GameObject micPinPrefab;

    public Transform parentTransform;

    private bool isInstantiated;


    // Start is called before the first frame update
    void Start()
    {
        
        colorPicked = Color.white;
        drawingParentPos = new Vector3(-26.79f, 0.8f, 10.85f);

    }

    // Update is called once per frame
    void Update()
    {
        //isInstantiated = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "DrawBtn")
        {
            for (int j = 0; j < parentTransform.childCount; j++)
            {
               parentTransform.GetChild(j).gameObject.SetActive(false);
            }

            GameObject drawTip = parentTransform.GetChild(0).gameObject;
            drawTip.SetActive(true);

            if(!isInstantiated)
            {
                newDrawingParent = Instantiate(drawingParentPrefab, drawingParentPos, Quaternion.identity);
                newDrawingParent.name = "DrawingParentObj";

                newDrawingParent.GetComponent<MeshRenderer>().enabled = false;
                isInstantiated = true;
            }
            

        }

        if (other.name == "PinBtn")
        {
            for (int j = 0; j < parentTransform.childCount; j++)
            {
                parentTransform.GetChild(j).gameObject.SetActive(false);
            }

            GameObject pinTip = parentTransform.GetChild(1).gameObject;
            pinTip.SetActive(true);
        }

        if (other.name == "MicBtn")
        {
            for (int j = 0; j < parentTransform.childCount; j++)
            {
                parentTransform.GetChild(j).gameObject.SetActive(false);
            }

            GameObject micTip = parentTransform.GetChild(2).gameObject;
            micTip.SetActive(true);

            micTip.GetComponent<DropMic>().micDetected = false;


        }

        if (other.name == "EraserBtn")
        {
            for (int j = 0; j < parentTransform.childCount; j++)
            {
                parentTransform.GetChild(j).gameObject.SetActive(false);
            }

            GameObject eraseTip = parentTransform.GetChild(3).gameObject;
            eraseTip.SetActive(true);

            if(isInstantiated)
            {

                newDrawingParent.GetComponent<MeshRenderer>().enabled = true;
                isInstantiated = false;
            }
         

        }


        if (other.name == "Yellow")
        {
            colorPicked = Color.yellow;
        }

        if (other.name == "Red")
        {
            colorPicked = Color.red;
        }

        if (other.name == "Blue")
        {
            colorPicked = Color.blue;
        }

    }
}
****************************/