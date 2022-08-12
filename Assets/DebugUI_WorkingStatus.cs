using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DebugUI_WorkingStatus : MonoBehaviour
{
    public static DebugUI_WorkingStatus Instance;
    
    public GameObject progressIndicator;
    private IProgressIndicator indicator;

    private void Awake()
    {
        Debug.Assert(Instance == null, "Should not have multiple instances of this object!");
        Instance = this;
    }

    private void Start()
    {
        indicator = progressIndicator.GetComponent<IProgressIndicator>();
    }

    public void SetMessage(string msg)
    {
        indicator.Message = msg;
    }



    private ProgressIndicatorState wantToBeInState = ProgressIndicatorState.Closed;

    public async void OpenProgressIndicator(string message = "Loading..")
    {
        if (wantToBeInState == ProgressIndicatorState.Open) return;
        wantToBeInState = ProgressIndicatorState.Open;

        while (indicator.State == ProgressIndicatorState.Closing || indicator.State == ProgressIndicatorState.Opening)
        {
            // wait for it to complete the previous state change
            await Task.Delay(200);
        }

        indicator.Message = message;
        await indicator.OpenAsync();

    }

    public async void CloseProgressIndicator()
    {
        await Task.Delay(200);
        
        if (wantToBeInState == ProgressIndicatorState.Closed) return;
        wantToBeInState = ProgressIndicatorState.Closed;

        while (indicator.State == ProgressIndicatorState.Closing || indicator.State == ProgressIndicatorState.Opening)
        {
            // wait for it to complete the previous state change
            await Task.Delay(200);
        }
        
        await indicator.CloseAsync();
    }

    /**
    private void OnGUI()
    {
        GUILayout.Label(".");
        GUILayout.Label(".");
        GUILayout.Label(".");
        if (GUILayout.Button("OPEN"))
        {
            OpenProgressIndicator();
        }
        if (GUILayout.Button("CLOSE"))
        {
            CloseProgressIndicator();
        }
        
    }
    **/

    // Update is called once per frame
    void Update()
    {
        
    }
}
