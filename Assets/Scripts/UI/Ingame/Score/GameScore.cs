using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScore : ScoreSystem
{
    [SerializeField] private Text m_ScoreTxt = null;
    [SerializeField] private Text m_HighScoreTxt = null;

    private void Awake()
    {
        Debug.Assert(m_ScoreTxt != null, "NullReference");
        Debug.Assert(m_HighScoreTxt != null, "NullReference");
    }

    public void OnEnable()
    {
        Menu_Buttons.OnClickStartButton += () =>
        {
            m_HighScoreTxt.text = BackEndRTRank.Instance.MyRTRank.rankData.score.ToString();
        };
    }
    protected override void ScoreFunc(uint increaseAmount)
    {
        m_ScoreTxt.text = m_nScore.ToString();
    }
}
