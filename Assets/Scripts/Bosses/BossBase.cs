using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BossBase : MonoBehaviour, IController
{
	[SerializeField] protected float m_MaxHp;
	protected float m_Hp;
	public float Hp
	{
		get { return m_Hp; }
		set { m_Hp = value; }
	}
	[SerializeField] private Vector3 m_Offset;
	protected CameraMovement m_Camera;
	[SerializeField] private GameObject m_DistanceIndicator;
	[SerializeField] private Image m_HpBar;
	[SerializeField] private GameObject m_GameSystem;
	private float m_Distance;
	private bool m_IsOnBattle;

	protected List<Boss_AttackBase> m_AttackScripts = new List<Boss_AttackBase>();

	protected bool m_StickOnPosition;
	protected bool m_IsOnPosition;
	protected GameObject m_Player;

	public System.Action m_OnBossDead;

	private void OnEnable()
	{
		GameSystem.Instance.SubscribeInitalizeListener(Initialize);
		GameSystem.Instance.SubscribeUpdateListener(FrameMove);

		m_Player = GameObject.FindGameObjectWithTag("Player");
		m_Camera = GameObject.Find("Main Camera").GetComponent<CameraMovement>();

		Boss_AttackBase Temp;
		for (int i = 0; i < transform.childCount; i++)
		{
			Temp = transform.GetChild(i).GetComponent<Boss_AttackBase>();
			if (Temp != null)
			{
				m_AttackScripts.Add(transform.GetChild(i).GetComponent<Boss_AttackBase>());
				Temp.SetCamera(m_Camera);
			}
			else
			{
				break;
			}
		}
	}

	private void OnDisable()
	{
		GameSystem.Instance.UnSubscribeInitalizeListener(Initialize);
		GameSystem.Instance.UnSubscribeUpdateListener(FrameMove);
	}

	public virtual void Initialize()
	{
	}

	public virtual void FrameMove()
	{
		if (m_IsOnBattle == false)
		{
			m_Distance -= m_Camera.GetVelocityWithoutShake().x;
			if(m_Distance <= 0)
			{
				m_IsOnBattle = true;
				m_StickOnPosition = true;
				m_DistanceIndicator.gameObject.SetActive(false);
				m_HpBar.transform.parent.gameObject.SetActive(true);
				m_HpBar.gameObject.SetActive(true);
				StartCoroutine(IFirstAttack());
				GameObject.Find("GameSystem").GetComponent<ObstacleGenerator>().IsInBossBattle = true;
			}
			else
			{
				m_DistanceIndicator.transform.GetChild(0).GetComponent<Text>().text = (int)m_Distance + "M";
			}
		}
		else
		{
			m_HpBar.fillAmount = m_Hp / m_MaxHp;
			if(m_Hp <= 0)
			{
				m_GameSystem.GetComponent<ObstacleGenerator>().IsInBossBattle = false;
				m_GameSystem.GetComponent<BossGenerator>().CreateBoss();
				m_OnBossDead();
				GameObject[] Temp = GameObject.FindGameObjectsWithTag("EnemyBullet");
				for(int i = 0;i < Temp.Length; i++)
				{
					Temp[i].SetActive(false);
					ObjectManager.Instance.AddObject("Bullet_Hit", Temp[i].transform.position);
				}
				SoundManager.Instance.PlaySound("SFX_" + gameObject.name + "_Death");
			}
		}
	}

	private void FixedUpdate()
	{
		if (m_IsOnBattle)
		{
			if (m_StickOnPosition)
			{
				if (m_IsOnPosition || Vector2.Distance(transform.position, m_Camera.GetPositionWithoutShake() + m_Offset) < 0.01f)
				{
					m_IsOnPosition = true;
				}
				else
				{
					transform.position = Vector2.Lerp(transform.position, m_Camera.GetPositionWithoutShake() + m_Offset, 0.1f);
				}
			}
		}
		transform.Translate(m_Camera.GetVelocity(), Space.World);
	}

	public abstract void Attack();

	public IEnumerator IFirstAttack()
	{
		while(m_IsOnPosition == false)
		{
			yield return null;
		}
		yield return new WaitForSeconds(2);
		Attack();
	}

	public void SetDistance(float _Distance)
	{
		m_Distance = _Distance;
		m_StickOnPosition = false;
		transform.position = new Vector3(m_Player.transform.position.x + m_Distance + 3, 2, 0);
		m_IsOnBattle = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("PlayerBullet"))
		{
			collision.gameObject.tag = "EnemyBullet";
			collision.gameObject.SetActive(false);
			m_Hp -= collision.GetComponent<BulletBase>().BossDamage;
			ObjectManager.Instance.AddObject("Bullet_Hit", collision.transform.position);
			SoundManager.Instance.PlaySound("SFX_Boss_Bullet_BulletHit");
		}
	}

	public void BossInit()
	{
		gameObject.SetActive(true);
		StopAllCoroutines();
		m_HpBar.transform.parent.gameObject.SetActive(false);
		m_HpBar.gameObject.SetActive(false);
		m_DistanceIndicator.gameObject.SetActive(true);
		m_Hp = m_MaxHp;
		m_IsOnPosition = false;
		m_OnBossDead = () => {
			ScoreSystem.OnAddScore(1000);
		};
	}
}
