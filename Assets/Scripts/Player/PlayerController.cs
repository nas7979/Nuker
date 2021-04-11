using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Controller
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour, IController
    {
        [Range(1, 30)]
        [SerializeField] float m_JumpForce = 10f;

        [Range(1, 20)]
        [SerializeField] float m_Speed = 10f;

        private Rigidbody2D rb;
        private PlayerInput playerInput;
		private Animator m_Animator = null;

        private void OnEnable()
        {
            GameSystem.Instance.SubscribeInitalizeListener(Initialize);
            GameSystem.Instance.SubscribeUpdateListener(FrameMove);
        }
        private void OnDisable()
        {
            GameSystem.Instance.UnSubscribeInitalizeListener(Initialize);
            GameSystem.Instance.UnSubscribeUpdateListener(FrameMove);
        }

		private void Start()
		{
			if (!TryGetComponent(out rb))
				Debug.Assert(rb);
			if (!TryGetComponent(out playerInput))
				Debug.Assert(playerInput);
			m_Animator = GetComponent<Animator>();
		}

		public void Initialize()
        {
        }

        public void Jump()
        {
            if (!playerInput.isAbleJump)
                return;
            rb.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
            playerInput.isAbleJump = !playerInput.isAbleJump;
			m_Animator.SetBool("IsJumping", true);
			SoundManager.Instance.PlaySound("SFX_Jump");

            StateListener.Instance?.SetFlag(
                EStateFlag.OnClick_JumpBtn
                , StateListener.EListenerState.SetOn);
		}

        public void FrameMove()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            if (Input.GetKeyDown(KeyCode.Space))
                Jump();

            // 장애물 충돌시 끊김 현상 방지차원으로 AddForce로 바꿈
			if(playerInput.IsDashing() == false)
            rb.velocity = new Vector2(m_Speed, rb.velocity.y);

            //rb.AddRelativeForce(Vector2.right * m_Speed, ForceMode2D.Force);
            //transform.Translate(new Vector3(m_Speed * Time.deltaTime, 0, 0));
        }
    }
}