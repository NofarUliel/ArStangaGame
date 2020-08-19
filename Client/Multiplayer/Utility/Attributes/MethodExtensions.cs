using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MethodExtensions
{
    public static string RemoveQuotes(this string value)
    {
        return value.Replace("\"", "");
    }
    public static float ThreeDecimals(this float value)
    {
        return Mathf.Round(value * 1000.0f) / 1000.0f;
    }
}
