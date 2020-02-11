using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            GameObject obj;
            obj = GameObject.Find(typeof(T).Name);
            if(instance == null)
            {
                obj = new GameObject(typeof(T).Name);
                instance = obj.AddComponent<T>();
            }
            else
            {
                instance = obj.GetComponent<T>();
            }
            return instance;
        }
    }

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
