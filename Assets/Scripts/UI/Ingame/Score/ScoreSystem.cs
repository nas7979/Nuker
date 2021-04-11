using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class ScoreSystem : MonoBehaviour
{
    [SerializeField] protected uint m_nScore = 0;

    public uint GetScore() => m_nScore;

    [Range(0.001f, 1.0f)]
    [SerializeField] protected float m_fUploadSpeed = 1f;

    [Range(0.01f, 1.0f)]
    [SerializeField] protected float m_fIncreaseSpeed = 1f;

    [Range(1, 20)]
    [SerializeField] protected uint m_fIncreaseScore = 1;

    [SerializeField] protected bool m_bUseIncreaseUnit = true;

    protected uint increaseUnit = 2;

    public static System.Action<uint> OnAddScore;
    
    public void Initialize()
    {
        OnAddScore = AddScore;
        
        // Load or Setting Score Field
        StartCoroutine(ScoreLoop());
    }
    IEnumerator ScoreLoop()
    {
        while (this.gameObject.activeInHierarchy)
        {
            AddScore(m_fIncreaseScore);
            yield return new WaitForSeconds(m_fIncreaseSpeed);
        }
    }
    public void AddScore(uint amount)
    {
        StartCoroutine(IncreaseScore(amount));
    }

    public uint GetScoreUnit(uint increaseAmount)
    {
        uint Unit = (uint)Mathf.FloorToInt(Mathf.Log10(increaseAmount)) + 1;
        switch (Unit)
        {
            case 1:
            case 2:
                return 2;
            case 3:
                return 10;
            case 4:
                return 100;
            default:
                return 1000;
        }
    }

    IEnumerator IncreaseScore(uint increaseAmount)
    {
        uint currentScore = m_nScore;

        
        if (m_bUseIncreaseUnit)
            increaseUnit = GetScoreUnit(increaseAmount);

        
        while (m_nScore < currentScore + increaseAmount)
        {
            m_nScore += increaseUnit;
            ScoreFunc(increaseAmount);
            yield return new WaitForSecondsRealtime(m_fUploadSpeed);
        }
        yield return null;
    }

    protected virtual void ScoreFunc(uint increaseAmount) { }


}
