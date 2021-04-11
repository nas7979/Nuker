using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bullet_Attack4 : Boss_AttackBase
{
	public override IEnumerator Attack(GameObject _Player, System.Action _EndFunc)
	{
		for(int i = 0; i < 5; i++)
		{
			BulletBase Temp = FireBullet(m_Bullets[0], transform.position + new Vector3(-2, 0), 180);
			yield return new WaitForSeconds(0.5f);
			Temp.Speed = 15;
			Temp.Direction = Vector2.SignedAngle(Vector2.right, _Player.transform.position - Temp.transform.position);
		}

		yield return new WaitForSeconds(1f);
		_EndFunc();
	}
}
