using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilLog
{ 
    public static void Error(object str)
    {
        UnityEngine.Debug.LogError(str);
    }

    public static void Warning(object str)
    {
        UnityEngine.Debug.LogWarning(str);
    }

    public static void Log(object str)
    {
        UnityEngine.Debug.Log(str);
    }
}
