using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGenerator : MonoBehaviour
{
	[SerializeField] private GameObject[] m_Bosses = new GameObject[1];
	[SerializeField] private float m_DistancePerBoss = 200;

	public void OnEnable()
	{
		CreateBoss();
	}

	public void CreateBoss()
	{
		GameObject Temp = m_Bosses[Random.Range(0, m_Bosses.Length)];
		Temp.SetActive(true);
		Temp.GetComponent<BossBase>().SetDistance(m_DistancePerBoss);
		Temp.GetComponent<BossBase>().BossInit();
	}
}
