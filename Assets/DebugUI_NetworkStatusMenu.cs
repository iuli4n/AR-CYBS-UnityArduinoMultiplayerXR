using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugUI_NetworkStatusMenu : MonoBehaviour
{
    public static DebugUI_NetworkStatusMenu Instance;

    public Renderer backplate;
    public TextMeshPro textMesh;

    public Material materialGood;
    public Material materialBad;


    Vector3 goodPos, goodRot;
    Vector3 badPos, badRot;
    Vector3 goodScale, badScale;

    public bool debug_dontMove = false;

    public void ShowStatusGood(string text)
    {
        if (!debug_dontMove)
        {
            this.GetComponent<SolverHandler>().AdditionalOffset = goodPos;
            this.GetComponent<SolverHandler>().AdditionalRotation = goodRot;
            this.transform.localScale = goodScale;
        }

        textMesh.text = text;
        backplate.material = materialGood;
    }
    public void ShowStatusBad(string text)
    {
        if (!debug_dontMove)
        {
            this.GetComponent<SolverHandler>().AdditionalOffset = badPos;
            this.GetComponent<SolverHandler>().AdditionalRotation = badRot;
            this.transform.localScale = badScale;
        }

        textMesh.text = text;
        backplate.material = materialBad;
    }


    private void Awake()
    {
        goodPos = this.GetComponent<SolverHandler>().AdditionalOffset;
        goodRot = this.GetComponent<SolverHandler>().AdditionalRotation;
        badPos = Vector3.zero;
        badRot = Vector3.zero;

        goodScale = this.transform.localScale;
        badScale = goodScale * 3;

        Debug.Assert(Instance == null, "Should not have multiple instances of this object!");
        Instance = this;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
