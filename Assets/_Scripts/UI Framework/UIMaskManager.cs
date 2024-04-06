using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{

    public class UIMaskManager : MonoBehaviour
    {
        private const float TRANS_COLOR_RGB = 1f;
        private const float TRANS_COLOR_A = 1f;
        private const float IMPENETRABLE_COLOR_RGB = 0.3f;
        private const float IMPENETRABLE_COLOR_A = 0.8f;

        private static UIMaskManager instance = null;

        private Transform scriptsMgrNode = null;
        private GameObject mainCanvas, topPanel, uiMask = null;
        private Camera uiCam;
        private float oriUICamDepth;

        public static UIMaskManager GetInstance()
        {
            if (instance == null)
                instance = new GameObject("_UIMaskManager").AddComponent<UIMaskManager>();

            return instance;
        }

        private void Start()
        {
            InitUIMask();
        }

        private void InitUIMask()
        {
            mainCanvas = GameObject.FindGameObjectWithTag(UIConstDefines.SYS_TAG_CANVAS);
            scriptsMgrNode = UnityHelper.FindChildNode(mainCanvas.gameObject, UIConstDefines.SYS_SCRIPTS_MANAGER_NODE);

            topPanel = mainCanvas;
            uiMask = UnityHelper.FindChildNode(mainCanvas, UIConstDefines.UI_MASK).gameObject;

            uiCam = GameObject.FindGameObjectWithTag(UIConstDefines.SYS_TAG_UI_CAMERA).GetComponent<Camera>();
            if (uiCam != null)
            {
                oriUICamDepth = uiCam.depth;
            }
            else
            {
                LogManager.Error($"找不到UI Camera，請檢查!!!");
            }
        }

        public void OpenUIMask(GameObject ui, UITransparency UITrans = UITransparency.Lucency)
        {
            if (!uiMask)
            {
                LogManager.Error($"找不到UI Mask!!!");
                return;
            }

            if (topPanel)
                topPanel.transform.SetAsLastSibling();

            switch (UITrans)
            {
                case UITransparency.Lucency:
                    SetUIMaskColor(new Color(TRANS_COLOR_RGB, TRANS_COLOR_RGB, TRANS_COLOR_RGB, TRANS_COLOR_A));
                    break;
                case UITransparency.Translucence:
                    SetUIMaskColor(new Color(TRANS_COLOR_RGB, TRANS_COLOR_RGB, TRANS_COLOR_RGB, TRANS_COLOR_A));
                    break;
                case UITransparency.Impenetrable:
                    SetUIMaskColor(new Color(IMPENETRABLE_COLOR_RGB, IMPENETRABLE_COLOR_RGB, IMPENETRABLE_COLOR_RGB, IMPENETRABLE_COLOR_A));
                    break;
                case UITransparency.Penetrate:
                    if (uiMask.activeInHierarchy)
                        uiMask.SetActive(false);
                    break;
                default:
                    break;
            }

            uiMask.transform.SetAsLastSibling();
            ui.transform.SetAsLastSibling();
            if (uiCam != null)
                uiCam.depth += 100;
        }

        public void CloseUIMask()
        {
            topPanel.transform.SetAsFirstSibling();

            if (uiMask.activeInHierarchy)
                uiMask.SetActive(false);

            if (uiCam != null)
                uiCam.depth = oriUICamDepth;
        }

        private void SetUIMaskColor(Color color)
        {
            if (uiMask)
            {
                uiMask.SetActive(true);

                Image img = uiMask.GetComponent<Image>();
                if (img)
                    img.color = color;
            }
        }
    }
}
