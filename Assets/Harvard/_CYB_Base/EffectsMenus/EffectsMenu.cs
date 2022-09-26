using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;
using static UnityEngine.GraphicsBuffer;

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
        EditingManager.Instance.onEditedObjectChanged += OnNewEditedObject;

        if (targetTransform != null)
        {
            targetTransform = null;
            menuController = menu.transform.GetComponent<EffectsMenuController>();
            menuController.SetTarget(targetTransform);
        }
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

    public static bool CanEditEffectsOnObject(GameObject o)
    {
        return
            (o.transform.Find("EffectsModel") != null);
    }

    void OnNewEditedObject(GameObject o)
    {
        // The edited object has changed; refresh the menu or just the button

        GameObject t = EditingManager.Instance.GetLastEdited();

        bool canEditEffects = CanEditEffectsOnObject(t);
        if (!canEditEffects)
        {
            // don't update anything because it means that object won't be editable
            return;
        }

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

    // Update is called once per frame
    void Update()
    {     
       
    }

    public void ToggleEffectsMenu()
    {
        if (!showingMenu)
        {
            // We will OPEN the menu
            PhotonView.Get(this).RPC("RPC_OpenMenu", RpcTarget.AllViaServer);
        }
        else
        {
            // we will CLOSE the menu
            PhotonView.Get(this).RPC("RPC_CloseMenu", RpcTarget.AllViaServer);
        }
    }

    [PunRPC] private void RPC_OpenMenu()
    {
        showingMenu = true;
        Vector3 effectsMenuPosition;

        if (useThisStaticPosition)
            effectsMenuPosition = useThisStaticPosition.transform.position;
        else
            effectsMenuPosition = targetTransform.position + effectsMenuPositionOffset;

        menu = Instantiate(effectsMenuprefab, effectsMenuPosition, Quaternion.identity);

        // place menu to face user
        menu.transform.LookAt(Camera.main.transform);
        menuController = menu.transform.GetComponent<EffectsMenuController>();
        menuController.SetTarget(targetTransform);
    }

    [PunRPC] private void RPC_CloseMenu()
    {
        foreach (AtomicDataSwitch s in menuController.GetDataSwitches())
        {
            s.OnDataUpdated -= menuController.RefreshVisTabDataSwitchValues;
        }
        Destroy(menu);
        Destroy(title);

        showingMenu = false;
    }

    public void RefreshMenuWhenTargetModelChanges()
    {
        Debug.Log("Refresh effects menu");
    }
}
