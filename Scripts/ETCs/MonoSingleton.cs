using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            // instance가 NULL일때 새로 생성한다.
            if (_instance == null)
            {
                _instance = GameObject.Instantiate(Resources.Load<T>("MonoSingleton/" + typeof(T).Name));
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

}