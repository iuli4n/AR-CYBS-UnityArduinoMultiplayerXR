using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCreatedPrefab : MonoBehaviour
{
    [SerializeField]
    private GameObject currentInstantiatedObject = null;

    [SerializeField]
    private PrefabDataModel myPrefabInfo;

    [SerializeField]
    public GameObject myPrefabContainer;

    private void Awake()
    {
        myPrefabInfo = this.gameObject.transform.Find("PrefabInfo").GetComponent<PrefabDataModel>();
        myPrefabContainer = this.gameObject.transform.Find("PrefabContainer").gameObject;

        /***
         * TODO: Might need to do something here when this object is loaded from a prefab.
         * This is the old code when we used to instantiate locally instead of through the network.
        if (myPrefabInfo.modelPrefabName != null && myPrefabInfo.modelPrefabName.Length > 0)
        {
            Refresh();
        }
        ***/
    }
    public string GetModelName()
    {
        return myPrefabInfo.modelPrefabName;
    }
    public void ConnectToExistingObject(int photonViewId, string prefabFilename)
    {
        GameObject subObject = PhotonNetwork.GetPhotonView(photonViewId).gameObject;
        if (subObject == null)
        {
            Debug.LogError("PlayerCreatedPrefab: could not find subobject");
            return;
        }
        // prefab filename is only used so we can regenerate it when we load from the saved database
        myPrefabInfo.modelPrefabName = prefabFilename;


        if (currentInstantiatedObject != null)
        {
            // we've already been instantiated
            Debug.LogError("*** ?? PlayerCreatedPrefab: supposed to initialize but we already have a currentInstantiatedObject");
            return;
        }

        subObject.transform.parent = myPrefabContainer.transform;
        currentInstantiatedObject = subObject;

        //Debug.Log("Prefab about to create " + filepath);
        // WAS: currentInstantiatedObject = GameObject.Instantiate((GameObject)Resources.Load(filepath), myPrefabContainer.transform);
        // WAS_OLDER: but didn't work because rescale. currentInstantiatedObject.transform.parent = myPrefabContainer.transform;

        // Now resize if needed

        if (myPrefabInfo.scaleMultiplier == 0)
            myPrefabInfo.scaleMultiplier = 1;

        currentInstantiatedObject.transform.localPosition = Vector3.zero;
        currentInstantiatedObject.transform.localScale *= myPrefabInfo.scaleMultiplier;


        // Now set the BoundsControl to point to this new object

        BoundsControl b = this.GetComponent<BoundsControl>();
        if (b != null)
        {
            BoxCollider bc = myPrefabContainer.GetComponentInChildren<BoxCollider>();
            if (bc != null && bc.enabled)
            {
                // we're using a custom collider, so will need to refresh the collision area
                b.BoundsOverride = bc;

                // not sure why but the boundscontrol created a boxcollider and it needs to be deleted by force
                b.enabled = false;
                if (this.gameObject.GetComponent<BoxCollider>())
                {
                    this.gameObject.GetComponent<BoxCollider>().enabled = false;

                    GameObject.DestroyImmediate(this.gameObject.GetComponent<BoxCollider>());
                    //Debug.LogError("**** DESTROYED BOXCOLLIDERERRER");
                }
                //b.enabled = true;
            }
        }
    }
    /**
    public void ConnectToExistingObject_OLD(string prefabFilename)
    {
        myPrefabInfo.modelPrefabName = prefabFilename;
        Refresh();
    }
    **/
    private void Refresh()
    {
        
    }
}
