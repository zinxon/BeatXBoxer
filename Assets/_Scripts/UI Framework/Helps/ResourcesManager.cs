using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 資源加載管理器
    /// </summary>
    public class ResourcesManager : MonoBehaviour
    {
        private static ResourcesManager instance = null;
        private Hashtable hashtable = null;

        public static ResourcesManager GetInstance()
        {
            if (instance == null)
                instance = new GameObject("_ResourcesManager").AddComponent<ResourcesManager>();

            return instance;
        }

        private void Awake()
        {
            //字段初始化
            hashtable = new Hashtable();

            DontDestroyOnLoad(gameObject);
        }

        public T LoadResources<T>(string path, bool isCatch) where T : UnityEngine.Object
        {
            if (hashtable.Contains(path))
                return hashtable[path] as T;

            T target = Resources.Load<T>(path);

            if (target == null)
                LogManager.Error($"{GetType()}/LoadResource<T>()找不到需要提取的資源，請檢查Path = {path}");
            else if (isCatch)
                hashtable.Add(path, target);

            return target;
        }

        public GameObject LoadAsset(string path, bool isCatch)
        {
            GameObject gameObject = LoadResources<GameObject>(path, isCatch);
            GameObject cloner = Instantiate<GameObject>(gameObject);

            if (cloner == null)
                LogManager.Error($"{GetType()}/LoadAsset()/克隆資源不成功，請檢查Path = {path}");

            return cloner;
        }
    }
}
