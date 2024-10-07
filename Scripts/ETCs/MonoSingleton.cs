using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static object lockObject = new object();
    private static bool IsQuitting = false;
    private static T _instance;

    public static T Instance
    {
        get
        {
            // 한번에 한 스래드만 lock블럭 실행
            lock (lockObject)
            {
                // 비활성화 됐다면 기존꺼 내비두고 새로 만든다.
                if ( IsQuitting )
                {
                    return null;
                }

                // instance가 NULL일때 새로 생성한다.
                if (_instance == null )
                {
                    _instance = GameObject.Instantiate(Resources.Load<T>("MonoSingleton/" + typeof(T).Name));
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
    }
}