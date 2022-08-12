/// <summary>
/// Represents data point (float) with associated time (seconds).
/// </summary>
public struct DataFloat
{
    public DataFloat(float f, float t)
    {
        F = f; T = t;
    }

    public float F { get; set; }
    public float T { get; set; }
}
