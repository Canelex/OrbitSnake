using UnityEngine;

/* Inherit from this base class to create a singleton.
 * e.g. public class MyClassName : Singleton<MyClassName> {} */
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static bool isReady;

    /* Access singleton instance through this propriety. */
    public static T Instance
    {
        get { return instance; }
    }

    public static bool IsReady
    {
        get { return isReady; }
        set {isReady = value; }
    }

    public void Init() {}

    public void Awake()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<T>();
            DontDestroyOnLoad(instance.gameObject);
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Removed copy of singleton");
        }
    }
}