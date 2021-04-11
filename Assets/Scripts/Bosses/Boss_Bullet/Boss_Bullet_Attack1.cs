using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bullet_Attack1 : Boss_AttackBase
{
	float[] m_BulletY = new float[3]{ -1, 3, 7 };

	public override IEnumerator Attack(GameObject _Player, System.Action _EndFunc)
	{
		for(int i = 0; i < 6; i++)
		{
			FireBullet(m_Bullets[0], new Vector3(transform.position.x, m_BulletY[Random.Range(0, 3)]), 180);
			yield return new WaitForSeconds(0.3f);
		}

		yield return new WaitForSeconds(1.5f);
		_EndFunc();
	}
}
