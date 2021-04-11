using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour, IController
{
	[SerializeField] private GameObject[] m_ObstaclePrefabs = new GameObject[1];
	[SerializeField] private GameObject[] m_StartObstacles = new GameObject[1];
	[SerializeField] private GameObject[] m_OnBossObstaclePrefabs = new GameObject[1];
	[SerializeField] private GameObject[] m_TutorialObstacles = new GameObject[1];
	[SerializeField] private float m_ObstacleYOffset = 0;
	[SerializeField] private CameraMovement m_Camera;
	private bool m_IsInBossBattle = true;
	private bool m_IsInTutorial = false;
	public bool IsInBossBattle
	{
		get { return m_IsInBossBattle; }
		set
		{
			m_IsInBossBattle = value;
		}
	}
	private SortedList<string, GameObject> m_Obstacles = new SortedList<string, GameObject>();
	private List<GameObject> m_OnBossObstacles = new List<GameObject>();

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

	public void Initialize()
	{
		m_IsInBossBattle = true;
		GameObject Temp;
		foreach (var iter in m_ObstaclePrefabs)
		{
			Temp = Instantiate(iter);
			Temp.name = iter.name;
			Temp.SetActive(false);
			Temp.GetComponent<ObstacleSet>().SetGenerator(this);
			Temp.GetComponent<ObstacleSet>().Camera = m_Camera;
			m_Obstacles.Add(iter.name, Temp);
		}
		foreach (var iter in m_OnBossObstaclePrefabs)
		{
			Temp = Instantiate(iter);
			Temp.name = iter.name;
			Temp.SetActive(false);
			Temp.GetComponent<ObstacleSet>().SetGenerator(this);
			Temp.GetComponent<ObstacleSet>().Camera = m_Camera;
			m_OnBossObstacles.Add(Temp);
		}
		CreateObstacle(m_Camera.transform.position.x + m_Camera.GetCameraWidth(), m_StartObstacles);
	}

	public void FrameMove()
	{

	}

	public void CreateObstacle(float _XPos, GameObject[] _Next)
	{
		GameObject ObstacleTemp;
		if (m_IsInTutorial && !m_IsInBossBattle)
		{
			m_IsInTutorial = false;
			ObstacleTemp = m_Obstacles[m_TutorialObstacles[0].name];
			ObstacleTemp.SetActive(true);
			ObstacleTemp.transform.position = new Vector2(_XPos, m_ObstacleYOffset);
		}
		else
		{
			while (true)
			{
				if (!m_IsInBossBattle)
				{
					ObstacleTemp = m_Obstacles[_Next[Random.Range(0, _Next.Length)].name];
				}
				else
				{
					ObstacleTemp = m_OnBossObstacles[Random.Range(0, m_OnBossObstacles.Count)];
				}
				if (ObstacleTemp.activeInHierarchy == false)
				{
					ObstacleTemp.SetActive(true);
					ObstacleTemp.transform.position = new Vector2(_XPos, m_ObstacleYOffset);
					return;
				}
			}
		}
	}

	public void StartTutorial()
	{
		m_IsInTutorial = true;
		GameObject Temp;
		for (int i = 0; i < m_TutorialObstacles.Length; i++)
		{
			Temp = Instantiate(m_TutorialObstacles[i]);
			Temp.name = m_TutorialObstacles[i].name;
			Temp.SetActive(false);
			Temp.GetComponent<ObstacleSet>().SetGenerator(this);
			Temp.GetComponent<ObstacleSet>().Camera = m_Camera;
			m_Obstacles.Add(m_TutorialObstacles[i].name, Temp);
		}
	}
}
