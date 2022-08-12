using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseEffectsMenu : MonoBehaviour
{
    // Start is called before the first frame update

    EffectsMenu effectsMenu;
    void Start()
    {
        effectsMenu = GameObject.Find("EffectsMenuManager").GetComponent<EffectsMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseMenu()
    {
        effectsMenu.ToggleEffectsMenu();
    }
}
