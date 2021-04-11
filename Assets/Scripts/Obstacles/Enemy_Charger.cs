using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Charger : Obstacle, IController
{
	private bool m_IsCharged;
	private bool m_IsCharging;
	[SerializeField] private float m_ChargeSpeed = 20;
	[SerializeField] private float m_DetectRange;
	[SerializeField] private float m_Damage;
	private GameObject m_Player;
	private Vector3 m_StartPosition = Vector3.zero;

	private void Start()
	{
		m_Player = ObjectManager.Instance.Player;
	}

	private void Awake()
	{
		m_StartPosition = transform.localPosition;
	}

	private void OnEnable()
	{
		GameSystem.Instance.SubscribeInitalizeListener(Initialize);
		GameSystem.Instance.SubscribeUpdateListener(FrameMove);
		m_IsCharged = false;
		m_IsCharging = false;
		transform.localEulerAngles = Vector3.zero;
		transform.localPosition = m_StartPosition;
	}

	private void OnDisable()
	{
		GameSystem.Instance.UnSubscribeInitalizeListener(Initialize);
		GameSystem.Instance.UnSubscribeUpdateListener(FrameMove);
	}

	public void Initialize()
	{

	}

	public void FrameMove()
	{
		if(m_IsCharged == false && Vector2.Distance(transform.position, m_Player.transform.position) <= m_DetectRange)
		{
			m_IsCharged = true;
			StartCoroutine("ICharge");
		}

		if(transform.rotation.eulerAngles.z >= 90 && transform.rotation.eulerAngles.z < 270)
		{
			transform.localScale = new Vector3(1, -1, 1);
		}
		else
		{
			transform.localScale = new Vector3(1, 1, 1);
		}
	}

	private IEnumerator ICharge()
	{
		float Timer = 0.6f;
		while(Timer > 0)
		{
			Timer -= Time.deltaTime;
			transform.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, (m_Player.transform.position - transform.position)));
			yield return null;
		}

		m_IsCharging = true;
		while(true)
		{
			transform.Translate(new Vector3(m_ChargeSpeed, 0, 0) * Time.deltaTime, Space.Self);
			yield return null;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("Player"))
		{
			gameObject.SetActive(false);
			PlayerInput Player = collision.transform.parent.GetComponent<PlayerInput>();
			if (Player.IsDashing())
			{
				ScoreSystem.OnAddScore(500);
				CameraMovement.Instance.Shake(0.25f, 0.5f);
				TimeManager.Instance.SetTimeScale(0.1f, 0f);
			}
			else if(Player.IsInvincible() == false)
			{
				Player.AddHp(-m_Damage);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, m_DetectRange);
	}
}
