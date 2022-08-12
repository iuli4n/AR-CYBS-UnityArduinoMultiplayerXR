using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IuliExtensions
{
    public static bool In<T>(this T val, params T[] values) where T : struct
    {
        return values.Contains(val);
    }
}