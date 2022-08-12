using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManualDataChannelsControllerStandalone : MonoBehaviour
{
    public AtomicDataSwitch myModel;
    int storedChannelNum;
    public TextMeshPro channelLabel;

    // Start is called before the first frame update
    void Start()
    {
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
        myModel.SetCurrentChannel("C" + n);
        ToggleOnDataSwitchChannelButton(n_);
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
}
