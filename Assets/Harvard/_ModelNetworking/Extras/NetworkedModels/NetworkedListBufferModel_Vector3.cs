using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NetworkedListBufferModel_Vector3 : GenericNetworkedListBufferModel<Vector3>//Tuple<float, float, float>>
{
    public static Vector3 TupleToVector3(Tuple<float, float, float> t) { return new Vector3(t.Item1, t.Item2, t.Item3); }
    public static Tuple<float, float, float> Vector3ToTuple(Vector3 v) { return new Tuple<float, float, float>(v.x, v.y, v.z); }
}