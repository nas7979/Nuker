using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Bounce : BulletBase
{
	public override void Initialize()
	{

	}

	public override void FrameMove()
	{
		transform.rotation = Quaternion.Euler(0, 0, m_Direction);
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

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			Vector2 CurDir = new Vector2(Mathf.Cos(m_Direction * Mathf.Deg2Rad), Mathf.Sin(m_Direction * Mathf.Deg2Rad));
			CurDir.y *= -1;
			m_Direction = Vector2.SignedAngle(Vector2.right, CurDir);
		}
	}
}