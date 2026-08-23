using UnityEngine;
using System;
public class SingletonGame<T> : MonoBehaviour where T : SingletonGame<T>
{
    private static T _instance;
    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null) 
                {
                    _instance = FindFirstObjectByType<T>();
                }
                return _instance;
            }
        }
    }
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else if (_instance != null)
        {
            Debug.LogWarning($"[{typeof(T).Name}] Duplicate instance found. Destroying: {gameObject.name}");
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy() 
    {
        if (_instance != null) 
        {
            _instance = null;
        }
    }
}
