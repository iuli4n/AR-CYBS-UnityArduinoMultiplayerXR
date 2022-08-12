using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP_Debug_DemoSequenceTools : MonoBehaviour
{
    public static TEMP_Debug_DemoSequenceTools Instance;

    public bool Override_DontAttachToScene = true;
    public bool Override_DontEffect4Gen = true;

    public GameObject[] toolsInSequence;
    public int currentStep = -1;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    void NextStep()
    {
        if (currentStep>-1)
        {
            toolsInSequence[currentStep].SetActive(false);
        }
        currentStep++;
        if (toolsInSequence.Length>currentStep)
        {
            toolsInSequence[currentStep].SetActive(true);

            if (toolsInSequence[currentStep].GetComponent<Tool_Creator2Part>())
            {
                toolsInSequence[currentStep].GetComponent<Tool_Creator2Part>().SetPlacementCompleteCallback(NextStep);
            }
            if (toolsInSequence[currentStep].GetComponent<Tool_Creator1Part>())
            {
                toolsInSequence[currentStep].GetComponent<Tool_Creator1Part>().OnPlacementComplete = (NextStep);
            }
        }
    }

    public void BeginSequence()
    {
        NextStep();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
