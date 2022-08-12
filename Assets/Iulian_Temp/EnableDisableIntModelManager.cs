using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableIntModelManager : MonoBehaviour
{

    public AtomicDataModelInt model;

    public GameObject view1;
    public GameObject view2;
    public GameObject view3;
    public int maxValues = 3;


    // Start is called before the first frame update
    void Start()
    {
        model.OnDataUpdated += Model_OnDataUpdated;
    }
    public void NextValue()    {        model.Value = (model.Value + 1) % maxValues;    }    public void PrevValue()    {        model.Value = (model.Value - 1) % maxValues;    }
    public void Model_OnDataUpdated(int newval)
    {
        view1.SetActive(newval == 1);
        view2.SetActive(newval == 2);
        view3.SetActive(newval == 3);
    }

    void Update()
    {

    }
}
