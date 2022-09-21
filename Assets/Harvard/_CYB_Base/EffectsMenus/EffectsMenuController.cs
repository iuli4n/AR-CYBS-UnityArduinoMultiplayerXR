using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

public class EffectsMenuController : MonoBehaviour
{
    #region define objects

    // The transform of the target object,
    // which is updated live during runtime based on which object in the scene the user most recently touched / clicked on.
    // We assume this target has at least one data switch attached to it.
    // When a new target object is assigned, the function LoadTargetComponents is called, which refreshes the menu.
    Transform target;


    // DATA

    // This is the effectsModel attached to a target object.
    // This model has boolean parameters representing whether each effect is enabled,
    // as well as float parameters representing the current value of each effect.
    // This model’s parameters are updated whenever the user toggles effects on/off
    // with buttons or updates the values of the effects with sliders.
    Effect4Model effectsModel;

    // These are the dataSwitches attached to a target object. Each target object has at least one dataSwitch,
    // the first of which is the data switch which controls the basic effects:
    // rotation, scaling, visibility toggling, and color.
    // Any additional dataSwitches are assumed to apply to additional plugins activated on the target object.
    AtomicDataSwitch[] dataSwitches;

    // This is the name of the first attached dataSwitch.
    // To improve this code, we should maintain a reference to each name of the dataSwitches,
    // and rename this variable dataSwitchNames.
    TextMeshPro dataSwitchText;

    // A property of the primary dataSwitch which refers to the channel of the effects model managing the main visual effects:
    // rotation, scaling, and visibility toggling.
    string selectedChannel = "";

    // This slider object shows the current value of the primary dataSwitch as a position along a slider
    PinchSlider indicatorpinchSlider;

    // The object containing the sensor chart. This chart, displayed in the upper right corner of the EffectsMenu,
    // shows a live-updated graph of the data over time streaming through the dataSwitch.
    SensorDataBroadcasterFromAtomic broadcaster;

    // The label gameObject which is refreshed when the data channel is updated.
    GameObject chartChannelName;

    AEffect4PluginManager[] pluginManagers;


    // The label of the currently edited object
    TextMeshPro objectNameLabel;

    // BUTTONS AND SLIDERS

    // This dictionary refers to the off-state button plate for each EffectsMenu effect.
    Dictionary<string, GameObject> backplateNormals;

    // This dictionary refers to the on-state button plate for each EffectsMenu effect.
    Dictionary<string, GameObject> backplateToggles;

    // This dictionary refers to the GameObject for each slider on the EffectsMenu.
    // We assign a slider to each effect that has a float parameter,
    // and do not assign a slider to an effect that only has a boolean parameter (e.g. VisAboveOrBelow).
    Dictionary<string, GameObject> sliderObjects;

    // This dictionary refers to the TextMeshPro to display the current value of each slider.
    Dictionary<string, GameObject> sliderLabels;

    // This dictionary refers to the PinchSlider object for each slider.
    Dictionary<string, PinchSlider> sliders;


    // MANUAL DATA CHANNEL
    // The prefab of the manualDataChannels menu that allows the user to directly adjust the dataSwitch channel and values
    public GameObject manualDataChannelPrefab;
    GameObject manualDataChannel;


    // COLOR AND DRAWING

    // The DrawingPropertiesManager, which we only get if the target object is a BaseLineDrawing object.
    // We use this object to get a reference to the available colors the target object can be displayed as.
    DrawingPropertiesManager drawingPropertiesManager;

    // The lineRenderer of the target object, which we only get if the target object is a BaseLineDrawing object.
    LineRenderer lineRenderer;

    // A string representing the current color of the target object.
    string selectedColor;

    // An array of the available colors from the DrawingPropertiesManager
    Material[] drawingColors;


    // NAVIGATION

    // The material to be displayed when a tab is not available (not critical, will render as pink otherwise)
    public Material greyMaterial;

    // The parent game object storing all the navigation gameObjects.
    GameObject nav;

    // An int representing the number of available pages on the EffectsMenu.
    // This number is 4 by default (Rotate, Scale, Visibility, Channels)
    // with a 5th Color page if the target object is a drawing object.
    int totalNumPages;

    // An int representing the page number of the tab currently visible.
    int currentPageNum;

    // A string array with the current active tabs,
    // meant to contain “Color” or not whether the target object is a drawing.
    string[] activeTabDisplayNames;

    // A string array with the current active tabs’ object names,
    // meant to contain “Color” or not whether the target object is a drawing.
    string[] activeTabObjectNames;


    // ***************************************************************************
    // ***************************************************************************

    // WHEN ADDING NEW EFFECTS MENU PAGES, ADD ITS TAB NAME FROM THE UNITY HIERARCHY HERE
    // A string array with the names of the tabs in the EffectsMenu.
    // The current values are “Color”, “Rot”, “Scale”, “Vis”, and “Cha”

    string[] allTabObjectNames = { "Color", "Rot", "Scale", "Vis", "Cha", "Plugin" };

    // WHEN ADDING NEW EFFECTS MENU PAGES, ADD A REFERENCE TO ITS GAMEOBJECT IN THE UNITY HIERARCHY HERE
    // The array of gameObjects for each page on the effects menu.
    // The current values are rotPage, scalePage, visPage, colorPage, channelsPage.
    GameObject[] pages;
    GameObject rotPage;
    GameObject scalePage;
    GameObject visPage;
    GameObject colorPage;
    GameObject channelsPage;
    GameObject pluginsPage;

    // WHEN ADDING NEW MODEL EFFECTS, ADD THEIR NAME FROM THE UNITY HIERARCHY NAME HERE
    string[] ModelEffectsToLoad =
    {
        "XRotate", 
        "YRotate", 
        "ZRotate", 
        "XScaleLeft", 
        "XScaleRight", 
        "YScaleUp", 
        "YScaleDown", 
        "ZScaleLeft", 
        "ZScaleRight", 
        "Vis", 
        "AboveOrBelow"
    };
    // ***************************************************************************
    // ***************************************************************************

    #endregion

    void Awake()
    {
        objectNameLabel = transform.Find("All/Title/Subtitle_EditedObject").gameObject.GetComponent<TextMeshPro>();

        rotPage = transform.Find("All/Rot").gameObject;
        scalePage = transform.Find("All/Scale").gameObject;
        visPage = transform.Find("All/Vis").gameObject;
        channelsPage = transform.Find("All/Cha").gameObject;
        colorPage = transform.Find("All/Color").gameObject;
        pluginsPage = transform.Find("All/Plugin").gameObject;
        // WHEN ADDING A NEW PAGE TO THE EFFECTS MENU, MAKE SURE TO FIND IT HERE IN THE UNITY HIERARCHY

        nav = transform.Find("All/EffectsMenuNav").gameObject;
        broadcaster = transform.Find("All/SensorAndBarVisualizers/_DataSource").GetComponent<SensorDataBroadcasterFromAtomic>();
        chartChannelName = transform.Find("All/SensorAndBarVisualizers/name").gameObject;
        indicatorpinchSlider = transform.Find("All/Vis/Labels/Slider").GetComponent<PinchSlider>();    

        currentPageNum = 0;    
        dataSwitchText = transform.Find("All/Vis/Labels/ValSensor").GetComponent<TextMeshPro>();
        backplateNormals = new Dictionary<string, GameObject>();
        backplateToggles = new Dictionary<string, GameObject>();
        sliderObjects = new Dictionary<string, GameObject>();
        sliderLabels = new Dictionary<string, GameObject>();
        sliders = new Dictionary<string, PinchSlider>();

        foreach (string valname in ModelEffectsToLoad)
        {
            LoadModelEffectValueDicts(valname);       
        }
        manualDataChannel = Instantiate(manualDataChannelPrefab);
        manualDataChannel.SetActive(false);

        // add clicks to channel buttons
        // can't set this in the inspector because the function takes two arguments
        for (int i = 0; i < 3; i++)
        {
            for (int j = 1; j < 6; j++)
            {
                int x = i;
                int y = j;
                transform.Find("All/Cha/" + i.ToString() + "/Buttons/Channel" + j.ToString()).GetComponent<Interactable>().OnClick.AddListener(() => ToggleChannel(x, y));
            }
        }

        // add clicks to plugin buttons
        //int numPlugins = 
        for (int i = 0; i < 3; i++)
        {
            int x = i;
            transform.Find("All/Plugin/" + i.ToString() + "/Buttons/Launch").GetComponent<Interactable>().OnClick.AddListener(() => TogglePlugin(x));
        }
    }

    void LoadModelEffectValueDicts(string valname)
    {
        string tabname = GetTab(valname);
        string modelval = GetModelVal(valname);
        backplateNormals.Add(valname, transform.Find("All/" + tabname + "/Buttons/Toggle" + valname + "/Backplate Plus Button/BackplateNormal").gameObject);
        backplateToggles.Add(valname,transform.Find("All/" + tabname + "/Buttons/Toggle" + valname + "/Backplate Plus Button/BackplateToggle").gameObject);
        if ((modelval != "AboveOrBelow")&& !sliderObjects.ContainsKey(modelval))
        {
            sliderObjects.Add(modelval, transform.Find("All/" + tabname + "/Inputs/Val" + modelval).gameObject);
            sliderLabels.Add(modelval,transform.Find("All/" + tabname + "/Labels/Val" + modelval).gameObject);
            sliders.Add(modelval, transform.Find("All/" + tabname + "/Inputs/Val" + modelval + "/Slider").GetComponent<PinchSlider>());
        }
    }
    string GetTab(string valname)
    {
        foreach (string page in allTabObjectNames)
        {
            if (valname.Contains(page)) { return page; }
        }
        if (valname == "AboveOrBelow") { return "Vis"; }
        return "None";
    }
    string GetModelVal(string valname)
    {
        string[] directions = new string[] { "Left", "Right", "Up", "Down" };
        foreach (string d in directions)
        {
            if (valname.Contains(d)) { return valname.Replace(d, ""); }
        }
        return valname;
    }

    public AtomicDataSwitch[] GetDataSwitches()
    {
        return dataSwitches;
    }

    public void SetTarget(Transform t)
    {
        UnloadTargetComponents();

        target = t;

        if (target.Find("EffectsModel") == null)
        {
            // This object actually doesn't have an EffectsModel on it (means it was somehow custom created by users and can't have effects)
            // So reset the effects menu so it can't edit anything
            target = null;
        }

        LoadTargetComponents();
    }

    public void UnloadTargetComponents()
    {
        // just unregister ourselves from the current target's data channel
        if (target == null) return;

        dataSwitches = target.GetComponentsInChildren<AtomicDataSwitch>();
        if (dataSwitches == null || dataSwitches.Length == 0) return;

        //update vis tab UI based on target's primary dataSwitch
        dataSwitches[0].OnDataUpdated -= RefreshVisTabDataSwitchValues;
    }

    public void LoadTargetComponents()
    {
        if (target != null)
        {
            objectNameLabel.text = target.name;

            #region setup data

            //get attached components from target
            effectsModel = target.Find("EffectsModel").GetComponent<Effect4Model>();
            dataSwitches = target.GetComponentsInChildren<AtomicDataSwitch>();

            //update vis tab UI based on target's primary dataSwitch
            dataSwitches[0].OnDataUpdated += RefreshVisTabDataSwitchValues;

            //toggle each dataSwitch's channel button on the channels tab
            for (int i = 0; i < dataSwitches.Length; i++)
            {
                ToggleChannel(i, int.Parse(dataSwitches[i].CurrentChannel.Substring(1, 1)));
            }            
            
            // send the target's primary dataSwitch to display its data on the SensorChart's broadcaster chart
            broadcaster.sensorModels = new AtomicDataSwitch[] { dataSwitches[0] };
            #endregion

            // ***************************************************************************
            // ***************************************************************************
            // WHEN ADDING NEW PAGES TO THE EFFECTS MENU, MAKE SURE TO ADD ITS DISPLAY NAME AND GAMEOBJECT NAME IN THIS SECTION
            #region setup navigation
            activeTabDisplayNames = new string[] { "Rotation", "Scale", "Visibility", "Channels", "Plugins"};
            activeTabObjectNames = new string[] { "Rot", "Scale", "Vis", "Cha", "Plugin"};           
            pages = new GameObject[] { rotPage, scalePage, visPage, channelsPage, pluginsPage};
            totalNumPages = pages.Length;
            #endregion

            #region setup rotate and scale tabs
            LoadModelEffectValue("XRotate", ref effectsModel.rx_enabled, ref effectsModel.rx_mult);
            LoadModelEffectValue("YRotate", ref effectsModel.ry_enabled, ref effectsModel.rx_mult);
            LoadModelEffectValue("ZRotate", ref effectsModel.rz_enabled, ref effectsModel.rx_mult);
            LoadModelEffectValue("XScaleLeft", "XScaleRight", ref effectsModel.sxEnabledA, ref effectsModel.sxEnabledB, ref effectsModel.sx_mult);
            LoadModelEffectValue("YScaleUp", "YScaleDown", ref effectsModel.syEnabledA, ref effectsModel.syEnabledB, ref effectsModel.sy_mult);
            LoadModelEffectValue("ZScaleLeft", "ZScaleRight", ref effectsModel.szEnabledA, ref effectsModel.szEnabledB, ref effectsModel.sz_mult);
            #endregion

            #region setup vis tab
            if (!effectsModel.vis_enabled)
            {
                GameObject visAboveBelow = transform.Find("All/Vis/Labels/AboveOrBelow").gameObject;
                visAboveBelow.SetActive(false);
                GameObject visAboveBelowButton = transform.Find("All/Vis/Buttons/ToggleAboveOrBelow").gameObject;
                visAboveBelowButton.SetActive(false);
                GameObject visValSensor = transform.Find("All/Vis/Labels/ValSensor").gameObject;
                visValSensor.SetActive(false);
                GameObject visVal = transform.Find("All/Vis/Labels/ValVis").gameObject;
                visVal.SetActive(false);
                sliderObjects["Vis"].SetActive(false);
                transform.Find("All/Vis/Labels/Slider").gameObject.SetActive(false);
            }
            // set the visibility threshold slider value
            sliders["Vis"].SliderValue = effectsModel.vis_thresh / 1024f;

            // set the visibility threshold slider text
            transform.Find("All/Vis/Labels/ValVis").GetComponent<TextMeshPro>().SetText(string.Format("{0:0.00}", effectsModel.vis_thresh / 1024f));
            #endregion

            #region setup channel tab

            selectedChannel = dataSwitches[0].CurrentChannel.Substring(1);

            Transform toggle = transform.Find("All/Cha/0/Buttons/Channel" + selectedChannel.ToString());
            GameObject channelbackplateToggle = toggle.Find("Backplate Plus Button/BackplateToggle").gameObject;
            channelbackplateToggle.SetActive(true);

            //activate extra rows on the channels tab if the target has more dataSwitches
            transform.Find("All/Cha/1").gameObject.SetActive(dataSwitches.Length > 1);
            transform.Find("All/Cha/2").gameObject.SetActive(dataSwitches.Length == 3);

            #endregion

            #region setup plugins tab

            pluginManagers = new AEffect4PluginManager[target.Find("EffectsModel").childCount];
            int pluginIndex = 0;
            foreach (Transform child in target.Find("EffectsModel"))
            {
                pluginManagers[pluginIndex] = child.GetComponent<AEffect4PluginManager>();

                // activate game objects for plugin launch button
                transform.Find("All/Plugin/"+pluginIndex.ToString()).gameObject.SetActive(true);

                // set plugin name text field
                transform.Find("All/Plugin/" + pluginIndex.ToString() + "/Labels/Name").GetComponent<TextMeshPro>().SetText(pluginManagers[pluginIndex].GetPluginName());
                pluginIndex++;
            }

            #endregion

            // ***************************************************************************
            // ***************************************************************************
            // WHEN ADDING NEW PAGES TO THE EFFECTS MENU, MAKE SURE TO ADD ITS DISPLAY NAME AND GAMEOBJECT NAME IN THIS SECTION
            #region setup color tab
            // enable Color tab if the target is a drawing
            // otherwise, grey out the color tab
            if (target.name == "BaseLineDrawing")
            {
                // add the color page to the beginning of the pages arrays
                // ***************************************************************************
                // ***************************************************************************
                // WHEN ADDING NEW PAGES TO THE EFFECTS MENU, MAKE SURE TO ADD ITS DISPLAY NAME AND GAMEOBJECT NAME HERE
                activeTabDisplayNames = new string[] { "Color", "Rotation", "Scale", "Visibility", "Channels", "Plugins"};
                activeTabObjectNames = new string[] { "Color", "Rot", "Scale", "Vis", "Cha", "Plugin"};
                pages = new GameObject[] { colorPage, rotPage, scalePage, visPage, channelsPage, pluginsPage};
                totalNumPages = pages.Length;
                // prepare color tab
                lineRenderer = target.GetComponent<LineRenderer>();
                selectedColor = "";
                drawingPropertiesManager = target.GetComponent<DrawingPropertiesManager>();
                drawingColors = new Material[]{
                    drawingPropertiesManager.lineMaterial_color0,
                    drawingPropertiesManager.lineMaterial_color1,
                    drawingPropertiesManager.lineMaterial_color2,
                    drawingPropertiesManager.lineMaterial_color3
                };
                for (int i = 1; i < 5; i++)
                {
                    MeshRenderer colormesh = colorPage.transform.Find("Buttons/Color" + i.ToString() + "/Backplate Plus Button/BackplateNormal").GetComponent<MeshRenderer>();
                    colormesh.material = drawingColors[i - 1];
                }
            }
            else
            {
                MeshRenderer colorTabMesh = transform.Find("All/EffectsMenuNav/Tabs/Color/GoTo/BackPlate/Quad").GetComponent<MeshRenderer>();
                colorTabMesh.material = greyMaterial;
            }
            #endregion

            UpdateCurrentMenuPage();       
        }
        else
        {
            objectNameLabel.text = "(no edited object)";

            #region disable all tabs
            activeTabDisplayNames = new string[] { "No target"};
            totalNumPages = 1;
            
            foreach (string tabname in allTabObjectNames)
            {
                transform.Find("All/EffectsMenuNav/Tabs/" + tabname + "/BackplateOff/Quad").GetComponent<MeshRenderer>().material = greyMaterial;
            }
            #endregion 
        }
    }

    void LoadModelEffectValue(string valname, ref bool modelValEnabled, ref float modelVal)
    {
        backplateNormals[valname].SetActive(!modelValEnabled);
        backplateToggles[valname].SetActive(modelValEnabled);

        string modelValName = GetModelVal(valname);
        sliders[modelValName].SliderValue = 0.5f * (modelVal + 1f);
        sliderObjects[modelValName].SetActive(modelValEnabled);
        sliderLabels[modelValName].SetActive(modelValEnabled);
        if (modelValEnabled)
        {
            sliderLabels[modelValName].GetComponent<TextMeshPro>().SetText(string.Format("{0:0.00}", modelVal));
        }
    }

    void LoadModelEffectValue(string valnameA, string valnameB, ref bool modelValEnabledA,ref bool modelValEnabledB, ref float modelVal)
    {
        backplateNormals[valnameA].SetActive(!modelValEnabledA);
        backplateToggles[valnameA].SetActive(modelValEnabledA);
        backplateNormals[valnameB].SetActive(!modelValEnabledB);
        backplateToggles[valnameB].SetActive(modelValEnabledB);

        string modelValName = GetModelVal(valnameA);
        sliders[modelValName].SliderValue = 0.5f * (modelVal + 1f);
        sliderObjects[modelValName].SetActive(modelValEnabledA || modelValEnabledB);
        sliderLabels[modelValName].SetActive(modelValEnabledA || modelValEnabledB);
        if (modelValEnabledA || modelValEnabledB)
        {
            sliderLabels[modelValName].GetComponent<TextMeshPro>().SetText(string.Format("{0:0.00}", modelVal));
        }
    }

    public void RefreshVisTabDataSwitchValues(float val)
    {
        float newval = val / 1024f;
        if (target != null)
        {
            //update the valsensor text field on the Vis page
            dataSwitchText.SetText(string.Format("{0:0.00}", val));

            //updatged the slider position on the Vis page
            indicatorpinchSlider.SliderValue = newval;
        }
    }

    #region toggle button update functions

    public void ToggleButton(string buttonName, ref bool modelValueEnabled)
    {
        string modelValName = GetModelVal(buttonName);
        backplateNormals[buttonName].SetActive(!backplateNormals[buttonName].activeSelf);
        backplateToggles[buttonName].SetActive(!backplateToggles[buttonName].activeSelf);
        modelValueEnabled = !modelValueEnabled;
        sliderObjects[modelValName].SetActive(modelValueEnabled);
        sliderLabels[modelValName].SetActive(modelValueEnabled);
    }

    public void ToggleXRotate() { ToggleButton("XRotate", ref effectsModel.rx_enabled); }
    public void ToggleYRotate() { ToggleButton("YRotate", ref effectsModel.ry_enabled); }
    public void ToggleZRotate() { ToggleButton("ZRotate", ref effectsModel.rz_enabled); }
    public void ToggleXScaleLeft() { ToggleButton("XScaleLeft", ref effectsModel.sxEnabledA); }
    public void ToggleXScaleRight() { ToggleButton("XScaleRight", ref effectsModel.sxEnabledB); }
    public void ToggleYScaleUp() { ToggleButton("YScaleUp", ref effectsModel.syEnabledA); }
    public void ToggleYScaleDown() { ToggleButton("YScaleDown", ref effectsModel.syEnabledB); }
    public void ToggleZScaleLeft() { ToggleButton("ZScaleLeft", ref effectsModel.szEnabledA); }
    public void ToggleZScaleRight() { ToggleButton("ZScaleRight", ref effectsModel.szEnabledB); }
    
    public void ToggleVisibility()
    {
        Debug.Log("Toggle visibility");
        backplateNormals["Vis"].SetActive(!backplateNormals["Vis"].activeSelf);
        backplateToggles["Vis"].SetActive(!backplateToggles["Vis"].activeSelf);
        effectsModel.vis_enabled = !effectsModel.vis_enabled;
        sliderObjects["Vis"].SetActive(effectsModel.vis_enabled);
        sliderLabels["Vis"].SetActive(effectsModel.vis_enabled);

        string[] labelObjectNames = new string[] { "AboveOrBelow", "ValSensor", "ValVis", "Slider" };
        foreach (string n in labelObjectNames)
        {
            transform.Find("All/Vis/Labels/"+n).gameObject.SetActive(effectsModel.vis_enabled);
        }
        transform.Find("All/Vis/Buttons/ToggleAboveOrBelow").gameObject.SetActive(effectsModel.vis_enabled);
    }

    public void ToggleAboveBelow()
    {
        GameObject backplateNormal = transform.Find("All/Vis/Buttons/ToggleAboveOrBelow/Backplate Plus Button/BackplateNormal").gameObject;
        GameObject backplateToggle = transform.Find("All/Vis/Buttons/ToggleAboveOrBelow/Backplate Plus Button/BackplateToggle").gameObject;
        backplateNormal.SetActive(!backplateNormal.activeSelf);
        backplateToggle.SetActive(!backplateToggle.activeSelf);
        effectsModel.vis_visibleAbove = !effectsModel.vis_visibleAbove;
        effectsModel.vis_visibleBelow = !effectsModel.vis_visibleBelow;
    }

    public void ToggleColor(int n)
    {
        if (selectedColor != n.ToString())
        {
            Transform toggle = transform.Find("All/Color/Buttons/Color" + n.ToString());
            GameObject backplateToggle = toggle.Find("Backplate Plus Button/BackplateToggle").gameObject;
            backplateToggle.SetActive(true);
            if (selectedColor != "")
            {
                Transform oldtoggle = transform.Find("All/Color/Buttons/Color" + selectedColor);
                GameObject oldbackplateToggle = oldtoggle.Find("Backplate Plus Button/BackplateToggle").gameObject;
                oldbackplateToggle.SetActive(false);
            }
            selectedColor = n.ToString();
            lineRenderer.material = drawingColors[n - 1];
        }
    }

    public void ToggleChannel(int switchNum, int n)
    {
        if (selectedChannel != n.ToString())
        {
            Transform toggle = transform.Find("All/Cha/" + switchNum.ToString() + "/Buttons/Channel" + n.ToString());
            GameObject backplateToggle = toggle.Find("Backplate Plus Button/BackplateToggle").gameObject;
            backplateToggle.SetActive(true);
            if (selectedChannel != "")
            {
                Debug.Log("Selected " + selectedChannel + " new " + n.ToString());
                Transform oldtoggle = transform.Find("All/Cha/" + switchNum.ToString() + "/Buttons/Channel" + selectedChannel);
                GameObject oldbackplateToggle = oldtoggle.Find("Backplate Plus Button/BackplateToggle").gameObject;
                oldbackplateToggle.SetActive(false);
            }
            selectedChannel = n.ToString();

            dataSwitches[switchNum].CurrentChannel = "C" + selectedChannel;
            chartChannelName.GetComponent<TextMesh>().text = "C" + selectedChannel;
        }
    }

    public void TogglePlugin(int pluginNum)
    {
        pluginManagers[pluginNum].ToggleMenuGUI();
        Transform toggle = transform.Find("All/Plugin/" + pluginNum.ToString() + "/Buttons/Launch");
        GameObject backplateToggle = toggle.Find("Backplate Plus Button/BackplateToggle").gameObject;
        backplateToggle.SetActive(!backplateToggle.activeSelf);
    }
    #endregion

    #region slider update functions
    public void UpdateSlider(string modelValName, ref float modelVal)
    {
        float sliderVal = 2f * sliders[modelValName].SliderValue - 1f;
        modelVal = sliderVal;
        sliderLabels[modelValName].GetComponent<TextMeshPro>().SetText(string.Format("{0:0.00}", sliderVal));
    }
    public void UpdateXRotation() { UpdateSlider("XRotate", ref effectsModel.rx_mult); }
    public void UpdateYRotation() { UpdateSlider("YRotate", ref effectsModel.ry_mult); }
    public void UpdateZRotation() { UpdateSlider("ZRotate", ref effectsModel.rz_mult); }
    public void UpdateXScale() { UpdateSlider("XScale", ref effectsModel.sx_mult); }
    public void UpdateYScale() { UpdateSlider("YScale", ref effectsModel.sy_mult); }
    public void UpdateZScale() { UpdateSlider("ZScale", ref effectsModel.sz_mult); }
    public void UpdateVisibilityThreshold()
    {
        float sliderVal = 1024f * sliders["Vis"].SliderValue;
        effectsModel.vis_thresh = sliderVal;
        transform.Find("All/Vis/Labels/ValVis").GetComponent<TextMeshPro>().SetText(string.Format("{0:0.00}", sliderVal));
    }
    #endregion

    #region navigation functions

    public void GoTo(string tabname)
    {
        int pageNum = System.Array.IndexOf(activeTabObjectNames, tabname);
        if (pageNum != -1)
        {
            currentPageNum = pageNum;
            UpdateCurrentMenuPage();
        }
    }

    void UpdateNavLabel()
    {
        nav.transform.Find("Current/IconAndText/TextMeshPro").GetComponent<TextMeshPro>().SetText(activeTabDisplayNames[currentPageNum]);
    }

    public void UpdateCurrentMenuPage()
    {
        foreach (string tabName in allTabObjectNames)
        {
            transform.Find("All/" + tabName).gameObject.SetActive(false);
            nav.transform.Find("Tabs/" + tabName + "/BackplateOn").gameObject.SetActive(false);
            nav.transform.Find("Tabs/" + tabName + "/GoTo/BackPlate").gameObject.SetActive(true);
        }

        for (int i = 0; i < totalNumPages; i++)
        {
            if (i == currentPageNum)
            {
                pages[i].SetActive(true);
                nav.transform.Find("Tabs/" + activeTabObjectNames[i] + "/BackplateOn").gameObject.SetActive(true);
                nav.transform.Find("Tabs/" + activeTabObjectNames[i] + "/GoTo/BackPlate").gameObject.SetActive(false);
            }
        }
        UpdateNavLabel();
    }
    #endregion 

    public void ClickSensorChart()
    {
        if (target != null)
        {
            manualDataChannel.SetActive(true);
            manualDataChannel.transform.GetComponent<ManualDataChannelsController>().SetModel(dataSwitches[0]);
            manualDataChannel.transform.Find("DataSwitchInputs/SwitchSlider").GetComponent<ChannelSliderController>().AddModel(dataSwitches[0]);
            manualDataChannel.transform.Find("DataSwitchInputs/SwitchButton").GetComponent<ChannelButtonController>().AddModel(dataSwitches[0]);                      
        }
    }
}