using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject m_gameOverObject;
    [SerializeField] PlayerInput m_playerInput;

    private void Awake()
    {
        Debug.Assert(m_gameOverObject != null, "NullReference");
        Debug.Assert(m_playerInput != null, "NullReference");
        m_playerInput.m_OnPlayerDead = () =>
        {
            m_gameOverObject.SetActive(true);
            GameSystem.Instance.UnSubscribeUpdateListener(m_playerInput.FrameMove);
            PauseSystem.OnPause();
        };
    }
}
