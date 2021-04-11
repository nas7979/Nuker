using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopOutWall : Obstacle
{
	[SerializeField] private Vector2 m_TargetPosition = Vector2.zero;
	[SerializeField] private float m_Speed = 10;
	[SerializeField] private float m_SpeedMultiplyer = 1f;
	[SerializeField] private float m_EndSpeed = 5f;
	[SerializeField] private bool m_IsObstacle = false;
	private Vector3 m_StartPosition = Vector3.zero;
	public Vector2 TargetPosition { get => m_TargetPosition; set => m_TargetPosition = value; }
	public float Speed { get => Speed; set => Speed = value; }
	public float SpeedMultiplyer { get => m_SpeedMultiplyer; set => m_SpeedMultiplyer = value; }
	public float EndSpeed { get => m_EndSpeed; set => m_EndSpeed = value; }
	public Coroutine IMoveCoroutine = null;
	private void Awake()
	{
		m_StartPosition = transform.localPosition;
		m_Speed *= transform.lossyScale.x;
	}

	public void Move()
	{
		if(gameObject.activeInHierarchy)
			IMoveCoroutine = StartCoroutine(IMove());
	}

	private IEnumerator IMove()
	{
		Vector2 Pos = transform.position;
		Vector2 TargetPos = m_TargetPosition * transform.lossyScale.x;
		while (Vector2.Distance(transform.position, Pos + TargetPos) > 0.1f)
		{
			m_Speed *= Mathf.Pow(m_SpeedMultiplyer, 60f * Time.deltaTime);
			if (m_SpeedMultiplyer > 1f)
			{
				if (m_Speed >= m_EndSpeed)
				{
					m_Speed = m_EndSpeed;
					m_SpeedMultiplyer = 1f;
				}
			}
			else
			{
				if (m_Speed <= m_EndSpeed)
				{
					m_Speed = m_EndSpeed;
					m_SpeedMultiplyer = 1f;
				}
			}
			transform.position = Vector2.MoveTowards(transform.position, Pos + TargetPos, m_Speed * Time.deltaTime);
			yield return null;
		}
		if (m_IsObstacle)
			SoundManager.Instance.PlaySound("SFX_Obstacle_Land");
	}

	private void OnEnable()
	{
		transform.localPosition = m_StartPosition;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Vector3 Pos = transform.position + (Vector3)m_TargetPosition * transform.lossyScale.x;
		Gizmos.DrawLine(Pos + new Vector3(-1, 0), Pos + new Vector3(1, 0));
		Gizmos.DrawLine(Pos + new Vector3(0, -1), Pos + new Vector3(0, 1));
	}
}
