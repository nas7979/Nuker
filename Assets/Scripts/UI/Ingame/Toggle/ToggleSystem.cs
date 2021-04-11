using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Toggle
{
    using Toggle = UnityEngine.UI.Toggle;

    [RequireComponent(typeof(Toggle))]
    public class ToggleSystem : MonoBehaviour
    {
        private Toggle _toggle;
        public Toggle Toggle
        {
            get
            {
                if (_toggle == null)
                    _toggle = GetComponent<Toggle>();
                return _toggle;
            }
        }

        [SerializeField] private Sprite _On;
        public Sprite On
        {
            get
            {
                if (_On == null)
                    Debug.Assert(false, $"NullReference type : {_On.GetType().Name}");
                return _On;
            }
        }
        [SerializeField] private Sprite _Off;
        public Sprite Off
        {
            get
            {
                if (_Off == null)
                    Debug.Assert(false, $"NullReference type : {_Off.GetType().Name}");
                return _Off;
            }
        }

        private Image toggleImage = null;

        private event UnityAction<bool> GetValueChangedAction;
        public void AddValueChangedListener(UnityAction<bool> unityAction)
            => GetValueChangedAction += unityAction;

        //Initalize
        protected void Awake()
        {
            AddValueChangedListener(ChangedState);
        }

        protected virtual void OnEnable()
        {
            Toggle.onValueChanged.AddListener(GetValueChangedAction);
            toggleImage = (Image)Toggle.targetGraphic;
        }
        protected virtual void OnDisable()
        {
            GetValueChangedAction = delegate { };
            Toggle.onValueChanged.RemoveListener(GetValueChangedAction);
        }

        internal virtual void ChangedState(bool state)
        {
            toggleImage.sprite = (state) ? On : Off;
            Toggle.isOn = state;
        }
    }
}