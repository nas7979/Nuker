// original : https://github.com/maydinunlu/virtual-joystick-unity

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum JoystickState
{
    TOUCH_NONE,
    TOUCH_DOWN,
    TOUCH_DRAG,
    TOUCH_UP
}
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    enum JoystickType
    {
        Fixed,
        Floating,
    }

    [HideInInspector]
    public JoystickState touchState = JoystickState.TOUCH_NONE;

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    JoystickType joystickType = JoystickType.Fixed;

    [SerializeField]
    Image imageBg;

    [SerializeField]
    Image imageJoystick;

    private Vector3 _inputNomralizeVector;
    public Vector3 InputNormalizeVector
    {
        get
        {
            return this._inputNomralizeVector;
        }
    }
    public Vector3 InputDir { get; set; }
    public Vector3 InputVector { get; set; }

    public float FaceAngel = 0f;
	[SerializeField]
	float Sensitivity = 0.9f;

	private bool m_bEnableTouch = true;
    public bool EnableTouch { get => m_bEnableTouch; set => m_bEnableTouch = value; }
    public bool IsEnableTouch() => (EnableTouch) ? true : false; 

    void MoveJoystickToCurrentTouchPosition()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.canvas.transform as RectTransform, Input.mousePosition, this.canvas.worldCamera, out pos);
        this.imageBg.rectTransform.position = this.canvas.transform.TransformPoint(pos);
    }


    public void OnPointerDown(PointerEventData e)
    {
        if (!IsEnableTouch())
            return;

        if (PauseSystem.m_pauseState == PauseState.PAUSE) return;
        if (this.joystickType == JoystickType.Floating)
        {
            MoveJoystickToCurrentTouchPosition();
        }
		else
		{
			if (Vector2.Distance(e.position, (Vector2)transform.GetChild(0).position) > 120) return;
		}
        touchState = JoystickState.TOUCH_DOWN;
        OnDrag(e);
    }


    public void OnDrag(PointerEventData e)
    {
        if (!IsEnableTouch())
            return;

        if (this.joystickType == JoystickType.Fixed &&
			touchState != JoystickState.TOUCH_DRAG &&
			Vector2.Distance(e.position, transform.GetChild(0).position) > 120) return;

		if (PauseSystem.m_pauseState == PauseState.PAUSE) return;
        Vector2 pos;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            this.imageBg.rectTransform,
            e.position,
            e.pressEventCamera,
            out pos))
        {

            pos.x = (pos.x / this.imageBg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / this.imageBg.rectTransform.sizeDelta.y);
            InputVector = pos;
            Vector3 calc = InputVector - Vector3.zero;
            FaceAngel = Mathf.Atan2(calc.y, calc.x) * Mathf.Rad2Deg;

            this._inputNomralizeVector = new Vector3(pos.x, 0, pos.y);
            this._inputNomralizeVector = (this._inputNomralizeVector.magnitude > 1.0f) ? 
                this._inputNomralizeVector.normalized : this._inputNomralizeVector;

            this.InputVector = (this.InputVector.magnitude > 1.0f) ? 
                this.InputVector.normalized : this.InputVector;

            Vector3 joystickPosition = new Vector3(
                this._inputNomralizeVector.x * (this.imageBg.rectTransform.sizeDelta.x * Sensitivity),
                this._inputNomralizeVector.z * (this.imageBg.rectTransform.sizeDelta.y * Sensitivity));
            this.imageJoystick.rectTransform.anchoredPosition = joystickPosition;
            this.InputDir = _inputNomralizeVector;
        }
        touchState = JoystickState.TOUCH_DRAG;
    }

    public void OnPointerUp(PointerEventData e)
    {
		if(StateListener.Instance.GetFlag(EStateFlag.OnTutorialDashGuide))
		{
			if (_inputNomralizeVector.x >= 0.99 && Mathf.Abs(_inputNomralizeVector.y) <= 0.005)
			{
				Debug.Log("Pass");
				StateListener.Instance.SetFlag(EStateFlag.OnPlayerDash, StateListener.EListenerState.SetOn);
			}
			else
			{
				Debug.Log("Return");
				this.InputDir = _inputNomralizeVector;
				this._inputNomralizeVector = Vector3.zero;
				this.imageJoystick.rectTransform.anchoredPosition = Vector3.zero;
				touchState = JoystickState.TOUCH_UP;
				return;
			}
		}
        if (!IsEnableTouch())
            return;
		if (touchState != JoystickState.TOUCH_DRAG && joystickType == JoystickType.Fixed)
			return;

        if (PauseSystem.m_pauseState == PauseState.PAUSE) return;
        this.InputDir = _inputNomralizeVector;
		this._inputNomralizeVector = Vector3.zero;
        this.imageJoystick.rectTransform.anchoredPosition = Vector3.zero;
        touchState = JoystickState.TOUCH_UP;
    }
}