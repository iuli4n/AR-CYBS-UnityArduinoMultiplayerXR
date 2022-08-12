using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToggleEnabler : MonoBehaviour
{
    Interactable toggleButton;

    public UnityEvent onToggled;
    public UnityEvent onUntoggled;

    // Start is called before the first frame update
    void Start()
    {
        toggleButton = this.GetComponent<Interactable>();
        toggleButton.OnClick.AddListener(OnToggleEvent);
    }

    public void OnToggleEvent()
    {
        if (toggleButton.IsToggled)
        {
            onToggled?.Invoke();
        } else
        {
            onUntoggled?.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
