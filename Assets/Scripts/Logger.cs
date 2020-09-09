using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
    public static void Error(string text)
    {
        Debug.LogError(text);
    }

    public static void Warn(string text)
    {
        Debug.LogWarning(text);
    }

    public static void Log(string text)
    {
        Debug.Log(text);
    }

    public static void DevError(string text)
    {
        #if (DEBUG)
            Debug.LogError("[DEBUG] "+text);
        #endif
    }

    public static void DevWarn(string text)
    {
        #if (DEBUG)
            Debug.LogWarning("[DEBUG] " + text);
        #endif
    }

    public static void DevLog(string text)
    {
        #if (DEBUG)
            Debug.Log("[DEBUG] " + text);
        #endif
    }
}
