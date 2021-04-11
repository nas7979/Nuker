using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_Buildings : MonoBehaviour, IController
{
	[SerializeField] private GameObject[] m_Buildings = new GameObject[5];
	[SerializeField] private CameraMovement m_Camera;
	[SerializeField] private float m_DelayDistance;
	[SerializeField][Range(0.1f, 0.99f)] private float m_Speed;
	private Vector3 m_PrevCamPos;
	private LinkedList<int> m_CurrentSet = new LinkedList<int>();
	private float m_CamSize;
	private float m_CamMovedDistance;

	private void OnEnable()
	{
		GameSystem.Instance.SubscribeInitalizeListener(Initialize);
		GameSystem.Instance.SubscribeUpdateListener(FrameMove);
	}

	private void OnDisable()
	{
		GameSystem.Instance.UnSubscribeInitalizeListener(Initialize);
		GameSystem.Instance.UnSubscribeUpdateListener(FrameMove);

		for(int i = 0; i < m_Buildings.Length; i++)
		{
			m_Buildings[i].SetActive(true);
		}
	}

	public void Initialize()
	{
		InitalizeSet();
		m_CamSize = m_Camera.GetCameraWidth() * 0.5f;
	}

	public void FrameMove()
	{
		
	}

	/*
	 * 멈춘 원인(Freezing) : 빌딩을 찾는 도중에 빌딩이 5개가 모두 활성화 되어 있다면 while문은 빌딩을 찾을 때까지 게속 루프를 돌기 때문에
	 * 아래에 있는 루프 문에서 무한루프가 발생, 따라서 나머지 기능들을 동작할 수 없게되어서 스크린 Freezing이 발생
	 * 
	 * 따라서 안정성을 위해 코루틴을 사용함
	 * 
	 * Reference by https://answers.unity.com/questions/1309703/unity-while-loop-freeze.html
	 */
	private IEnumerator<bool> FindBuilding()
    {
		int Val;
		LinkedListNode<int> iter;
		do
		{
			iter = m_CurrentSet.First;
			Val = Random.Range(0, m_CurrentSet.Count);
			for (int i = 0; i < Val; i++)
			{
				iter = iter.Next;
			}
			Val = iter.Value;
			yield return false;
		} while (m_Buildings[Val].activeInHierarchy == true);

		m_CurrentSet.Remove(Val);
		m_Buildings[Val].SetActive(true);
		m_Buildings[Val].transform.localScale = new Vector3(1, 1) * Random.Range(0.5f, 1f);
		m_Buildings[Val].transform.localPosition = new Vector3(m_Camera.transform.position.x + m_CamSize + 5, -6, 0);
		m_Buildings[Val].GetComponent<SpriteRenderer>().sortingOrder = (int)(m_Buildings[Val].transform.localScale.x * 100) - 10;
		yield return true;
	}
	private void FixedUpdate()
	{
		Vector3 CamVel = m_Camera.GetVelocity();
		m_PrevCamPos = m_Camera.transform.position;
		m_CamMovedDistance -= CamVel.x;

		if (m_CamMovedDistance <= 0)
		{
			m_CamMovedDistance = m_DelayDistance;
			AddBuilding();
		}

		for (int i = 0; i < m_Buildings.Length; i++)
		{
			if (m_Buildings[i].activeInHierarchy == false)
				continue;
			m_Buildings[i].transform.position += new Vector3(CamVel.x * (1f - m_Buildings[i].transform.localScale.x) * m_Speed, 0);
			if (m_Buildings[i].transform.position.x + m_Buildings[i].GetComponent<SpriteRenderer>().size.x * 0.5f <= m_Camera.transform.position.x - m_CamSize)
			{
				m_Buildings[i].SetActive(false);
			}
		}
	}

	private void AddBuilding()
	{
		if(m_CurrentSet.Count == 0)
		{
			InitalizeSet();
		}

		StartCoroutine(FindBuilding());

		//int Val;

		
		//LinkedListNode<int> iter;

  //      do
  //      {
  //          iter = m_CurrentSet.First;
  //          Val = Random.Range(0, m_CurrentSet.Count);
  //          for (int i = 0; i < Val; i++)
  //          {
  //              iter = iter.Next;
  //          }
  //          Val = iter.Value;
  //          Debug.Log("A");
  //      } while (m_Buildings[Val].activeInHierarchy == true);

  //      m_CurrentSet.Remove(Val);
		//m_Buildings[Val].SetActive(true);
		//m_Buildings[Val].transform.localScale = new Vector3(1, 1) * Random.Range(0.5f, 1f);
		//m_Buildings[Val].transform.localPosition = new Vector3(m_Camera.transform.position.x + m_CamSize + 5, -5, 0);
		//m_Buildings[Val].GetComponent<SpriteRenderer>().sortingOrder = (int)(m_Buildings[Val].transform.localScale.x * 100) - 10;
	}

	private void InitalizeSet()
	{
		List<int> List = new List<int>();
		int Val;
		for(int i = 0; i < 5; i++)
		{
			List.Add(i);
		}

		for(int i = 0; i < 5; i++)
		{
			Val = Random.Range(0, List.Count);
			m_CurrentSet.AddLast(List[Val]);
			List.RemoveAt(Val);
		}
	}
}
