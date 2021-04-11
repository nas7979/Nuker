using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BulletBase
{
	public override void Initialize()
	{

	}

	public override void FrameMove()
	{
		transform.rotation = Quaternion.Euler(0, 0, m_Direction);
		transform.Translate(new Vector3(m_Speed, 0) * Time.deltaTime, Space.Self);

		if(m_Friction != 1)
		{
			m_Speed *= Mathf.Pow(m_Friction, Time.deltaTime * 60f);

			if(m_Friction > 1)
			{
				if(m_EndSpeed <= m_Speed)
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