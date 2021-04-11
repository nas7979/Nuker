using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class BulletData
{
	public GameObject Base = null;
	public Sprite Sprite = null;
	public float Speed = 10f;
	public float Damage = 10f;
	public float BossDamage = 10f;
	public float Friction = 1f;
	public float EndSpeed = 1f;
};

public abstract class Boss_AttackBase : MonoBehaviour
{
	[SerializeField] protected BulletData[] m_Bullets = new BulletData[1];
	private CameraMovement m_Camera;

	public abstract IEnumerator Attack(GameObject _Player, Action _EndFunc);

	public BulletBase FireBullet(BulletData _Data, Vector2 _Pos, float _Dir)
	{
		BulletBase Temp = ObjectManager.Instance.AddObject(_Data.Base.name, _Pos).GetComponent<BulletBase>();
		Temp.SetProperties(m_Camera, _Data, _Dir);
		Temp.tag = "EnemyBullet";
		return Temp;
	}

	public void SetCamera(CameraMovement _Camera)
	{
		m_Camera = _Camera;
	}
}
