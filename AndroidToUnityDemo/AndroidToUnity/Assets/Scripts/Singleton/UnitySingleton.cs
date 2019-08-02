using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitySingleton<T> : MonoBehaviour where T : Component
{

    static T Instance;

    // 继承与Mono
	protected virtual void Awake()
    {
        if (Instance)
        {
            // 立即删除该对象
            DestroyImmediate(this);
        }
        else
        {
            // 切换场景不会被删除
            DontDestroyOnLoad(this);
            Instance = this as T;
        }
    }

}

// 不继承Mono的单例
public abstract class Singleton<T> where T : new()
{
    private static T _instance;
    static object _lock = new object();
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new T();
                }
            }
            return _instance;
        }
    }
}












