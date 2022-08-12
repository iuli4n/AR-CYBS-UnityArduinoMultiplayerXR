using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;

public class EffectsMenu : MonoBehaviour
{

    [SerializeField]
    GameObject effectsMenuprefab;
    EffectsMenuController menuController;

    [SerializeField]
    GameObject menuLaunchButtonPrefab;
    GameObject menuLaunchButton;

    [SerializeField]
    GameObject menuTitlePrefab;
    GameObject title;

    public Vector3 effectsMenuPositionOffset = new Vector3(-0.25f, 0f, -0.25f);
    public Vector3 effectsButtonPositionOffset = new Vector3(0f, 0f, -0.3f);

    public GameObject useThisStaticPosition;

    bool showingMenu;

    GameObject menu;
    Transform targetTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlaceLaunchButton()
    {
        Vector3 effectsLaunchButtonPosition = targetTransform.position + effectsButtonPositionOffset;
        if (useThisStaticPosition)
            effectsLaunchButtonPosition = useThisStaticPosition.transform.position;

        menuLaunchButton = Instantiate(menuLaunchButtonPrefab, effectsLaunchButtonPosition, transform.rotation);
        menuLaunchButton.transform.parent = transform;
        menuLaunchButton.AddComponent<Billboard>();
        var interactable = menuLaunchButton.GetComponent<Interactable>();
        interactable.OnClick.AddListener(ToggleEffectsMenu);
    }

    // Update is called once per frame
    void Update()
    {     
        if (EditingManager.Instance.GetLastEdited() != null)
        {
            GameObject t = EditingManager.Instance.GetLastEdited();

            if (t.transform != targetTransform)
            {
                if (useThisStaticPosition && showingMenu)
                {
                    // Menu is static in one place
                    targetTransform = t.transform;

                    EffectsMenuController menuController = menu.transform.GetComponent<EffectsMenuController>();
                    menuController.SetTarget(targetTransform);

                    /*
                    // reopen the menu so we get updated settings
                    // TODO: need cleaner way of refreshing, but for now this is ok
                    // TODO: if keeping this way, should remember what tab we were on
                    ToggleEffectsMenu(); // close
                    ToggleEffectsMenu(); // reopen
                    */
                }
                else
                {
                    // Menu is dynamically generated for each object

                    Destroy(menuLaunchButton);
                    targetTransform = t.transform;
                    PlaceLaunchButton();
                }
                
            }
        }
        else
        {
            if (targetTransform != null)
            {
                targetTransform = null;
                menuController = menu.transform.GetComponent<EffectsMenuController>();
                menuController.SetTarget(targetTransform);
            }
        }
    }

    public void ToggleEffectsMenu()
    {
        // get most recently touched object;

        if (!showingMenu)
        {
            showingMenu = true;
            Vector3 effectsMenuPosition;
            
            if (useThisStaticPosition)
                effectsMenuPosition  = useThisStaticPosition.transform.position;
            else
                effectsMenuPosition = targetTransform.position + effectsMenuPositionOffset;
            
            menu = Instantiate(effectsMenuprefab, effectsMenuPosition, Quaternion.identity);

            // place menu to face user
            menu.transform.LookAt(Camera.main.transform);
            menuController = menu.transform.GetComponent<EffectsMenuController>();
            menuController.SetTarget(targetTransform);
        }
        else
        {
            foreach (AtomicDataSwitch s in menuController.GetDataSwitches())
            {
                s.OnDataUpdated -= menuController.RefreshVisTabDataSwitchValues;
            }
            Destroy(menu);
            Destroy(title);
            
            showingMenu = false;
        }
    }

    //Iulian will make function to notify when current target transform's model changes

    public void RefreshMenuWhenTargetModelChanges()
    {
        Debug.Log("Refresh effects menu");
    }
}
