using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// TODO: RENAME THIS. This is just used for the menu
public class SceneManagerScript : MonoBehaviour
{
    private int sceneNumber;
    public TMP_Text sceneText;


    void Start()
    {
        sceneNumber = 0;
        //sceneText = GameObject.Find("sceneText").GetComponent<TextMeshProUGUI>();
        sceneText.text = "SCENE  " + sceneNumber;

    }
    public void NextButtonPress()
    {
        SceneStepsManager.Instance.Activate_MoveToNextScene();
        //Switch to the next scene
        sceneNumber++;
        UpdateText();
    }


    public void PrevButtonPress()
    {
        //Switch to the previous scene
        SceneStepsManager.Instance.Activate_MoveToPrevScene();
        if (sceneNumber > 0)
        {
            sceneNumber--;
            UpdateText();
        }
    }

    public void UpdateText()
    {
        sceneText.text = "SCENE  " + sceneNumber;
    }
}
