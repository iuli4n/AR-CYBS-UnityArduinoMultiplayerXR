using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCreatedModel : MonoBehaviour
{
    [SerializeField]
    private GameObject currentInstantiated3DModel;
    
    public GameObject myModelContainer;

    public float overrideScaleFactor = 1;

    [SerializeField]
    AtomicDataModelString myPathModel;
    [SerializeField] 
    AtomicDataModelVector3 myOffsetModel;

    private void Awake()
    {
        myPathModel = this.gameObject.transform.Find("ModelInfo").GetComponent<AtomicDataModelString>();
        myOffsetModel = this.gameObject.transform.Find("ModelInfo").GetComponent<AtomicDataModelVector3>();
        myModelContainer = this.gameObject.transform.Find("ModelContainer").gameObject;

        if (myPathModel.Value != null && myPathModel.Value.Length > 0)
        {
            Refresh();
        }
    }
    
    public string GetModelPath()
    {
        return myPathModel.Value;
    }
    public Vector3 GetModelOffset()
    {
        return myOffsetModel.Value;
    }
    
    public void SetAndRefresh(string drivePath, Vector3 offset)
    {
        myPathModel.Value = drivePath;
        myOffsetModel.Value = offset;
        Refresh();
    }



    public void Refresh()
    {
        if (!myPathModel.Value.StartsWith("/") && !myPathModel.Value.StartsWith("\\"))
        {
            myPathModel.Value = "/" + myPathModel.Value;
        }

        if (currentInstantiated3DModel != null)
        {
            Debug.LogWarning("DESTROYING LOCAL: SHOULD BE NETWORKED");
            GameObject.Destroy(currentInstantiated3DModel);
            currentInstantiated3DModel = null;
        }

        currentInstantiated3DModel = FileDriveManager.Instance.SpawnModelFromFile(myPathModel.Value);
        currentInstantiated3DModel.transform.parent = myModelContainer.transform;
        currentInstantiated3DModel.transform.localScale *= overrideScaleFactor;// * new Vector3(1f, 1f, 1f); // will be overriden by effect4g
        currentInstantiated3DModel.transform.localRotation = Quaternion.Euler(0,0,0);
        currentInstantiated3DModel.transform.localPosition = myOffsetModel.Value;

        BoxCollider bc = myModelContainer.GetComponentInChildren<BoxCollider>();
        if (bc != null && bc.enabled)
        {
            this.GetComponent<BoundsControl>().BoundsOverride = bc;
        }

        // refresh this because the model's bounds have changed
        this.GetComponent<BoundsControl>().enabled = false;

        // BUG: this doesn't seem to do anything, maybe because we're doing it in the same frame ?
        this.GetComponent<BoundsControl>().enabled = true;



    }
}
