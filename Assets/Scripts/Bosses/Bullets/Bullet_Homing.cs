using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Homing : BulletBase
{
	[SerializeField] private float m_HomingTime = 0.5f;
	private float m_HomingTimer = 0.5f;
	private GameObject m_Target = null;
	public GameObject Target { get => m_Target; set => m_Target = value; }

	public override void OnEnable()
	{
		base.OnEnable();
		m_HomingTimer = m_HomingTime;
		m_Target = null;
	}

	public override void Initialize()
	{

	}

	public override void FrameMove()
	{
		if (m_Target != null && m_HomingTimer > 0)
		{
			m_Direction = Vector2.SignedAngle(Vector2.right, m_Target.transform.position - transform.position);
			m_HomingTimer -= Time.deltaTime;
		}
		transform.localRotation = Quaternion.Euler(0, 0, m_Direction);
		transform.Translate(new Vector3(m_Speed, 0) * Time.deltaTime, Space.Self);

		if (m_Friction != 1)
		{
			m_Speed *= Mathf.Pow(m_Friction, Time.deltaTime * 60f);

			if (m_Friction > 1)
			{
				if (m_EndSpeed <= m_Speed)
				{
					m_Friction = 1;
					m_Speed = m_EndSpeed;
				}
			}
			else
			{
				{
					if (m_EndSpeed >= m_Speed)
					{
						m_Friction = 1;
						m_Speed = m_EndSpeed;
					}
				}
			}
		}
	}
}
