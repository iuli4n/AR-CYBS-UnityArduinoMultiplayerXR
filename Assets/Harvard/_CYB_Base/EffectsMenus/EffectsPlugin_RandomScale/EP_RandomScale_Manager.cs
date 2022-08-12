using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EP_RandomScale_Manager : AEffect4PluginManager
{
    [SerializeField]
    private EP_RandomScale_Menu myMenu;
    [SerializeField] 
    private EP_RandomScale_Model myModel;

    bool menuGUIOn = false;
    bool savedMenuGUIOn = false;

    override public AEffect4PluginModel GetModel() { return myModel; }
    override public AEffect4PluginMenu GetMenu() { return myMenu; }



    override public void OnCreated()
    {
        // nothing to do
    }
    override public void OnEnable()
    {
        // nothing to do
    }
    override public void OnDisable()
    {
        // nothing to do
    }

    public override void ToggleMenuGUI()
    {
        menuGUIOn = !menuGUIOn;
    }

    public override string GetPluginName()
    {
        return "Random Scale";
    }



    // Start is called before the first frame update
    void Start()
    {

        // TODO: configure the menu to update automatically when the model changes       
    }

    // Update is called once per frame
    void Update()
    {
        if (savedMenuGUIOn != menuGUIOn)
        {
            savedMenuGUIOn = menuGUIOn;
            if (menuGUIOn) {
                myMenu.Activate();
                myMenu.OpenMenu(this.transform); 
            } 
            else { myMenu.HideMenu(); }
        }
    }





    override public void RespondToNewValue(
        float currentSensorValue, float oldSensorValue,
        Vector3 baseLocalPos, Quaternion baseLocalRot, Vector3 baseLocalScale,
        ref float newSensorValue,
        ref Vector3 newLocalPos, ref Quaternion newLocalRot, ref Vector3 newLocalScale)
    {
        // this plugin just randomizes the current object's scale a bit, influenced by the sensor and the local configuration

        if (!myModel.isEnabled)
            return;

        // first get a random value whose magnitude is influenced by the current sensor value
        float rand = Random.Range(0, currentSensorValue / 1000f);
        // and then magnify that by our randomMagnitude
        rand = rand * myModel.randomMagnitude;

        // now update the new local scale (this changes the external value supplied through ref Vector3 newLocalScale)
        newLocalScale = baseLocalScale * rand;
    }

    public void RespondToMenuSliderValue()
    {
        float v = myMenu.slider.SliderValue * 5;
        myModel.randomMagnitude = v;
        myMenu.SetValText(v);
    }

    public void RespondToMenuToggle()
    {
        myMenu.Toggle();
        myModel.isEnabled = !myModel.isEnabled;
    }

    
}
