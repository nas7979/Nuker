using Game.INPUT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Game.Utils;
using UnityEngine.Tilemaps;

[Serializable]
public struct JoystickSprite
{
	public const string normal = "NORMAL";
	public Sprite Normal;
	public const string red = "RED";
	public Sprite RED;
	public const string blue = "BLUE";
	public Sprite BLUE;
}
[Serializable]
public class PlayerInput : MonoBehaviour, IController
{
    #region InputFunctions Initalization : PlayerInput

    [SerializeField] public VirtualJoystick virtualJoystick = null;
    [SerializeField] private Rigidbody2D rigidbody2D = null;
	[SerializeField] private ParticleSystem Afterimage = null;

	[SerializeField] float hp = 100f;

	public float Hp
	{
		get { return hp; }
		set { hp = value; }
	}

	[Tooltip("Dash 지속 범위(클수록 대쉬 시간이 길어짐)")]
	[Range(50.0f, 250.0f)]
	[SerializeField] private float DashDuration = 150f;
	[Range(1.0f, 30.0f)]
    [SerializeField] private float DashForce = 10.0f;
	[SerializeField] private float GravityScale = 7;
	[SerializeField] private CameraMovement camera;
	private bool isInvincible = false;
	private bool isArrowActive = false;
	private bool isDashing = false;
	private bool isReflectedBullet = false;

	[HideInInspector] public bool isAbleJump = true;
	[SerializeField] private Vector3 normalizedInputVector = Vector3.zero;
	[SerializeField] private Vector3 DashVelocity = Vector3.zero;

	[SerializeField] GameObject ArrowUI = null;
	[SerializeField] JoystickSprite JoystickSprite;
	[SerializeField] Image ChangeJoystickTarget = null;

	[SerializeField] ActionPoint actionPoint = null;

	private Camera m_MainCamera = null;

	public Action m_OnPlayerDead;

	private PlayerOutofScreen m_outofScreen;
	private SpriteRenderer[] m_PartRenderers = null;
	private Animator m_Animator = null;
	[SerializeField] private GameObject m_DestroyEffect = null;
	[SerializeField] private GameObject m_PlayerHitOverlay = null;

	public void Awake()
	{
		ObjectManager.Instance.Player = gameObject;
		Debug.Assert(actionPoint, "NullReferenceException");
		Debug.Assert(camera, "NullReferenceException");
		Debug.Assert(ArrowUI, "NullReferenceException");
		Debug.Assert(ChangeJoystickTarget, "NullReferenceException");
		Debug.Assert(virtualJoystick, "NullReferenceException");
		Debug.Assert(rigidbody2D, "NullReferenceException");
		Debug.Assert(Afterimage, "NullReferenceException");

		m_MainCamera = Camera.main;
		m_PartRenderers = GetComponentsInChildren<SpriteRenderer>();
		m_outofScreen = new PlayerOutofScreen(m_MainCamera, GetTransform);
		m_Animator = GetComponent<Animator>();
	}

	private void OnEnable()
	{
		GameSystem.Instance.SubscribeInitalizeListener(Initialize);
		GameSystem.Instance.SubscribeUpdateListener(FrameMove);
		rigidbody2D.gravityScale = GravityScale;
		Afterimage.Stop();
    }
    private void OnDisable()
	{
		GameSystem.Instance.UnSubscribeInitalizeListener(Initialize);
		GameSystem.Instance.UnSubscribeUpdateListener(FrameMove);
	}
    public Transform GetTransform
    {
        get
        {
			return this.transform;
        }
    }

	public Vector3 InputDir
    {
        get
        {
			return virtualJoystick.InputVector;
        }
    }

	public Vector3 FaceAngle { get; private set; }
	public bool IsCancel()
    {
		if (virtualJoystick.InputVector.x < 0f)
			return true;
		return false;
	}

	public void Initialize()
	{

	}


    public void FrameMove()
    {
		//if (PauseSystem.m_pauseState == PauseState.PAUSE)
		//	return;
		if (m_outofScreen.IsScreenOut())
			m_OnPlayerDead();
		

        switch (virtualJoystick?.touchState)
        {
            case JoystickState.TOUCH_NONE:
                break;
            case JoystickState.TOUCH_DOWN:
                Time.timeScale = 0.1f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;

				
                break;
            case JoystickState.TOUCH_DRAG:
				StateListener.Instance?.SetFlag(
					EStateFlag.OnTouch | EStateFlag.OnTouchDrag
					, StateListener.EListenerState.SetOn);

				if (Time.timeScale != 0)
				{
					Time.timeScale = 0.1f;
					Time.fixedDeltaTime = 0.02f * Time.timeScale;
				}

				if(actionPoint.IsRemainActionPoint() == false || IsCancel())
				{
					ChangeJoystickTarget.sprite = JoystickSprite.RED;
					isArrowActive = false;
				}
                else
                {
					ChangeJoystickTarget.sprite = JoystickSprite.BLUE;
					isArrowActive = true;
					StateListener.Instance?.SetFlag(EStateFlag.OnPlayerDashing, StateListener.EListenerState.SetOn);
				}
					

				ArrowUI?.SetActive(isArrowActive);
				ArrowUI.transform.position = m_MainCamera.WorldToScreenPoint(transform.position);
				Vector3 angle = ArrowUI.transform.eulerAngles;
				FaceAngle = new Vector3(angle.x, angle.y, virtualJoystick.FaceAngel);
				ArrowUI.transform.eulerAngles = FaceAngle;

				normalizedInputVector = ChangeVectorYToZ(virtualJoystick.InputDir).normalized;
                break;
            case JoystickState.TOUCH_UP:
				transform.eulerAngles = Vector3.zero;
				ArrowUI?.SetActive(false);
				ChangeJoystickTarget.sprite = ChangeJoystickTarget.sprite = JoystickSprite.Normal;
				virtualJoystick.touchState = JoystickState.TOUCH_NONE;

				if (StateListener.Instance.GetFlag(EStateFlag.OnTutorialDashGuide))
					break;
				StateListener.Instance?.SetFlag(
					EStateFlag.OnTouch | EStateFlag.OnTouchUp
					, StateListener.EListenerState.SetOn);

				Time.timeScale = 1.0f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;

				if (!IsCancel() && actionPoint.IsRemainActionPoint())
				{
					normalizedInputVector = ChangeVectorYToZ(virtualJoystick.InputDir).normalized;
					Dash(virtualJoystick.InputVector, DashForce);

					actionPoint?.AddPoint(-1);
				}
				
                break;
            default:
                break;
        }
		if (IsDashing())
		{
			transform.eulerAngles = FaceAngle;
			ParticleSystem.MainModule Temp = Afterimage.main;
			Temp.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
			rigidbody2D.velocity = DashVelocity;
			DashVelocity -= DashVelocity.normalized * DashDuration * Time.deltaTime;
			if (DashVelocity.sqrMagnitude <= 3f)
			{
				isDashing = false;
				DashVelocity = Vector3.zero;
				rigidbody2D.gravityScale = GravityScale;
				transform.eulerAngles = Vector3.zero;
				Afterimage.Stop();
				m_Animator.SetBool("IsDashing", false);
			}
		}
    }
	// 0 ~ 100f
	public float GetInputDistanceToPercent()
    {
		return virtualJoystick.InputVector.magnitude * 100f;
	}
    private Vector3 ChangeVectorYToZ(Vector3 targetVec)
    {
        return new Vector3(targetVec.x, targetVec.z);
    }
    private void Dash(Vector3 InputDir, float DashForce)
    {
        Vector3 force = InputDir * DashForce * Vector2.one;
		DashVelocity = force * 2;
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.gravityScale = 0;
		Afterimage.Play();
		isDashing = true;
		isReflectedBullet = false;
		camera.GetComponent<Camera>().orthographicSize = 5.2f;
		m_Animator.SetBool("IsDashing", true);
		m_Animator.SetBool("IsJumping", true);
		SoundManager.Instance.PlaySound("SFX_Dash");
		StateListener.Instance?.SetFlag(EStateFlag.OnPlayerDash, StateListener.EListenerState.SetOn);
	}
	public bool IsDashing()
	{
		return isDashing;
	}
	public bool IsInvincible()
	{
		return isInvincible;
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		switch(collision.tag)
		{
			case "EnemyBullet":
				{
					BulletBase Temp = collision.GetComponent<BulletBase>();
					if (IsDashing())
					{
						collision.tag = "PlayerBullet";
						if (Temp.GetComponent<Bullet_Homing>() != null)
                        {
							Temp.GetComponent<Bullet_Homing>().Target = null;
                        }
						Temp.Direction -= 180;
						if (isReflectedBullet == false)
						{
							ObjectManager.Instance.AddObject("Bullet_Reflect", collision.transform.position);
							SoundManager.Instance.PlaySound("SFX_Bullet_Reflect");
							isReflectedBullet = true;
						}

						StateListener.Instance?.SetFlag(EStateFlag.OnPlayerDashColl_Obstacle, StateListener.EListenerState.SetOn);
					}
					else if (IsInvincible() == false)
					{
						AddHp(-10);
						Vibration.Vibrate(100);
						collision.gameObject.SetActive(false);
						ObjectManager.Instance.AddObject("Bullet_Hit", collision.transform.position);
						Vibration.Vibrate(100);
						SoundManager.Instance.PlaySound("SFX_Boss_Bullet_BulletHit");
						StateListener.Instance?.SetFlag(EStateFlag.OnPlayerColl_Obstacle, StateListener.EListenerState.SetOn);
					}
					break;
				}

			case "Wall":
				{
					if (IsInvincible()) return;
					if (IsDashing() == false)
					{
						AddHp(-10);
						Vibration.Vibrate(100);
					}
					break;
				}

			case "DestructibleWall":
				{
					if (IsDashing() == false)
					{
						if (IsInvincible()) return;
						AddHp(-10);
						Vibration.Vibrate(100);
					}
					else
					{
						actionPoint.AddPoint(1);
						Vibration.Vibrate(100);
						collision.GetComponent<Obstacle>().CreateDestroyEffect();
						collision.gameObject.SetActive(false);
						TimeManager.Instance.SetTimeScale(0.2f, 0f);
						camera.Shake(0.25f, 0.5f);

						//GameObject[] Temp = MeshCut.Cut(collision.gameObject, collision.transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f)), new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f)), null);
						
					}
					break;
				}
		}
	}
	private void OnCollisionEnter2D(Collision2D collision)
	{
		switch (collision.gameObject.tag)
		{
			case "Ground":
				{
					if (rigidbody2D.velocity.y <= 1f && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Run"))
					{
						DashVelocity = Vector2.zero;
						rigidbody2D.gravityScale = GravityScale;
						isAbleJump = true;
						m_Animator.SetBool("IsJumping", false);
						m_Animator.SetBool("IsDashing", false);
						SoundManager.Instance.PlaySound("SFX_Land");
					}

					break;
				}
		}
	}
	public void AddHp(float _Hp)
	{
		hp += _Hp;
		if(_Hp < 0)
		{
			SoundManager.Instance.PlaySound("SFX_Player_Hit");
			m_PlayerHitOverlay.SetActive(true);
			camera.Shake(0.25f, 1);
			isInvincible = true;
			m_Animator.Play("Player_Hit");
			StartCoroutine(IInvincibleTimer(1.5f));
			for(int i = 0; i < m_PartRenderers.Length; i++)
			{
				m_PartRenderers[i].color = new Color(1, 1, 1, 0.7f);
			}
		}

		if (hp <= 0)
			m_OnPlayerDead();
	}
	private IEnumerator IInvincibleTimer(float _Time)
	{
		yield return new WaitForSeconds(_Time);
		isInvincible = false;
		for (int i = 0; i < m_PartRenderers.Length; i++)
		{
			m_PartRenderers[i].color = Color.white;
		}
	}
	public void DashAttack()
	{
		if (IsDashing() && (!m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Player_DashAttack") || m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f))
		{
			m_Animator.Play("Player_DashAttack");
		}
	}
	public void PlayAttackSound()
	{
		SoundManager.Instance.PlaySound("SFX_Slash" + UnityEngine.Random.Range(1, 4).ToString());
	}
	#endregion
}
