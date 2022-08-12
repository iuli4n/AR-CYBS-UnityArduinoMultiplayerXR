
/// <summary>
/// Simulates the state of a button (ie: simulates Input.GetMouseButton())
/// Output: you can query this object to see if the button is clicked or not; and if it's been clicked in the current frame or not
/// Input: a listener that responds to updates of a boolean value (whether button is clicked or not)
/// </summary>
public class IuliButtonSimulator
{
    // the current state
    public MouseButtonState buttonState = MouseButtonState.NotInitialized;
    public enum MouseButtonState
    {
        NotInitialized,
        Down_JustNow,
        Down_Longer,
        Up_JustNow,
        Up_Longer
        
    }
    // True if button is down
    public bool Clicked { get { return buttonState == MouseButtonState.Down_Longer || buttonState == MouseButtonState.Down_JustNow; } }
    // True if button is down on this frame
    public bool NowClicked { get { return buttonState == MouseButtonState.Down_JustNow; } }
    // True if button is released on this frame
    public bool NowUnclicked { get { return buttonState == MouseButtonState.Up_JustNow; } }

    // For timing, keep track of what state we haven't processed yet
    private bool needToProcessUpdate = false;
    private bool latestUpdateValue;
    private bool firstFrame = true; // we need to set our intial state if we're on the first frame


    // call this when the button changes or when you get an update from the button (ie: it can contain the same value as previous call)
    public void OnButtonChange(bool buttonIsDown)
    {
        if (buttonIsDown == latestUpdateValue)
        {
            // same as before, so don't do anything
            return;
        }

        // value actually changed so we'll need to process it
        latestUpdateValue = buttonIsDown;
        needToProcessUpdate = true;
    }

    // call this when a frame has passed. This does all the updates to the button
    public void Tick()
    {
        if (firstFrame || needToProcessUpdate)
        {
            // if we're here it means the server value actually changed since last tick
            
            // spend this frame processing the update if we need to shift into a down_justnow or up_justnow
            // note, since the value actually changed, this will probably always move 
            if (latestUpdateValue) { if (!Clicked) buttonState = MouseButtonState.Down_JustNow; }
            else { if (Clicked) buttonState = MouseButtonState.Up_JustNow; }

            needToProcessUpdate = false;
        }
        else
        {
            // note, if we got a server value update, this code will execute after 2 Tick (since the first tick is for responding to the update)

            if (buttonState == MouseButtonState.Down_JustNow) buttonState = MouseButtonState.Down_Longer;
            else if (buttonState == MouseButtonState.Up_JustNow) buttonState = MouseButtonState.Up_Longer;
        }

        firstFrame = false;
    }
}
