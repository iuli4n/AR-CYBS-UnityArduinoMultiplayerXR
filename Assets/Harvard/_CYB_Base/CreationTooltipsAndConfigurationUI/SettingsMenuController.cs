using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;

public enum SettingsType { DataChannel, RenameTooltip, MoveTooltipAnchor, Other }

[Serializable]
public class SettingsMenuButton
{
    public SettingsType type;
    public string text;
}

public class SettingsMenuController : MonoBehaviour
{

    [SerializeField]
    public GameObject settingsMenuprefab;
    [SerializeField]
    public GameObject settingsMenuLaunchButtonprefab;
    [SerializeField]
    public GameObject settingsMenuButtonprefab;
    GameObject menu;
    public GameObject menuLaunchButton;

    public bool showingMenu;
    public bool menuPressedToggle;
    [SerializeField]
    public SettingsMenuButton[] buttons;
    int numButtonRows;
    int numButtonCols;
    public int numberOfRows;
    public int numberOfColumns;

    public GameObject backplate;

    // format buttons
    public GameObject buttonCollection;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 settingsLaunchButtonPosition = transform.position + new Vector3(0f, 0f, -0.15f);
        menuLaunchButton = Instantiate(settingsMenuLaunchButtonprefab, settingsLaunchButtonPosition, transform.rotation);
        menuLaunchButton.transform.parent = transform;
        menuLaunchButton.AddComponent<Billboard>();
        var interactable = menuLaunchButton.GetComponent<Interactable>();
        interactable.OnClick.AddListener(ToggleSettingsMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleSettingsMenu()
    {
        if (!showingMenu)
        {
            menu = Instantiate(settingsMenuprefab, Vector3.zero, Quaternion.identity);
            buttonCollection = menu.transform.Find("ButtonCollection").gameObject;
            //menu.transform.localPosition += 0.1f*Vector3.down;
            FormatMenu();
            FormatButtons();
            showingMenu = true;
        }
        else
        {
            Destroy(menu);
            showingMenu = false;
        }
    }

    void FormatMenu()
    {
        //position the menu to the left of the Settings button
        menu.transform.position = transform.Find("SettingsMenuPosition").position;//transform.position - new Vector3(0f, .1f, 0f);
        //menu.transform.rotation = transform.rotation;
        //menu.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
        menu.transform.localScale = new Vector3(4, 3, 2);
        menu.transform.parent = transform;

        // instantiate backplate
        backplate = menu.transform.Find("Backplate").gameObject;

        // calculate backplate scale
        numButtonRows = 1 + ((int)((buttons.Length - 1) / 4));
        numButtonCols = Mathf.Min(buttons.Length, 4);
        if (numberOfRows != 0)
        {
            numButtonRows = numberOfRows;
        }
        if (numberOfColumns != 0)
        {
            numButtonCols = numberOfColumns;
        }
        float newYscale = numButtonRows * backplate.transform.localScale.y;      
        float newXscale = numButtonCols * backplate.transform.localScale.x;
        backplate.transform.localScale = new Vector3(newXscale, newYscale, backplate.transform.localScale.z);

        // calculate backplate shift
        float newXpos = 0.02f * (numButtonCols - 1);
        float newYpos = 0.02f + 0.02f * (numButtonRows - 1);
        backplate.transform.localPosition = new Vector3(newXpos, newYpos, backplate.transform.localPosition.z);
    }

    void FormatButtons()
    {
        
        buttonCollection.transform.localPosition = new Vector3(0f, .02f * numButtonRows, -.01f);

        // create button GameObjects on the menu. These should be dynamically created based on the buttons defined in the inspector
        for (int i = 0; i < buttons.Length; i++)
        {
            // attach button as child object of the button collection on the menu
            // dynamically position buttons on the menu
            GameObject newButton = Instantiate(settingsMenuButtonprefab, Vector3.zero, Quaternion.identity);
            newButton.transform.parent = buttonCollection.transform;
            newButton.transform.localPosition = new Vector3(.04f * (i % numButtonCols), -.04f * ((int)(i / numButtonCols)), 0f);
            newButton.transform.localRotation = Quaternion.Euler(Vector3.zero);
            newButton.transform.localScale = new Vector3(1, 1, 1);
            // fill in the text displayed on the button
            GameObject iconandtext = newButton.transform.Find("IconAndText").gameObject;
            TextMeshPro textmesh = iconandtext.transform.Find("TextMeshPro").GetComponent<TextMeshPro>();

            // assign function to the button
            SettingsMenuButton t = buttons[i];
            SettingsMenuButtonController buttonController = newButton.GetComponent<SettingsMenuButtonController>();
            buttonController.SetButtonType(t, transform);
            buttonController.settingsMenuController = this;

            if (t.type == SettingsType.DataChannel)
            {
                textmesh.fontSize = .06f;
                textmesh.SetText("CHANNEL: " + t.text);
            }
            else if (t.type == SettingsType.RenameTooltip)
            {
                // fill in the text displayed on the button
                textmesh.SetText("RENAME");
                textmesh.fontSize = .06f;
            }
            else if (t.type == SettingsType.MoveTooltipAnchor)
            {
                // fill in the text displayed on the button
                textmesh.SetText("ANCHOR");
                textmesh.fontSize = .06f;
            }
        }
    }
}
