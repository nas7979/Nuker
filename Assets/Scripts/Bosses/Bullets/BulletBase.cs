using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour, IController
{
	protected float m_Speed;
	protected float m_Damage;
	protected float m_BossDamage;
	protected float m_Direction;
	protected float m_Friction;
	protected float m_EndSpeed;
	private CameraMovement m_Camera;

	public float Speed
	{
		get { return m_Speed; }
		set { m_Speed = value; }
	}
	public float Damage
	{
		get { return m_Damage; }
		set { m_Damage = value; }
	}
	public float BossDamage
	{
		get { return m_BossDamage; }
		set { m_BossDamage = value; }
	}
	public float Direction
	{
		get { return m_Direction; }
		set { m_Direction = value; }
	}
	public float Friction
	{
		get { return m_Friction; }
		set { m_Friction = value; }
	}
	public float EndSpeed
	{
		get { return m_EndSpeed; }
		set { m_EndSpeed = value; }
	}

	public virtual void OnEnable()
	{
		GameSystem.Instance.SubscribeInitalizeListener(Initialize);
		GameSystem.Instance.SubscribeUpdateListener(FrameMove);
	}

	public virtual void OnDisable()
	{
		GameSystem.Instance.UnSubscribeInitalizeListener(Initialize);
		GameSystem.Instance.UnSubscribeUpdateListener(FrameMove);
	}

	private void OnBecameInvisible()
	{
		gameObject.SetActive(false);
	}

	private void FixedUpdate()
	{
		transform.Translate(m_Camera.GetVelocity(), Space.World);
	}

	public abstract void Initialize();

	public abstract void FrameMove();

	public void SetProperties(CameraMovement _Camera, float _Speed, float _Direction, float _Damage, float _BossDamage, float _Friction = 1, float _EndSpeed = 1)
	{
		m_Camera = _Camera;
		m_Speed = _Speed;
		m_Direction = _Direction;
		m_Damage = _Damage;
		m_BossDamage = _BossDamage;
		m_Friction = _Friction;
		m_EndSpeed = _EndSpeed;
	}

	public void SetProperties(CameraMovement _Camera, BulletData _Data, float _Direction)
	{
		m_Camera = _Camera;
		m_Speed = _Data.Speed;
		m_Direction = _Direction;
		m_Damage = _Data.Damage;
		m_BossDamage = _Data.BossDamage;
		m_Friction = _Data.Friction;
		m_EndSpeed = _Data.EndSpeed;
		GetComponent<SpriteRenderer>().sprite = _Data.Sprite;
	}
}