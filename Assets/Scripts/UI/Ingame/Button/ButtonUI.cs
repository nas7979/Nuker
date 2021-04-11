using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace InGame.Manager
{
    [Serializable]
    public struct ListenerTable
    {
        public ButtonListener listener;
    }
    [Serializable]
    public class ButtonPackage
    {
        public string name;
        [Tooltip("버튼 클릭에 따른 Action을 코드에서 처리할 것인가(true) Editor를 통해서 처리를 할 것인가(false)")]
        public bool IsCustomOn;
        public float delay = 0f;
        public Button button;
        [Header("OnClick")]
        public UnityEvent buttonEvent;

        [Header("Event Trigger")]
        public EventTrigger eventTrigger;
        public List<EventTrigger.Entry> entries;

        public UnityAction action;
        [SerializeField] List<ListenerTable> _listeners;

        public ButtonPackage(string name, bool IsCustomOn,
            Button button, UnityEvent buttonEvent, List<ListenerTable> _listeners,  UnityAction action)
        {
            this.name = name;
            this.IsCustomOn = IsCustomOn;
            this.button = button;
            this.buttonEvent = buttonEvent;
            this._listeners = _listeners;
            this.action = action;
        }
        public ButtonPackage(string name, bool IsCustomOn,
            Button button, UnityEvent buttonEvent, EventTrigger eventTrigger
            , List<ListenerTable> _listeners, UnityAction action)
        {
            this.name = name;
            this.IsCustomOn = IsCustomOn;
            this.button = button;
            this.buttonEvent = buttonEvent;
            this.eventTrigger = eventTrigger;
            this._listeners = _listeners;
            this.action = action;
        }
        public ButtonPackage(ButtonPackage package, UnityAction action)
        {
            this.name = package.name;
            this.IsCustomOn = package.IsCustomOn;
            this.button = package.button;
            this.buttonEvent = package.buttonEvent;
            this.eventTrigger = package.eventTrigger;
            this.action = action;
            this._listeners = package._listeners;
        }
        public IEnumerable<ListenerTable> GetListeners() => _listeners;
    }
    
    [Serializable]
    public class ButtonUI
    {
        private static Dictionary<string, ButtonPackage> buttonEvents;

        public static bool IsExist<T>(T element)
        {
            if (element != null)
                return true;
            return false;
        }
        public static bool AppendEvent(ButtonPackage package)
        {
            buttonEvents = buttonEvents ?? new Dictionary<string, ButtonPackage>();

            package.button?.onClick.AddListener(package.action);

            package.GetListeners()?.ToList().ForEach(e => {
                e.listener.GetComponent<Button>().onClick.AddListener(package.action);
            });

            if (!FindEvent(package.name))
            {
                buttonEvents.Add(package.name, package);
                return true;
            }
            return false;
        }

        public static ButtonPackage FindPackage(string name)
        {
            if (FindEvent(name))
            {
                return buttonEvents[name];
            }
            return null;
        }

        public static bool FindEvent(string name)
        {
            if (!IsExist(buttonEvents))
                return false;
            if (buttonEvents.ContainsKey(name))
                return true;
            return false;
        }
    }
    public abstract class ButtonEvent : MonoBehaviour
    {
        [SerializeField] List<ButtonPackage> Button;

        protected void Start()
        {
            Button?.Where(e => e.IsCustomOn == false).ToList().ForEach(e => {
                ButtonUI.AppendEvent(new ButtonPackage(e, () => {
                    StartCoroutine(DelayForEvent(e.delay, e.buttonEvent));
                }));
                if (e.eventTrigger)
                    e.eventTrigger.triggers = e.entries;
            });
            
            OnStartToInit();
        }

        public IEnumerator DelayForEvent(float time, UnityEvent unityEvent)
        {
            yield return new WaitForSecondsRealtime(time);
            unityEvent.Invoke();
        }
        protected virtual void OnStartToInit()
        {

        }
    }
}