using System;

/// <summary>
/// Subset of a graph's settings that can be saved and reloaded.
/// </summary>
public class SettingsPreset
{
    public bool autoScale;
    public float yMid;
    public float yScale;
    public float xScale;
    public bool[] sensors = new bool[SettingsModel.NUM_SENSORS + 1];

    public SettingsPreset(bool autoScale, float yMid, float yScale, float xScale, bool[] sensors)
    {
        this.autoScale = autoScale;
        this.yMid      = yMid;
        this.yScale    = yScale;
        this.xScale    = xScale;

        for (int i = 0; i < sensors.Length; i++)
            this.sensors[i] = sensors[i];
    }

    public SettingsPreset(SettingsPreset s2)
    {
        this.autoScale = s2.autoScale;
        this.yMid      = s2.yMid;
        this.yScale    = s2.yScale;
        this.xScale    = s2.xScale;

        for (int i = 0; i < sensors.Length; i++)
            this.sensors[i] = s2.sensors[i];
    }

    public override String ToString()
    {
        String s = "X: " + Math.Round(xScale, 2) + "s; Y: ";

        if (autoScale) s += "Auto-Scaled";
        else           s += (yMid - yScale / 2).ToString() + " to " + (yMid + yScale / 2).ToString();

        s += ";\n Sensor(s) # ";
        for (int i = 1; i < sensors.Length; i++)
            if (sensors[i]) s += i + " ";

        return s;
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType())) return false;
        SettingsPreset that = (SettingsPreset) obj;

        for (int i = 1; i < sensors.Length; i++)
            if (this.sensors[i] != that.sensors[i]) return false;

        return this.autoScale == that.autoScale
            && this.yMid      == that.yMid
            && this.yScale    == that.yScale
            && this.xScale    == that.xScale;
    }

    public override int GetHashCode() { return base.GetHashCode(); }

    public void UpdateSettings(Settings sett)
    {
        if (sett.m.autoScale != this.autoScale) sett.m.ToggleAS(!sett.m.autoScale);

        sett.m.ChangeYMid  (this.yMid);
        sett.m.ChangeYScale(this.yScale);
        sett.m.ChangeX     (this.xScale);

        for (int i = 1; i < sensors.Length; i++)
            if (sett.m.sensors[i] != this.sensors[i]) sett.m.ToggleSensor(i, !sett.m.sensors[i]);
    }
}
