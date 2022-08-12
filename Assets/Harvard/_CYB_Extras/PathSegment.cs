using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSegment : MonoBehaviour
{
    //public bool fakeData = false;

    List<Vector3> pointList;
    int numSegments;
    int oldNumSegments;

    public GameObject elementPrefab;

    public Vector3 startPos, endPos;

    public int NUMELEMENTS = 10;
    int oldNumElements;
    public GameObject[,] myElements;

    public float timeOffset = 0;
    public float timeOffsetSpeed = 0.1f;

    public bool directionFlag = true;
    public void SetSpeed(float speed)
    {
        timeOffsetSpeed = speed;
    }

    public void SetNumSegments(int num)
    {
        numSegments = num;
    }

    public void SetPointList(List<Vector3> l)
    {
        pointList = l;
    }

    void MoveAll()
    {
        for (int j = 0; j < numSegments; j++)
        {
            for (int i = 0; i < NUMELEMENTS; i++)
            {
                float timeOffsetNormalized = timeOffset - Mathf.Floor(timeOffset);
                if (!directionFlag) timeOffsetNormalized = 1 - timeOffsetNormalized;

                float lo = GetLerpOffset(timeOffsetNormalized, i, NUMELEMENTS);
                myElements[j,i].transform.position = GetPosition(lo, j);
                myElements[j,i].transform.localScale = GetLocalScale(lo);
            }
        }
        
    }

    float GetLerpOffset(float timeOffset, int elementIndex, int NUMELEMENTS)
    {
        float lerpOffset = 1f * elementIndex / NUMELEMENTS;
        lerpOffset = (lerpOffset + timeOffset);
        if (lerpOffset > 1) lerpOffset = lerpOffset - Mathf.Floor(lerpOffset);
        return lerpOffset;
    }
    
    Vector3 GetPosition(float lerpOffset, int segmentIndex)
    {
        //Vector3 path = pointList[segmentIndex+1] - pointList[segmentIndex];
        //float speed = path.magnitude
        return Vector3.Lerp(pointList[segmentIndex], pointList[segmentIndex+1], lerpOffset);
    }
    Vector3 GetLocalScale(float lerpOffset)
    {
        return elementPrefab.transform.localScale;
        //return Vector3.Lerp(startPos.localScale, endPos.localScale, lerpOffset);
        //return Quaternion.Lerp(startPos.localRotation, endPos.localRotation, lerpOffset);
    }
    // Start is called before the first frame update
    void Start()
    {
        ResetMyElementArray();
        MoveAll();

        /**
        if (fakeData)
        {

        }
        else
        {
            // connect event listener
            GameObject.Find("DebugGUI").GetComponent<DebugGUI>().model.OnSensorUpdated += SensorChangedEventHandler;
        }
        ***/
    }


    public void SensorChangedEventHandler(float newValue)
    {
        SetSpeed(newValue);
    }

    public void ResetMyElementArray()
    {
        bool needToDestroy = (myElements != null);
        if (myElements == null || myElements.GetLength(0) != numSegments || myElements.GetLength(1) != NUMELEMENTS)
        {            
            if (needToDestroy)
            {
                // destroy existing balls
                for (int j = 0; j < oldNumSegments; j++)
                {
                    for (int i = 0; i < oldNumElements; i++)
                    {
                        Destroy(myElements[j, i]);                        
                    }
                }
            }

            myElements = new GameObject[numSegments, NUMELEMENTS];

            Transform elementsParent = this.gameObject.transform.Find("ParticlesParent");
            Debug.Assert(elementsParent, "Couldn't find child, was it renamed?");

            foreach (Transform t in elementsParent)
            {
                GameObject.Destroy(t.gameObject);
            }

            // initialize balls
            for (int j = 0; j < numSegments; j++)
            {
                for (int i = 0; i < NUMELEMENTS; i++)
                {
                    if (needToDestroy)
                    {
                        Destroy(myElements[j, i]);
                    }
                    GameObject e = GameObject.Instantiate(elementPrefab, elementsParent);
                    e.SetActive(true);
                    myElements[j, i] = e;
                }
            }
            elementPrefab.SetActive(false);

            oldNumElements = NUMELEMENTS;
            oldNumSegments = numSegments;
        }
    }


    float nextTickTime;
    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextTickTime)
        {
            nextTickTime = Time.time + 0.1f;
            DelayedUpdate();
        }
    }
    void DelayedUpdate()
    {
        ResetMyElementArray();
        MoveAll();
        timeOffset += timeOffsetSpeed * 1;
    }
}
