using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EP_HistoricalDelay_Menu : AEffect4PluginMenu
{
    public GameObject myMenu;

    bool activated = false;

    override public void OpenMenu(Transform parentLocation)
    {
        myMenu.transform.parent = parentLocation;
        myMenu.transform.localPosition = Vector3.zero;
        myMenu.SetActive(true);
    }
    override public void HideMenu()
    {
        myMenu.SetActive(false);
    }
    override  public void Refresh(float currentSensorValue)
    {
        // should update the UI to match the current model but we have nothing to do
    }

    public void Activate()
    {
        activated = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
