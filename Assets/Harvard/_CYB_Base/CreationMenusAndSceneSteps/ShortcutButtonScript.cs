using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShortcutButtonScript : MonoBehaviour
{
    // this script updates the UI text on the knob
    public ShortcutManager myShortcutManager;
    public int shortcutValue;
    private TextMeshProUGUI rotValText;

    private void Start()
    {
        rotValText = GetComponent<TextMeshProUGUI>();
        rotValText.text = shortcutValue.ToString();
    }

    public void ButtonPress()
    {
        myShortcutManager.ShortcutButtonPress(shortcutValue);
    }
}
