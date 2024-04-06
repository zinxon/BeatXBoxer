using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    /// <summary>
    /// UI父類
    /// </summary>
    public class UIBaseClass : MonoBehaviour
    {
        public UIType uIType { get; set; } = new UIType();
        public bool IsShowed { get; private set; }

        public virtual void Display()
        {
            gameObject.SetActive(true);
        }

        public virtual void ReDisplay()
        {
            gameObject.SetActive(true);
        }

        public virtual void Freeze()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        protected virtual void ShowUI(string uiName)
        {
            UIManager.GetInstance().ShowUI(uiName);
        }

        protected virtual void CloseUI(string uiName)
        {
            UIManager.GetInstance().CloseUI(uiName);
        }

        protected void RigisterButtonOnEnterEvent(string objectName, Action<object> action)
        {
            GameObject button = UnityHelper.FindChildNode(gameObject, objectName).gameObject;
            
            if (button != null)
                EventTriggerListener.GetListener(button).onEnter += action;
        }

        protected void RigisterButtonOnClick(string buttonName, Action<object> action)
        {
            Button button = UnityHelper.FindChildNode(gameObject, buttonName).GetComponent<Button>();

            if (button != null)
                button.onClick.AddListener(() => action(gameObject));
        }

        protected void RigisterButtonBaseEvent(string buttonName)
        {
            GameObject button = UnityHelper.FindChildNode(gameObject, buttonName).gameObject;

            if (button != null)
            {
                Animator animator = button.GetComponent<Animator>();

                if (animator != null)
                    EventTriggerListener.GetListener(button).onEnter += p => PlayButtonAnimation(animator, "NormalToPressed", "DissolveToNormal");
                    EventTriggerListener.GetListener(button).onExit += p => PlayButtonAnimation(animator, "NormalToPressed", "NormalToDissolve");
            }
        }

        private void PlayButtonAnimation(Animator animator, string notPlayAnimName, string playAnimName)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(notPlayAnimName))
                animator.Play(playAnimName);
        }

        protected IEnumerator RefreshContentSizeFitter()
        {
            yield return null;

            ContentSizeFitter[] coms = gameObject.GetComponentsInChildren<ContentSizeFitter>();
            foreach (var item in coms)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(item.GetComponent<RectTransform>());
            }
        }
    }
}
