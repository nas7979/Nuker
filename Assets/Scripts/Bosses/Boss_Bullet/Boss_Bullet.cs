using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bullet : BossBase
{
	public override void Initialize()
	{
		base.Initialize();
	}

	public override void FrameMove()
	{
		base.FrameMove();
	}

	public override void Attack()
	{
		if (m_Hp > 0)
		{
			StartCoroutine(m_AttackScripts[Random.Range(0, m_AttackScripts.Count)].Attack(m_Player, Attack));
		}
	}
}
