using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance != null) 
                return instance;
            
            instance = FindAnyObjectByType<T>();
            if (instance == null)
            {
                Debug.LogError($"[Singleton] No instance of {typeof(T).Name} found in the scene.");
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning($"[Singleton] Another instance of {typeof(T).Name} already exists. Destroying this duplicate.");
            Destroy(gameObject);
            return;
        }
        instance = (T)this;
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}