using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace UIFramework
{
    public class EventTriggerListener : EventTrigger
    {
        public event Action<object> onClick;
        public event Action<object> onDown;
        public event Action<object> onEnter;
        public event Action<object> onExit;
        public event Action<object> onUp;
        public event Action<object> onSelect;
        public event Action<object> onUpdateSelect;

        public static EventTriggerListener GetListener(GameObject gameObject)
        {
            EventTriggerListener listener = gameObject.GetComponent<EventTriggerListener>();

            if (listener == null)
                listener = gameObject.AddComponent<EventTriggerListener>();

            return listener;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null)
                onClick(gameObject);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onDown != null)
                onDown(gameObject);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onEnter != null)
                onEnter(gameObject);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onExit != null)
                onExit(gameObject);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onUp != null)
                onUp(gameObject);
        }

        public override void OnSelect(BaseEventData eventBaseData)
        {
            if (onSelect != null)
                onSelect(gameObject);
        }

        public override void OnUpdateSelected(BaseEventData eventBaseData)
        {
            if (onUpdateSelect != null)
                onUpdateSelect(gameObject);
        }
    }
}