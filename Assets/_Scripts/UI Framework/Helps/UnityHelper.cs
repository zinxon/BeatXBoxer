using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 優化Unity的功能，並供其他類使用
    /// </summary>
    public class UnityHelper : MonoBehaviour
    {
        /// <summary>
        /// 獲取子類的元件
        /// </summary>
        /// <param name="parent">父類物件</param>
        /// <param name="childName">子類名稱</param>
        /// <typeparam name="T">元件</typeparam>
        /// <returns></returns>
        public static T GetCompentFromChildNode<T>(GameObject parent, string childName) where T : Component
        {
            Transform searchTrans = null;

            searchTrans = FindChildNode(parent, childName);

            if (searchTrans != null)
            {
                T component = null;

                component = searchTrans.gameObject.GetComponent<T>();

                if (component != null)
                {
                    Debug.Log($"在{parent.name}尋找{childName}的元件成功");
                    return component;
                }

                Debug.LogError($"在{parent.name}尋找{childName}的元件失敗，請檢查");
                return component;
            }

            return null;
        }

        /// <summary>
        /// 給子類添加元件
        /// </summary>
        /// <param name="parent">父類物件</param>
        /// <param name="childName">子類名稱</param>
        /// <typeparam name="T">元件</typeparam>
        /// <returns></returns>
        public static T AddChildNodeComponent<T>(GameObject parent, string childName) where T : Component
        {
            Transform searchTrans = null;

            searchTrans = FindChildNode(parent, childName);

            if (searchTrans != null)
            {
                T[] components = searchTrans.GetComponents<T>();

                for (int i = 0; i < components.Length; i++)
                {
                    Destroy(components[i]);
                }

                Debug.Log($"在{parent.name}為{childName}添加元件成功");
                return searchTrans.gameObject.AddComponent<T>();
            }

            Debug.Log($"在{parent.name}為{childName}添加元件失敗，請檢查");
            return null;
        }

        /// <summary>
        /// 將子類移至父類裏
        /// </summary>
        /// <param name="parent">父類位置</param>
        /// <param name="child">子類位置</param>
        public static void AddChildNodeToParentNode(Transform parent, Transform child)
        {
            child.SetParent(parent);
            child.localPosition = Vector3.zero;
            child.localEulerAngles = Vector3.zero;
            child.localScale = Vector3.one;
        }

        /// <summary>
        /// 查找子類的位置
        /// </summary>
        /// <param name="parent">父類物件</param>
        /// <param name="childName">子類名稱</param>
        /// <returns></returns>
        public static Transform FindChildNode(GameObject parent, string childName)
        {
            Transform searchTrans = null;

            searchTrans = parent.transform.Find(childName);

            if (searchTrans == null)
            {
                foreach (Transform trans in parent.transform)
                {
                    searchTrans = FindChildNode(trans.gameObject, childName);

                    if (searchTrans != null)
                    {
                        //Debug.Log($"在{parent.name}尋找{childName}成功");
                        return searchTrans;
                    }
                }

                //Debug.LogError($"在{parent.name}尋找{childName}失敗，請檢查");
                return searchTrans;
            }

            //Debug.Log($"在{parent.name}尋找{childName}成功");
            return searchTrans;
        }
    }
}
