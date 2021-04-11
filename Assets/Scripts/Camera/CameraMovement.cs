using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoSingleton<CameraMovement>, IController
{
	[SerializeField] private GameObject m_Player = null;
	public GameObject Player { get => m_Player; set => m_Player = value; }
	[SerializeField] private float m_XOffset = 0;
	[SerializeField] private float m_YOffset = 0;
	private Vector2 m_ShakeOffset = Vector2.zero;
	private Vector3 m_Velocity;
	public Vector3 Velocity { set => m_Velocity = value; }
	private float m_CameraWidth = 0;

	private void OnEnable()
	{
		GameSystem.Instance.SubscribeInitalizeListener(Initialize);
		GameSystem.Instance.SubscribeUpdateListener(FrameMove);
	}

	private void OnDisable()
	{
		GameSystem.Instance.UnSubscribeInitalizeListener(Initialize);
		GameSystem.Instance.UnSubscribeUpdateListener(FrameMove);
	}

	public void Initialize()
	{
		
	}

	public void FrameMove()
	{
		Camera Cam = GetComponent<Camera>();
		if(Cam.orthographicSize != 6)
		{
			Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, 6, 0.1f * 60 * Time.deltaTime);
		}
		if(PauseSystem.m_pauseState == PauseState.PAUSE)
		{
			m_Velocity = Vector3.zero;
		}
	}

	public void FixedUpdate()
	{
		Vector3 PrevPos = transform.position;
		transform.position = Vector3.Lerp(transform.position, new Vector3(m_Player.transform.position.x + m_XOffset, m_YOffset, -10), 0.15f * 60 * Time.deltaTime);
		m_Velocity = transform.position - PrevPos;
	}

	private void OnPreRender()
	{
		transform.position += (Vector3)m_ShakeOffset;
	}

	private void OnPostRender()
	{
		transform.position -= (Vector3)m_ShakeOffset;
	}

	private IEnumerator IShake(float _Force, float _Time)
	{
		float TimeLeft = _Time;
		float Force = _Force;
		float ForcePerSec = _Force / _Time;
		while(TimeLeft > 0)
		{
			TimeLeft -= Time.unscaledDeltaTime;
			Force -= ForcePerSec * Time.unscaledDeltaTime;
			m_ShakeOffset = new Vector2(Random.Range(-Force, Force), Random.Range(-Force, Force));
			yield return null;
		}
		m_ShakeOffset = Vector2.zero;
	}

	public void Shake(float _Force, float _Time)
	{
		StartCoroutine(IShake(_Force, _Time));
	}

	public Vector3 GetPositionWithoutShake()
	{
		return transform.position - (Vector3)m_ShakeOffset;
	}

	public Vector3 GetVelocity()
	{
		return m_Velocity;
	}

	public Vector3 GetVelocityWithoutShake()
	{
		return m_Velocity - (Vector3)m_ShakeOffset;
	}

	public float GetCameraWidth()
	{
		if(m_CameraWidth == 0)
		{
			m_CameraWidth = GetComponent<Camera>().orthographicSize * GetComponent<Camera>().aspect * 2;
		}
		return m_CameraWidth;
	}
}
