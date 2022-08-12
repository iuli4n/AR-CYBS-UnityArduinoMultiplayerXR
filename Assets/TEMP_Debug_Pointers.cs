using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class TEMP_Debug_Pointers : MonoBehaviour
{
    /// <summary>
    /// Simple class. Keeps track of the current pointer type and draws the proper color pointer tip.
    /// Also enables/disables editing for drawing mode
    /// Also enables/disables UI buttons
    /// Also tells the IuliPointersMediator where the tip is and when click happens.
    /// 
    /// Other classes use this to find out current pointer.
    /// Mainly these 3 classes work together: 
    ///     this (enables/disables objects), 
    ///     IuliPointersMediator (for enabling proper pointer during different modes), 
    ///     EditingManager (for actually doing the work of configuring objects to be manipulable)
    /// 
    /// </summary>
    /// 

    // only used if we are doing some custom demo with preset sequence
    // TODO: CAN BE REMOVED ?
    public TEMP_Debug_DemoSequenceTools demoSequence = null;

    public delegate void FOnPointerSwitched(PointerType oldPointer, PointerType newPointer);
    public event FOnPointerSwitched OnPointerSwitched;

    public enum PointerType
    {
        NoControl, /*default presentation*/
        Create_Default, /*grab move/scale, laser move/scale*/
        Create_PrecisionCreate,
        Edit_PrecisionMove,
        Edit_PrecisionScale, /*precision move, precision scale*/

        Edit_SpecialPrecisionMove, /*only for special objects like path endpoints*/

        OriginalTool_Drawing,
        OriginalTool_Point1,
        OriginalTool_Point2,


    }
    public PointerType currentPointerType;

    public GameObject menuObjects_default;
    public GameObject menuObjects_editing;
    public GameObject menuObjects_creating;
    public GameObject menuObjects_creating_special;


    public IuliButtonSimulator pointerButtonSimulator = new IuliButtonSimulator();
    //public AtomicDataModelBool pointerClickModel;
    private bool customPointerClickNow = false;

    public GameObject tipPoint_forEditing;
    public GameObject tipPoint_forCreating;
    public GameObject tipPoint_forCreating_Special;
    public GameObject tipPoint_forDefault;

    public GameObject currentPointerTipPoint = null;

    public bool HAVE_PRECISION_CLICKER = false;   // if we have an external clicker device we will use that for precise manipulation; if not we'll use regular grabbing

    public static TEMP_Debug_Pointers Instance;

    //public IuliPointersMediator pointerMediator;



    private void Awake()
    {
        Debug.Assert(!Instance, "Should only have one of these objects");
        Instance = this;
    }
    void Start()
    {
        // find our pointer mediator
        //ConfigureIuliPointerMediators();

        SwitchPointerTo(PointerType.NoControl);

    }

    

    private void EnableMenuAndTipObjectsForCurrentPointer()
    {
        if (menuObjects_default) 
            menuObjects_default.SetActive(false);
        if (menuObjects_editing)
            menuObjects_editing.SetActive(false);
        if (menuObjects_creating)
            menuObjects_creating.SetActive(false);
        if (menuObjects_creating_special)
            menuObjects_creating_special.SetActive(false);


        if (currentPointerTipPoint)
            currentPointerTipPoint.SetActive(false);


        if (currentPointerType == PointerType.Edit_PrecisionMove ||
            currentPointerType == PointerType.Edit_SpecialPrecisionMove ||
            currentPointerType == PointerType.Edit_PrecisionScale)
        {
            currentPointerTipPoint = HAVE_PRECISION_CLICKER ? tipPoint_forEditing : null;
            if (menuObjects_editing)
                menuObjects_editing.SetActive(true);
        }
        else if (currentPointerType == PointerType.Create_Default)
        {
            currentPointerTipPoint = HAVE_PRECISION_CLICKER ? tipPoint_forCreating : null;
            if (menuObjects_creating) 
                menuObjects_creating.SetActive(true);
        }
        else if (currentPointerType == PointerType.Create_PrecisionCreate)
        {
            currentPointerTipPoint = HAVE_PRECISION_CLICKER ? tipPoint_forCreating_Special : null;
            if (menuObjects_creating_special) 
                menuObjects_creating_special.SetActive(true);
        }
        else
        {
            currentPointerTipPoint = tipPoint_forDefault;
            if (menuObjects_default)
                menuObjects_default.SetActive(true);
        }

        if (currentPointerTipPoint)
            currentPointerTipPoint.SetActive(true);


        

        
        
    }


    public void SetEditingPointerHead(Color color)
    {
        currentPointerTipPoint.GetComponent<Renderer>().material.color = color;
    }

    float nextUpdatePointers = 0;

    public bool showGUI = false;

    public void SwitchPointerTo(PointerType t)
    {
        PointerType oldP = currentPointerType;
        currentPointerType = t;
        EnableMenuAndTipObjectsForCurrentPointer();

        OnPointerSwitched?.Invoke(oldP, currentPointerType);
    }

    // Update is called once per frame
    void OnGUI()
    {
        if (Time.time > nextUpdatePointers)
        {
            nextUpdatePointers = Time.time + 1f;

            foreach (IuliStickPointer p in PointerUtils.GetPointers<IuliStickPointer>())
            {
                p.overrideGrabPoint = currentPointerTipPoint;
            }

            //DisableOtherPointers();
        }


        // When using an external clicker, we need to explicitly tell the stick pointer that it was clicked. [TODO: there's gotta be a cleaner way of doing this through the MRTK input system] 
        if (HAVE_PRECISION_CLICKER)
        {

            if (pointerButtonSimulator.NowClicked)
            {
                foreach (IuliStickPointer p in PointerUtils.GetPointers<IuliStickPointer>())
                {
                    //Debug.Log("--hack11--");
                    p.HACKEDOnInputDown();
                }
            }
            if (pointerButtonSimulator.NowUnclicked)
            {
                foreach (IuliStickPointer p in PointerUtils.GetPointers<IuliStickPointer>())
                {
                    // TODO:BUG: If the pointer disappears and this happens during that time, the unclick might not be registered ?

                    //Debug.Log("--hack22--");
                    p.HACKEDOnInputUp();
                }
            }


            //if (!showGUI) return;

            //GUILayout.Label("Current mode: " + pointerMediator.currentPointerType);

            /**
            if (GUILayout.Button("MODE: DEMO"))
            {
                SwitchPointerTo(PointerType.Create);
                demoSequence.BeginSequence();
            }
            ***/
        }
        else
        {
            // when not using external clicker, the clicks are done through grab gestures which automatically work properly through the MRTK pointers
        }




        return;

        /*

        if (GUILayout.Button("POINTERS OFF"))
        {
            UpdatePointers(PointerBehavior.AlwaysOff);
        }
        if (GUILayout.Button("POINTERS ON"))
        { 
            UpdatePointers(PointerBehavior.Default);
        }

        if (Input.GetMouseButtonDown(1))
        {
            foreach (IuliStickPointer p in PointerUtils.GetPointers<IuliStickPointer>())
            {
                Debug.Log("--hack1--");
                p.HACKEDOnInputDown();
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            foreach (IuliStickPointer p in PointerUtils.GetPointers<IuliStickPointer>())
            {
                Debug.Log("--hack2--");
                p.HACKEDOnInputUp();
            }
        }
        */
    }

    private void Update()
    {
        // TODOIR PICKUP HERE: use boolean variable inside this

        if (HAVE_PRECISION_CLICKER)
        {
            customPointerClickNow /*pointerClickModel.Value*/ = Input.GetKey(KeyCode.Return);// Input.GetMouseButton(1);
        } else
        {
            customPointerClickNow /*pointerClickModel.Value*/ = Debug_Keyboard.IsPinching(Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right);
        }
        pointerButtonSimulator.Tick();
        pointerButtonSimulator.OnButtonChange(customPointerClickNow /*pointerClickModel.Value*/);
    }

}


/**
void ConfigureIuliPointerMediators(IuliPointersMediator.CurrentPointerType pointerType = IuliPointersMediator.CurrentPointerType.Default)
{
    IMixedRealityInputSystem inputSystem;
    if (!MixedRealityServiceRegistry.TryGetService(out inputSystem))
    {
        Debug.LogError("Cannot find input system");
    }
    int count = 0;
    foreach (IMixedRealityPointerMediator pm in (inputSystem.FocusProvider as FocusProvider)?.PointerMediators.Values)
    {
        if (pm is IuliPointersMediator)
        {
            Debug.LogWarning("*** found mediator " + count++ + " " + pm);
            (pm as IuliPointersMediator).overrideCustomPointerTipWith = editingPoint;
            (pm as IuliPointersMediator).currentPointerType = pointerType;
            //if (pointerMediator != null) Debug.LogError("**** Already found another IuliPointersMediator ***");
            //return (IuliPointersMediator)pm;
        }
    }
    //return null;
}
**/

/**
void ConfigureDefaultMRTKPointers(PointerBehavior beh)
{
    PointerUtils.SetPointerBehavior<ShellHandRayPointer>(beh);
    PointerUtils.SetPointerBehavior<ParabolicTeleportPointer>(beh);
    PointerUtils.SetPointerBehavior<TouchPointer>(beh);
    //PointerUtils.SetPointerBehavior<MousePointer>(beh);
    PointerUtils.SetPointerBehavior<GGVPointer>(beh);
    //PointerUtils.SetPointerBehavior<PokePointer>(beh);
    PointerUtils.SetPointerBehavior<SpherePointer>(beh);

}

void DisableOtherPointers()
{
    var beh = PointerBehavior.AlwaysOff;

    ConfigureDefaultMRTKPointers(PointerBehavior.AlwaysOff);
    PointerUtils.SetPointerBehavior<IuliStickPointer>(PointerBehavior.AlwaysOn);
    //PointerUtils.get

    // TODO: not the right place for this

    //PointerUtils.SetPointerBehavior<IuliStickPointer>(PointerBehavior.AlwaysOn);
    foreach (IuliStickPointer p in PointerUtils.GetPointers<IuliStickPointer>())
    {
        p.overrideGrabPoint = editingPoint;
    }

}
void EnableOtherPointers()
{
    var beh = PointerBehavior.Default;
    PointerUtils.SetPointerBehavior<ShellHandRayPointer>(beh);
    PointerUtils.SetPointerBehavior<ParabolicTeleportPointer>(beh);
    PointerUtils.SetPointerBehavior<TouchPointer>(beh);
    //PointerUtils.SetPointerBehavior<MousePointer>(beh);
    PointerUtils.SetPointerBehavior<GGVPointer>(beh);
    //PointerUtils.SetPointerBehavior<PokePointer>(beh);
    PointerUtils.SetPointerBehavior<SpherePointer>(beh);

    PointerUtils.SetPointerBehavior<IuliStickPointer>(PointerBehavior.AlwaysOff);
}

**/


