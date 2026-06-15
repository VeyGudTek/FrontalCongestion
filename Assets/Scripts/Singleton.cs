using UnityEngine;
using System;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T Instance { get; set; }

    public static T GetInstance()
    {
        if (Instance == null)
        {
            throw new NullReferenceException("Singleton not initialized");
        }
        return Instance;
    }

    protected void IntializeSingleton(T instance)
    {
        if (Instance != null && Instance != instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = instance;
    }
}
