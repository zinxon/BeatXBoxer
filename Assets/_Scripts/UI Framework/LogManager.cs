using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    public class LogManager
    {
        public static bool isOpenDebug = true;

        public static void Log(string message)
        {
            if (isOpenDebug)
                Debug.Log(message);
        }

        public static void Warning(string message)
        {
            if (isOpenDebug)
                Debug.LogWarning(message);
        }

        public static void Error(string message)
        {
            if (isOpenDebug)
                Debug.LogError(message);
        }
    }
}
