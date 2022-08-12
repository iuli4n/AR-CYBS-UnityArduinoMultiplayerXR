using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManualDataChannelsController : MonoBehaviour
{
    public AtomicDataSwitch myModel;
    int storedChannelNum;
    public TextMeshPro channelLabel;
    public bool isStandalone;

    // Start is called before the first frame update
    void Start()
    {
        if (isStandalone)
        {
            if (myModel == null)
            {
                Debug.LogError("This ManualDataChannelController is set for standalone - in this case it requires its myModel to point to an AtomicDataSwitch. Attach one to this object and link it to myModel.");
            }

            SetModel(myModel);

            foreach (var g in gameObject.transform.GetComponentsInChildren<ChannelSliderController>())
            {
                g.AddModel(myModel);
            }

            foreach (var g in gameObject.transform.GetComponentsInChildren<ChannelButtonController>())
            {
                g.AddModel(myModel);
            }
            string n = myModel.unityView__CurrentChannel.Substring(1, 1);
            int n_ = int.Parse(n);
            ToggleOnDataSwitchChannelButton(n_);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseDataChannels()
    {
        Destroy(this.gameObject);
    }

    public void SetDataSwitchChannel(int n)
    {
        ToggleOffDataSwitchChannelButton(storedChannelNum);
        myModel.SetCurrentChannel("C" + n.ToString());
        channelLabel.SetText("C" + n.ToString());
        ToggleOnDataSwitchChannelButton(n);
    }

    public void ToggleOnDataSwitchChannelButton(int n)
    {
        transform.Find("ChannelButtons/C" + n.ToString() + "/Backplate Plus Button/BackplateNormal").gameObject.SetActive(false);
        transform.Find("ChannelButtons/C" + n.ToString() + "/Backplate Plus Button/BackplateToggle").gameObject.SetActive(true);
        storedChannelNum = n;
    }

    public void ToggleOffDataSwitchChannelButton(int n)
    {
        transform.Find("ChannelButtons/C" + n.ToString() + "/Backplate Plus Button/BackplateNormal").gameObject.SetActive(true);
        transform.Find("ChannelButtons/C" + n.ToString() + "/Backplate Plus Button/BackplateToggle").gameObject.SetActive(false);
    }

    public void SetModel(AtomicDataSwitch model)
    {
        myModel = model;
        string nn = myModel.unityView__CurrentChannel.Substring(1, 1);
        int n_ = int.Parse(nn);
        myModel.SetCurrentChannel("C" + nn);

        channelLabel.SetText("C" + n_);
        ToggleOnDataSwitchChannelButton(n_);
    }

}
