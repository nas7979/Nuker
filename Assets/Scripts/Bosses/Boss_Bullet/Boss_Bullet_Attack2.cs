using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bullet_Attack2 : Boss_AttackBase
{
	float[] m_BulletY = new float[3] { -1, 3, 7 };

	public override IEnumerator Attack(GameObject _Player, System.Action _EndFunc)
	{
		float StartPos = m_BulletY[Random.Range(0, 3)];
		for (int i = 0; i < 5; i++)
		{
			FireBullet(m_Bullets[0], new Vector3(transform.position.x, StartPos), 180);
			yield return new WaitForSeconds(0.15f);
		}

		yield return new WaitForSeconds(1.5f);

		_EndFunc();
	}
}
