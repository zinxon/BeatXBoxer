using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickEffect : MonoBehaviour
{
    public GameObject prefab;
    private RectTransform rectTransform
    {
        get
        {
            return transform as RectTransform;
        }
    }

    private Dictionary<int, Queue<PoolObjectInstance>> poolDic;
    private Dictionary<int, bool> poolExpandDic;

    private void Start()
    {
        poolDic = new Dictionary<int, Queue<PoolObjectInstance>>();
        poolExpandDic = new Dictionary<int, bool>();
        CreatePool(prefab);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 localpoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localpoint);

            ReuseObject(prefab, localpoint, Quaternion.identity);
        }

        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Vector2 localpoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.touches[0].position, null, out localpoint);

            ReuseObject(prefab, localpoint, Quaternion.identity);
        }
    }

    public void CreatePool(GameObject prefab, int poolSize = 3, bool shouldExpand = true)
    {
        if (poolSize <= 0)
            poolSize = 1;

        //取得預製體的ID
        int poolKey = prefab.GetInstanceID();

        // 設置一個父類來對預製體進行管理
        GameObject poolHolder = SetPoolHolder(prefab);

        //如果"poolDic"也找不到這個預製體的ID，進行以下操作
        if (!poolDic.ContainsKey(poolKey))
        {
            //"poolDic"為這個預製體新增保存位置
            poolDic.Add(poolKey, new Queue<PoolObjectInstance>());
            //使用"poolExpandDic"來對這它"是否會"擴展""進行記錄
            poolExpandDic.Add(poolKey, shouldExpand);

            //創建特定數目的預製體並設置父類
            for (int i = 0; i < poolSize; i++)
            {
                PoolObjectInstance obj = new PoolObjectInstance(Instantiate(prefab) as GameObject);
                poolDic[poolKey].Enqueue(obj);//可以改為LIST
                obj.SetParent(poolHolder.transform);
            }
        }
    }

    public void ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        //取得預製體的ID
        int poolKey = prefab.GetInstanceID();

        //如果ObjectPool存在着未顯示的"prefab"，便將它重用
        if (poolDic.ContainsKey(poolKey))
        {
            for (int i = 0; i < poolDic[poolKey].Count; i++)
            {
                //取得預製體
                PoolObjectInstance objToReuse = poolDic[poolKey].Peek();

                //如果這個預製體尚未顯示，進行以下操作
                if (!objToReuse.IsSetActive())
                {
                    //為預製體重新排列
                    poolDic[poolKey].Dequeue();//可以改為LIST
                    poolDic[poolKey].Enqueue(objToReuse);//可以改為LIST
                    //重置預製體的位置
                    objToReuse.Reuse(position, rotation);
                    //跳出這個方法，防止執行以下代碼
                    return;
                }
            }
        }

        //如果這個預製件能擴展，ObjectPool的"prefab"也全部顯示，便進行擴展
        if (poolExpandDic[poolKey] == true)
        {
            //創建預製體
            PoolObjectInstance obj = new PoolObjectInstance(Instantiate(prefab) as GameObject);
            //為預製體重新排列
            poolDic[poolKey].Enqueue(obj);//
            //設置prefab父類
            obj.SetParent(SetPoolHolder(prefab).transform);
            //重置預製體的位置
            obj.Reuse(position, rotation);
        }
    }

    private GameObject SetPoolHolder(GameObject prefab)
    {
        GameObject poolHolder = null;
        string poolHolderName = prefab.name + " Pool";

        poolHolder = GameObject.Find(poolHolderName);

        if (poolHolder == null)
        {
            poolHolder = new GameObject(poolHolderName, typeof(RectTransform));
            poolHolder.transform.SetParent(transform, false);

            RectTransform rect = poolHolder.GetComponent<RectTransform>();
            rect.anchoredPosition = rect.anchorMax;
        }

        return poolHolder;
    }

    public class PoolObjectInstance
    {
        private GameObject obj;
        private Transform trans;
        private RectTransform rectTrans;

        private bool hasPoolObjComponent;
        private PoolObject poolObject;

        public PoolObjectInstance(GameObject gameObject)
        {
            obj = gameObject;
            trans = obj.transform;
            rectTrans = obj.GetComponent<RectTransform>();
            obj.SetActive(false);

            if (obj.GetComponent<PoolObject>())
                poolObject = obj.GetComponent<PoolObject>();
        }

        public void Reuse(Vector3 position, Quaternion rotation)
        {
            if (poolObject)
                poolObject.OnObjectReuse();

            obj.SetActive(true);
            rectTrans.anchoredPosition = position;
            //trans.position = position;
            trans.rotation = rotation;
        }

        public void SetParent(Transform parent)
        {
            trans.SetParent(parent, false);
        }

        public bool IsSetActive()
        {
            return obj.activeInHierarchy;
        }
    }
}
