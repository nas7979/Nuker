using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
	[SerializeField] Sprite PlaySprite;
	[SerializeField] Sprite PauseSprite;
	private Button m_PauseButton;
    private void Awake()
    {
		m_PauseButton = GetComponent<Button>();

	}
    public void OnClick()
	{
		if (PauseSystem.m_pauseState == PauseState.PLAY)
		{
			m_PauseButton.image.sprite = PauseSprite;
			PauseSystem.OnPause();
			SoundManager.Instance.PlaySound("SFX_Button8");
		}
		else if (PauseSystem.m_pauseState == PauseState.PAUSE)
		{
			m_PauseButton.image.sprite = PlaySprite;
			PauseSystem.OnResume();
			SoundManager.Instance.PlaySound("SFX_Button13");
		}
	}
}
