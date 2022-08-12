using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IuliPointersMediator : DefaultPointerMediator
{
    // TODOS:
    //  - what does IuliStickPointer.grabbingDisabled do ?? grab should be controlled in ObjectManipulatorIuli

    // based on @julienka's code https://github.com/microsoft/MixedRealityToolkit-Unity/issues/4954

    
    // This enables/disables the MRTK pointers (ex: laser beam, fingertip, grab, ..) according to what mode we are in
    public override void UpdatePointers()
    {
        // first let MRTK do its thing, and then we'll enable/disable things as we like
        base.UpdatePointers();

        // If we have a clicker device, we're going to edit things precisely using the fingertip and select with the clicker device
        if (TEMP_Debug_Pointers.Instance.HAVE_PRECISION_CLICKER)
        {

            if (TEMP_Debug_Pointers.Instance.currentPointerType.In(
                    TEMP_Debug_Pointers.PointerType.NoControl,
                    TEMP_Debug_Pointers.PointerType.OriginalTool_Drawing,
                    TEMP_Debug_Pointers.PointerType.OriginalTool_Point1,
                    TEMP_Debug_Pointers.PointerType.OriginalTool_Point2,
                    TEMP_Debug_Pointers.PointerType.Create_PrecisionCreate))
            {
                // Presentation mode - can't do anything except push on buttons
                //      ENABLED: poke 
                //      DISABLED: everything else

                foreach (var item in allPointers)
                {
                    if (!(item is PokePointer))
                    {
                        item.IsActive = false;
                    }
                    else
                    {
                        item.IsActive = item.IsActive;
                    }
                }
            }


            else if (TEMP_Debug_Pointers.Instance.currentPointerType == TEMP_Debug_Pointers.PointerType.Create_Default)
            {
                // Creating with hands not precision
                //      ENABLED: button poke, laser hand, grab hand, ...
                //      DISABLED: iuli precision

                foreach (var item in allPointers)
                {
                    if (item is IuliStickPointer)
                        item.IsActive = false;
                    else
                        item.IsActive = item.IsActive;

                }
            }

            else if (
                TEMP_Debug_Pointers.Instance.currentPointerType == TEMP_Debug_Pointers.PointerType.Edit_PrecisionMove ||
                TEMP_Debug_Pointers.Instance.currentPointerType == TEMP_Debug_Pointers.PointerType.Edit_SpecialPrecisionMove ||
                TEMP_Debug_Pointers.Instance.currentPointerType == TEMP_Debug_Pointers.PointerType.Edit_PrecisionScale)
            {
                // Precision mode for creating and editing
                //      ENABLED: only iuli precision & button poke
                //      DISABLED: everything else

                foreach (var item in allPointers)
                {
                    // TODO:PERF: this can be stored in a variable, there'll only be one
                    if (item is IuliStickPointer)
                    {
                        // do it this way in case it's disabled for some weird reason
                        (item as IuliStickPointer).overrideGrabPoint = TEMP_Debug_Pointers.Instance.currentPointerTipPoint;

                        // leave it as the original mediator thinks
                        item.IsActive = item.IsActive;
                    }
                    else if (item is PokePointer)
                    {
                        // leave it as the original mediator thinks
                        item.IsActive = item.IsActive;

                    }
                    else
                    {
                        item.IsActive = false;
                    }
                }
            }
        }

        else
        {
            // IF WE DON'T USE EXTERNAL CLICKER DEVICE

            // just keep mrtk defaults for now
        }

    }
}
