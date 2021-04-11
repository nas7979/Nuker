using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PauseState
{
    PAUSE = 0,
    PLAY
}

public class PauseSystem : MonoBehaviour
{
    public static PauseState m_pauseState = PauseState.PLAY;
    public static System.Action OnPause;
    public static System.Action OnResume;

    private void Awake()
    {
        Resume();
        OnPause = Pause;
        OnResume = Resume;
    }
	public void Pause()
	{
		m_pauseState = PauseState.PAUSE;
		Time.timeScale = 0.0f;
	}
    public void Resume()
	{
		m_pauseState = PauseState.PLAY;
		Time.timeScale = 1f;
	}
}
