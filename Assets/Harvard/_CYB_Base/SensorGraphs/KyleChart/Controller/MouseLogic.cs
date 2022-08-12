using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

/// <summary>
/// Enables mouse input for all interactables in scene.
/// </summary>
public class MouseLogic : MonoBehaviour
{
    // update delay
    public float  updateWait = 0.1f;
    private float updateNext;

    // clicks between updates
    private bool click;
    
    void Start()
    {
        updateNext = Time.time + updateWait;
        click = false;
    }
    
    void Update()
    {
        click |= Input.GetMouseButtonDown(0);
        if (Time.time < updateNext) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Interactable button = hit.transform.GetComponent<Interactable>();
            if (click && button != null) button.TriggerOnClick();

            PinchSlider slider = hit.transform.GetComponent<PinchSlider>();
            if (click && slider != null)
            {
                float magLess = Vector3.Magnitude(hit.point - slider.SliderStartPosition);
                float magMore = Vector3.Magnitude(slider.SliderEndPosition - hit.point);

                slider.SliderValue = Mathf.Clamp(magLess / (magLess + magMore), 0, 1);
            }

            Settings sett = hit.transform.GetComponentInParent<Settings>();
            if (sett != null) sett.manipulator.enabled = (button == null) && (slider == null);
        }
            
        updateNext = Time.time + updateWait;
        click = false;
    }
}
