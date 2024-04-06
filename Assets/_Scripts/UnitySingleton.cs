using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 單例模式的父類。
/// 為需要使用單例模式的類，提供一個基本的父類並整合一些功能。
/// </summary>
/// <typeparam name="T">泛型(需要是帶有 MonoBehaviour 的類)</typeparam>
public class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;

    /// <summary>
    /// 取得實例。
    /// 如果instance為空，自動創建一個新的實例。
    /// </summary>
    /// <returns></returns>
    public static T GetInstance()
    {
        if (instance == null)
            instance = new GameObject($"_{typeof(T).ToString()}").AddComponent<T>();
        return instance;
    }

    /// <summary>
    /// 進行初始化。
    /// </summary>
    protected virtual void Awake()
    {
        //如果instance不為空，刪除其他同類的對象，已確保遊戲中只有這一實例，
        //否則它成為實例。
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as T;
        }

        //如果IsDontDestroyObjectOnLoad()是true，使用DontDestroyOnLoad(Object)，
        //確保它不會被刪除。
        if (IsDontDestroyObjectOnLoad())
            DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 當加載至其他場景時，是否不刪除這個對象+類。
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsDontDestroyObjectOnLoad()
    {
        return false;
    }
}
