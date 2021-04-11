using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bullet_Attack3 : Boss_AttackBase
{
	List<BulletBase> m_SpawnedBullets = new List<BulletBase>();
	public override IEnumerator Attack(GameObject _Player, System.Action _EndFunc)
	{
		for (int i = 0; i < 5; i++)
		{
			m_SpawnedBullets.Add(FireBullet(m_Bullets[0], new Vector3(transform.position.x + 2, -1 + i * 2), 0).GetComponent<BulletBase>());
			yield return new WaitForSeconds(0.075f);
		}

		yield return new WaitForSeconds(0.75f);

		for(int i = 0; i < m_SpawnedBullets.Count; i++)
		{
			m_SpawnedBullets[i].Speed = 35;
			m_SpawnedBullets[i].Direction = Vector2.SignedAngle(Vector2.right, _Player.transform.position - m_SpawnedBullets[i].transform.position);
		}
		m_SpawnedBullets.Clear();
		yield return new WaitForSeconds(1.5f);
		_EndFunc();
	}

	public void OnEnable()
	{
		m_SpawnedBullets.Clear();
	}
}
