using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Updates the user's hand menu to match what tool they're currently got selected
public class MenuView_CurrentToolSelection : MonoBehaviour
{
    public Material menuOnMat, menuOffMat;
    
    public ObjectMenuFinder objectMenuFinder;
    public PenTipModel tipModel;

    public MeshRenderer buttonDraw, buttonPoint1, buttonPoint2;
    public MeshRenderer buttonObjectMenu;

    // Start is called before the first frame update
    void Start()
    {
        tipModel.modelChangedLastTriggerEntered += TipModel_modelChangedLastTriggerEntered;
    }

    private void TipModel_modelChangedLastTriggerEntered(string value)
    {
        UpdateAllVisuals();
    }

    // TODO: Should only be called by the hand menu when it's shown or hidden and/or when models change
    public void UpdateAllVisuals()
    {
        bool objectMenuVisible = objectMenuFinder.IsOpened();
        buttonObjectMenu.material = (objectMenuVisible ? menuOnMat : menuOffMat);
        //Debug.Log("Menu on material: " + objectMenuVisible + " " + buttonObjectMenu.material);

        string value = tipModel._lastTriggerEntered;
        buttonDraw.material = (value == "Draw" ? menuOnMat : menuOffMat);
        buttonPoint1.material = (value == "Point1" ? menuOnMat : menuOffMat);
        buttonPoint2.material = (value == "Point3" ? menuOnMat : menuOffMat);
    }

    float nextUpdate = 0;

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Menu material: " + buttonObjectMenu.material);

        if (Time.time < nextUpdate)
            return;

        nextUpdate = Time.time + 0.5f;

        UpdateAllVisuals();
    }
}
