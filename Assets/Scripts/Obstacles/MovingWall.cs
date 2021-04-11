using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : Obstacle, IController
{
	[SerializeField] private float m_Speed = 5f;
	private Vector2 m_StartPosition = Vector2.zero;

	public void Awake()
	{
		m_StartPosition = transform.localPosition;
	}

	public void OnEnable()
	{
		transform.localPosition = m_StartPosition;
		GameSystem.Instance.SubscribeInitalizeListener(Initialize);
		GameSystem.Instance.SubscribeUpdateListener(FrameMove);
	}

	public void OnDisable()
	{
		GameSystem.Instance.UnSubscribeInitalizeListener(Initialize);
		GameSystem.Instance.UnSubscribeUpdateListener(FrameMove);
	}

	public void Initialize()
	{

	}

	public void FrameMove()
	{
		transform.Translate(m_Speed * Time.deltaTime, 0, 0);
	}
}
