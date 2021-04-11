using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPoint : MonoBehaviour
{
    private const int m_Max_Act_Count = 4;

    [Range(0.1f, 5.0f)]
    [SerializeField] float m_fRecoveryActionPointTime = 1.0f;

    [SerializeField] int m_nActPointCount = 0;

    [SerializeField] GameObject[] act_objects = new GameObject[m_Max_Act_Count];

    public bool IsRemainActionPoint() => m_nActPointCount > 0;
    // Call this Method, if you want to Control Action Point
    public void AddPoint(int amount)
    {
        m_nActPointCount += amount;
        m_nActPointCount = Mathf.Min(m_nActPointCount, m_Max_Act_Count);
        m_nActPointCount = Mathf.Max(m_nActPointCount, 0);

        bool active = (amount > 0) ? true : false;

        if (active) act_objects[m_nActPointCount - 1].SetActive(active);
        else act_objects[m_nActPointCount].SetActive(active);
    }

    private void OnEnable() => GameSystem.Instance?.SubscribeInitalizeListener(Initialize);

    private void OnDisable() => GameSystem.Instance?.UnSubscribeInitalizeListener(Initialize);

    [ContextMenu("TEST_DECREASE")]
    public void TEST_DECREASE()
    {
        AddPoint(-1);
    }

    [ContextMenu("TEST_INCREASE")]
    public void TEST_INCREASE()
    {
        AddPoint(1);
    }

	private void Start()
	{
		m_nActPointCount = m_Max_Act_Count;
		StartCoroutine(RecoveryActionPoint());
	}

	public void Initialize()
    {
        
    }

    IEnumerator RecoveryActionPoint()
    {
        while(gameObject.activeInHierarchy)
        {
            AddPoint(1);
            yield return new WaitForSeconds(m_fRecoveryActionPointTime);
        }
    }
}
