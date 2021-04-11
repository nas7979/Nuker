using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StringList
{
	public List<string> List = new List<string>();
}

public class Request : MonoBehaviour, IController
{
	[SerializeField] private CameraMovement m_Camera;
	[SerializeField] private GameObject m_GameSystem;
	[SerializeField] private List<StringList> m_Texts = new List<StringList>();
	private Text m_Text;

	private void OnEnable()
	{
		GameSystem.Instance.SubscribeInitalizeListener(Initialize);
		GameSystem.Instance.SubscribeUpdateListener(FrameMove);

		m_Text = transform.GetChild(0).GetComponent<Text>();
		transform.localScale = new Vector2(0, 0);
		transform.localPosition = new Vector3(0, 0, -10);
		m_Text.text = "";
		for(int i = 0; i < m_Texts.Count; i++)
		{
			m_Text.text += m_Texts[i].List[Random.Range(0, m_Texts[i].List.Count)];
		}

		StartCoroutine(IAppear());
	}

	private void OnDisable()
	{
		GameSystem.Instance.UnSubscribeInitalizeListener(Initialize);
		GameSystem.Instance.UnSubscribeUpdateListener(FrameMove);
	}

	public void Initialize()
	{

	}

	public void FrameMove()
	{
		
	}

	private IEnumerator IAppear()
	{
		yield return new WaitForSeconds(2);
		transform.localScale = new Vector2(0.001f, 0.001f);
		while (transform.localScale.x <= 1.5f)
		{
			transform.localScale *= 1.08f;
			transform.Rotate(0, 0, 1500 * Time.unscaledDeltaTime);
			yield return null;
		}
		transform.localScale = new Vector2(2f, 2f);
		transform.rotation = Quaternion.Euler(0, 0, 10);
		m_Camera.Shake(2, 1);

		while(!Input.GetKeyDown(KeyCode.Mouse0))
		{
			yield return null;
		}

		float Speed = -100;
		for(int i = 0; i < 100; i++)
		{
			Speed *= 1.07f;
			transform.Translate(Speed * Time.unscaledDeltaTime, 0, 0, Space.World);
			yield return null;
		}

		yield return new WaitForSeconds(2);
		gameObject.SetActive(false);
		m_GameSystem.GetComponent<ObstacleGenerator>().IsInBossBattle = false;
		m_GameSystem.GetComponent<BossGenerator>().CreateBoss();
	}
}