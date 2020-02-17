using System.Collections;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool IsInitialized { get; private set; }
    private static T _instance = null;
    private static object _syncobj = new object();
    //private static bool appIsClosing = false;

    public static T Instance
    {
        get
        {
            //if (appIsClosing)
            //    return null;

            lock (_syncobj)
            {
                if (_instance == null)
                {
                    
                    T[] objs = FindObjectsOfType<T>();
                    if (objs.Length > 0)
                        _instance = objs[0];

                    //if(objs.Length > 1)

                    if (_instance == null)
                    {
                        string goName = typeof(T).ToString();
                        GameObject go = GameObject.Find(goName);
                        if (go == null)
                            go = new GameObject(goName);
                        _instance = go.AddComponent<T>();

                    }

                }
                return _instance;
            }
        }
    }

    protected virtual void OnApplicationQuit()
    {
        //appIsClosing = true;
    }

    public void Initialize()
    {
        if (IsInitialized) return;
        InitializeChild();
        IsInitialized = true;
    }

    public void Initialize<P>(P param) where P : class
    {
        if (IsInitialized) return;
        InitializeChild<P>(param);
        IsInitialized = true;
    }                            

    protected virtual void InitializeChild() { }
    protected virtual void InitializeChild<P>(P arg) where P : class{ }
}