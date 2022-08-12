using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// calls the related function when a button is pressed
public class ShortcutManager : MonoBehaviour
{
    public AtomicDataModel myRotValModel;

    public GameObject shortcuttButton;
    public GameObject shortcutLocationPlaceHolder; // an empty object for determining the next shortcut button location

    //private GameObject newButton;
    private Vector3 newLinePos;
    private Vector3 buttonPos;
    private Vector3 startPos;
    private int newButtonID;
    private string buttonID;
    List<GameObject> buttonList;

    private void Start()
    {
       

        buttonList = new List<GameObject>();
        newButtonID = 0;
        
        buttonPos = shortcutLocationPlaceHolder.transform.position;
        startPos = buttonPos;
        newLinePos = buttonPos;
    }

    // instantiates a shortcut prefab at the shortcut button location and adds the new prefab to the button list
    public void PlusButtonPress()
    {
        GameObject newButton = PhotonNetwork.Instantiate("shortcutButton", buttonPos, Quaternion.identity, 0);
        //GameObject newButton = (GameObject)Instantiate(shortcuttButton, buttonPos, Quaternion.identity);
        newButton.name = newButtonID.ToString();
        newButtonID++;
        newButton.transform.parent = shortcutLocationPlaceHolder.transform;

        if (newButton != null)
        {
            buttonList.Add(newButton);
        }


        ShortcutButtonScript newButtonScript = newButton.GetComponentInChildren<ShortcutButtonScript>();
        newButtonScript.myShortcutManager = this;
        newButtonScript.shortcutValue = (int)myRotValModel.Value;

        ChangePosValues();
    }

    // gets rotation value from the ShortcutButtonScript that is attached to the selected button and updates the rotation value 
    public void ShortcutButtonPress(int shortcutValue)
    {
        myRotValModel.Value = (float)shortcutValue;
    }

    // gets the button ID from DeleteShortcut script and destroys the button object
    public void DeleteButtonPress(string buttonID)
    {
        GameObject selectedbutton = GameObject.Find(buttonID);
        buttonList.Remove(selectedbutton);
        PhotonNetwork.Destroy(selectedbutton);
        //Destroy(selectedbutton);
        RearrangeButtons();
    }

    public void RearrangeButtons()
    {
        buttonPos = startPos;
        newLinePos = startPos;
        buttonList[0].transform.position = buttonPos;

        if(buttonList.Count > 0)
        {
            for (int i = 1; i < buttonList.Count; i++)
            {
                if (i % 3 != 0)
                {
                    buttonPos += Vector3.right * .1f;
                    buttonList[i].transform.position = buttonPos;
                }

                if (i % 3 == 0)
                {
                    buttonPos = newLinePos + Vector3.down * .15f;
                    newLinePos = buttonPos;
                    buttonList[i].transform.position = newLinePos;
                }
            }

            ChangePosValues();
            
        }

    }

    public void ChangePosValues()
    {
        if (buttonList.Count % 3 != 0)
        {
            buttonPos += Vector3.right * .1f;
        }

        if (buttonList.Count % 3 == 0)
        {
            buttonPos = newLinePos + Vector3.down * .15f;
            newLinePos = buttonPos;
        }
    }
}