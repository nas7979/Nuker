using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using GoogleMobileAds.Api;

[DefaultExecutionOrder(-2000)]
public class Menu_Buttons : MonoBehaviour
{
	[System.Serializable]
	public struct GameStart
	{
		[Header("Title")]
		public GameObject Title;
		public GameObject StartButton;
		public GameObject SettingButton;
		public GameObject AdRemove;
		public GameObject Ranking;
		public GameObject DummyPlayer;
		public CameraMovement Camera;

		[Header("Ingame")]
		public GameObject Player;
		public GameObject GameSystem;
		public GameObject Joystick;
		public GameObject HP;
		public GameObject Jump;
		public GameObject Score;
		public GameObject Pause;
		public GameObject ActPoint;
	}

	public GameStart m_GameStart = new GameStart();
	[SerializeField] private GameObject m_TutorialCursor = null;

	public static Action OnClickStartButton;
	private void Awake()
	{
		if(OnClickStartButton != null)
			OnClickStartButton = delegate { };
	}

	private void Start()
	{
		SoundManager.Instance.PlaySound("BGM", true, true);
	}

	public void OnClickStart(bool IsTitleUiMove)
	{
		OnClickStartButton.Invoke();
		StartCoroutine(IGameStart(IsTitleUiMove));
		SoundManager.Instance.PlaySound("SFX_Button5");
		StateListener.Instance?.SetFlag(EStateFlag.OnClick_StartBtn, StateListener.EListenerState.SetOn);
	}

	public void OnClickAdRemove()
	{
		m_GameStart.AdRemove.SetActive(false);
		SoundManager.Instance.PlaySound("SFX_Button8");
		SaveSystem.Instance.BuyAdRemove();
	}

	private IEnumerator IGameStart(bool IsTitleUiMove)
	{
		m_GameStart.StartButton.GetComponent<Button>().enabled = false;
		bool isTutorial = SaveSystem.Instance.Data.HighScore == 0;

		if (IsTitleUiMove)
		{
			m_GameStart.StartButton.GetComponent<PopOutWall>().Move();
			m_GameStart.SettingButton.GetComponent<PopOutWall>().Move();
			m_GameStart.Title.GetComponent<PopOutWall>().Move();
			m_GameStart.AdRemove.GetComponent<PopOutWall>().Move();
			m_GameStart.Ranking.GetComponent<PopOutWall>().Move();

			m_GameStart.HP.GetComponent<PopOutWall>().Move();
			m_GameStart.ActPoint.GetComponent<PopOutWall>().Move();

			if (!isTutorial)
			{
				m_GameStart.Joystick.GetComponent<PopOutWall>().Move();
				m_GameStart.Jump.GetComponent<PopOutWall>().Move();
				m_GameStart.Score.GetComponent<PopOutWall>().Move();
				m_GameStart.Pause.GetComponent<PopOutWall>().Move();
			}
			else
			{
				m_GameStart.GameSystem.GetComponent<ObstacleGenerator>().StartTutorial();
				StartCoroutine(ITutorial());
			}
		}
        else
        {
			m_GameStart.StartButton		.SetActive(false);
			m_GameStart.SettingButton	.SetActive(false);
			m_GameStart.Title			.SetActive(false);
			m_GameStart.AdRemove		.SetActive(false);
			m_GameStart.Ranking			.SetActive(false);

			m_GameStart.Joystick.GetComponent<PopOutWall>().Move();
			m_GameStart.HP.GetComponent<PopOutWall>().Move();
			m_GameStart.Jump.GetComponent<PopOutWall>().Move();
			m_GameStart.Score.GetComponent<PopOutWall>().Move();
			m_GameStart.Pause.GetComponent<PopOutWall>().Move();
			m_GameStart.ActPoint.GetComponent<PopOutWall>().Move();

		}
		m_GameStart.Player.SetActive(true);

		Rigidbody2D _playerRigidbody = m_GameStart.Player.GetComponent<Rigidbody2D>();
		PlayerInput _playerInput = m_GameStart.Player.GetComponent<PlayerInput>();

		_playerInput.virtualJoystick.EnableTouch = false;
		_playerRigidbody.bodyType = RigidbodyType2D.Kinematic;

		while (Mathf.Abs(m_GameStart.Player.transform.position.x - m_GameStart.DummyPlayer.transform.position.x) >= 0.1f)
		{
			m_GameStart.Player.transform.position = Vector2.Lerp(m_GameStart.Player.transform.position, m_GameStart.DummyPlayer.transform.position, 0.03f * Time.deltaTime * 60f);
			yield return null;
		}

		m_GameStart.Player.GetComponent<PlayerInput>().isAbleJump = true;
		_playerRigidbody.bodyType = RigidbodyType2D.Dynamic;
		_playerInput.virtualJoystick.EnableTouch = true;
		m_GameStart.Camera.Player = m_GameStart.Player;
		m_GameStart.DummyPlayer.SetActive(false);
		m_GameStart.GameSystem.GetComponent<ObstacleGenerator>().IsInBossBattle = false;

		if (!isTutorial)
		{
			m_GameStart.Score.GetComponent<ScoreSystem>().Initialize();
			m_GameStart.GameSystem.GetComponent<BossGenerator>().enabled = true;
		}

		yield break;
	}

	private IEnumerator ITutorial()
	{
		yield return new WaitUntil(() => { return StateListener.Instance.GetFlag(EStateFlag.OnTutorialJumpGuide); }); //장애물 앞에서 점프

		m_GameStart.Jump.transform.Translate(m_GameStart.Jump.GetComponent<PopOutWall>().TargetPosition * m_GameStart.Jump.transform.lossyScale.x);
		Time.timeScale = 0;
		m_TutorialCursor.SetActive(true);

		yield return new WaitUntil(() => { return StateListener.Instance.GetFlag(EStateFlag.OnClick_JumpBtn); }); //점프 버튼 누름

		m_GameStart.Jump.transform.Translate(-m_GameStart.Jump.GetComponent<PopOutWall>().TargetPosition * m_GameStart.Jump.transform.lossyScale.x);
		Time.timeScale = 1;
		m_TutorialCursor.SetActive(false);

		yield return new WaitUntil(() => { return StateListener.Instance.GetFlag(EStateFlag.OnTutorialDashGuide); }); //장애물 앞에서 대쉬

		m_GameStart.Joystick.transform.Translate(m_GameStart.Joystick.GetComponent<PopOutWall>().TargetPosition * m_GameStart.Jump.transform.lossyScale.x);
		Time.timeScale = 0;
		m_TutorialCursor.SetActive(true);

		VirtualJoystick Stick = m_GameStart.Joystick.transform.parent.GetComponent<VirtualJoystick>();
		m_TutorialCursor.GetComponent<Animator>().Play("Dash");
		while (!StateListener.Instance.GetFlag(EStateFlag.OnPlayerDash)) //스틱을 오른쪽으로 드래그한 채로 손가락을 땔 때까지
		{
			if(StateListener.Instance.GetFlag(EStateFlag.OnTouchUp))
				m_TutorialCursor.SetActive(true);

			if(StateListener.Instance.GetFlag(EStateFlag.OnTouch))
				m_TutorialCursor.SetActive(false);

			yield return null;
		}

		Time.timeScale = 1;
		StateListener.Instance.SetFlag(EStateFlag.All, StateListener.EListenerState.SetOff);
		m_GameStart.Joystick.transform.Translate(-m_GameStart.Joystick.GetComponent<PopOutWall>().TargetPosition * m_GameStart.Jump.transform.lossyScale.x);
		m_TutorialCursor.SetActive(false);

		yield return new WaitForSeconds(1);

		m_GameStart.Player.GetComponent<PlayerInput>().Hp = 100;

		m_GameStart.GameSystem.GetComponent<BossGenerator>().enabled = true;
		m_GameStart.Joystick.GetComponent<PopOutWall>().Move();
		m_GameStart.Jump.GetComponent<PopOutWall>().Move();
		m_GameStart.Score.GetComponent<PopOutWall>().Move();
		m_GameStart.Pause.GetComponent<PopOutWall>().Move();

		yield return new WaitForSeconds(1);
		m_GameStart.Score.GetComponent<ScoreSystem>().Initialize();

	}
}
