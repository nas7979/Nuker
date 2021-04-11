using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

/*
 * 
 * Usage
 * 
 * Input Touch Event
 * 
 * To use InputController Class, Necessary have EventSystem, Image Panel
 * 
 * OnPointerDown : Touch Down
 * 
 * OnDrag : Touch Drag
 * 
 * OnPointerUp : Touch Up
 */

namespace Game.INPUT
{
    public enum TouchState
    {
        TOUCH_NONE,
        TOUCH_DOWN,
        TOUCH_DRAG,
        TOUCH_UP
    }
    public class InputController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public static TouchState touchState = TouchState.TOUCH_NONE;

        private Vector3 _inputVector;
        public Vector3 InputVector
        {
            get
            {
                return _inputVector;
            }
        }

        private Vector3 _nextInputVector;
        public Vector3 NextInputVector
        {
            get
            {
                return _nextInputVector;
            }
        }
        protected static event Action<Vector3> Input_DownListener;
        protected static event Action<Vector3> Input_DragListener;
        protected static event Action<Vector3> Input_UpListener;

        #region InputFunctions class : InputController
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _inputVector = Input.mousePosition;
            _inputVector = Camera.main.ScreenToWorldPoint(_inputVector);

            Input_DownListener?.Invoke(_inputVector);
            touchState = TouchState.TOUCH_DOWN;
        }
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            _nextInputVector = Input.mousePosition;
            _nextInputVector = Camera.main.ScreenToWorldPoint(_nextInputVector);

            Input_DragListener?.Invoke(_nextInputVector);
            touchState = TouchState.TOUCH_DRAG;
        }
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _inputVector = Input.mousePosition;
            _inputVector = Camera.main.ScreenToWorldPoint(_inputVector);

            Input_UpListener?.Invoke(_inputVector);
            touchState = TouchState.TOUCH_UP;
        }
        #endregion
    }
}