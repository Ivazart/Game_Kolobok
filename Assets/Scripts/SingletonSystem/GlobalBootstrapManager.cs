using UnityEngine;
using System.Collections.Generic;

public class GlobalBootstrapManager : MonoBehaviour
{
    private static bool isInitialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void InitializeOnLoad()
    {
        if (isInitialized) return;
        isInitialized = true;

        var globalPrefab = Resources.Load<GameObject>("GlobalSingletons");
        if (globalPrefab == null)
        {
            Debug.LogError("GlobalBootstrapPrefab not found in Resources!");
            return;
        }

        var globalInstance = Instantiate(globalPrefab);
        DontDestroyOnLoad(globalInstance);
    }
}