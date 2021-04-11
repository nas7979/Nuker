using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScore : ScoreSystem
{
    [SerializeField] Text m_GameOverScoreText = null;
    [SerializeField] Text m_GameOverHighScoreText = null;
    [SerializeField] GameScore m_GameScore;
    uint offset;

    private void Awake()
    {
        Debug.Assert(m_GameOverScoreText != null, "NullRefererece");
        Debug.Assert(m_GameOverHighScoreText != null, "NullRefererece");
        Debug.Assert(m_GameScore != null, "NullRefererece");
		SoundManager.Instance.PlaySound("Gameover");
		AdsManager.Instance.ShowInterstitialAd();
    }
    private void OnEnable()
    {
		if (m_GameScore.GetScore() > SaveSystem.Instance.Data.HighScore)
		{
			Debug.Log(SaveSystem.Instance.Data.HighScore);
			SaveSystem.Instance.Data.HighScore = (int)m_GameScore.GetScore();
            BackEndRTRank.Instance.UpdateRTRank((int)m_GameScore.GetScore());
		}
        AddScore(m_GameScore.GetScore());
    }

    protected override void ScoreFunc(uint increaseAmount)
    {
        offset = increaseAmount - m_nScore;
        increaseUnit = GetScoreUnit(offset);
        m_GameOverScoreText.text = string.Format("{0}", m_nScore.ToString());
        m_GameOverHighScoreText.text = string.Format("{0}", SaveSystem.Instance.Data.HighScore);
    }

	public void PlayButtonSound()
	{
		SoundManager.Instance.PlaySound("SFX_Button8");
	}
}
