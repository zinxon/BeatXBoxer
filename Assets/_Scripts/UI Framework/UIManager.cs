using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance = null;

        private Dictionary<string, string> uiPathDic;
        private Dictionary<string, UIBaseClass> allUIDic;
        private Dictionary<string, UIBaseClass> curShowUIDic;
        private Stack<UIBaseClass> curUIStack;

        private Transform canvasNode, normalNode, fixedNode, popupNode = null;
        private Transform scriptsMgrNode = null;

        public static UIManager GetInstance()
        {
            if (instance == null)
                instance = new GameObject("_UIManager").AddComponent<UIManager>();

            return instance;
        }

        private void Awake()
        {
            InitUIManager();
        }

        private void InitUIManager()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }

            DontDestroyOnLoad(gameObject);

            uiPathDic = new Dictionary<string, string>();
            allUIDic = new Dictionary<string, UIBaseClass>();
            curShowUIDic = new Dictionary<string, UIBaseClass>();
            curUIStack = new Stack<UIBaseClass>();

            Canvas obj = GameObject.FindObjectOfType<Canvas>();

            if(obj == null || !obj.gameObject.CompareTag(UIConstDefines.SYS_TAG_CANVAS)){
               InitMainCanvas();
            }

            canvasNode = GameObject.FindGameObjectWithTag(UIConstDefines.SYS_TAG_CANVAS).transform;
            normalNode = UnityHelper.FindChildNode(canvasNode.gameObject, UIConstDefines.SYS_NORMAL_NODE);
            fixedNode = UnityHelper.FindChildNode(canvasNode.gameObject, UIConstDefines.SYS_FIXED_NODE);
            popupNode = UnityHelper.FindChildNode(canvasNode.gameObject, UIConstDefines.SYS_POPUP_NODE);
            scriptsMgrNode = UnityHelper.FindChildNode(canvasNode.gameObject, UIConstDefines.SYS_SCRIPTS_MANAGER_NODE);


            transform.SetParent(scriptsMgrNode, false);
            DontDestroyOnLoad(canvasNode);
            InitUIPathData();
        }

        public void ShowUI(string uiName)
        {
            if (string.IsNullOrEmpty(uiName))
            {
                LogManager.Error("UI名字為空，請檢查!!!");
                return;
            }

            UIBaseClass ui = LoadUIFromAllUIDictionary(uiName);
            if (ui == null)
                return;

            switch (ui.uIType.showMode)
            {
                case UIShowMode.Normal:
                    LoadUIToCurrentCache(uiName);
                    break;
                case UIShowMode.ReverseChange:
                    break;
                case UIShowMode.HideOther:
                    break;
                default:
                    break;
            }
        }

        private void LoadUIToCurrentCache(string uiName)
        {
            UIBaseClass ui = null;
            UIBaseClass cacheUI = null;

            curShowUIDic.TryGetValue(uiName, out ui);
            if (ui != null)
            {
                LogManager.Log($"{uiName}正在顯示中");
                return;
            }

            allUIDic.TryGetValue(uiName, out cacheUI);
            if (cacheUI != null)
            {
                curShowUIDic.Add(uiName, cacheUI);
                cacheUI.Display();
            }
        }

        public void CloseUI(string uiName)
        {
            if (string.IsNullOrEmpty(uiName))
            {
                LogManager.Error("UI名字為空，請檢查!!!");
                return;
            }

            UIBaseClass ui = LoadUIFromAllUIDictionary(uiName);
            if (ui == null)
                return;

            switch (ui.uIType.showMode)
            {
                case UIShowMode.Normal:
                    ShutUI(uiName);
                    break;
                case UIShowMode.ReverseChange:
                    break;
                case UIShowMode.HideOther:
                    break;
                default:
                    break;
            }
        }

        private void ShutUI(string uiName)
        {
            UIBaseClass ui = null;

            curShowUIDic.TryGetValue(uiName, out ui);
            if (ui == null)
            {
                LogManager.Log($"UI早已不再顯示，請檢查");
                return;
            }

            ui.Hide();
            curShowUIDic.Remove(uiName);
        }

        private UIBaseClass LoadUIFromAllUIDictionary(string uiName)
        {
            UIBaseClass ui = null;

            allUIDic.TryGetValue(uiName, out ui);

            if (ui == null)
                ui = LoadUI(uiName);

            return ui;
        }

        private UIBaseClass LoadUI(string uiName)
        {
            UIBaseClass ui = null;
            string uiPath = null;
            GameObject tempObject = null;

            uiPathDic.TryGetValue(uiName, out uiPath);

            if (!string.IsNullOrEmpty(uiPath))
            {
                tempObject = ResourcesManager.GetInstance().LoadAsset(uiPath, false);
            }

            if (canvasNode != null && tempObject != null)
            {

                ui = tempObject.GetComponent<UIBaseClass>();

                if (ui == null)
                {
                    LogManager.Error($"{uiName}無法取得UI元件，請檢查{uiName}是為加載了UI元件");
                    return null;
                }

                switch (ui.uIType.baseType)
                {
                    case UIBaseType.Normal:
                        tempObject.transform.SetParent(normalNode, false);
                        break;
                    case UIBaseType.Fixed:
                        tempObject.transform.SetParent(fixedNode, false);
                        break;
                    case UIBaseType.Popup:
                        tempObject.transform.SetParent(popupNode, false);
                        break;
                    default:
                        break;
                }
            }

            tempObject.SetActive(false);
            allUIDic.Add(uiName, ui);

            return ui;
        }

        private void InitMainCanvas()
        {
            ResourcesManager.GetInstance().LoadAsset(UIConstDefines.SYS_PATH_MAIN_CANVAS, false);
        }

        private void InitUIPathData()
        {
            IConfig config = new ConfigManager(UIConstDefines.SYS_PATH_UIFORMS_CONFIG_INFO);

            if (config != null)
                uiPathDic = config.ConfigSettingDic;
        }
    }
}