using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BackgroundLayer
{
	[SerializeField] private GameObject[] _objects;
	[SerializeField] private float _speed;
	[SerializeField] private float _size;

	BackgroundLayer(GameObject[] _Objects, float _Speed, float _Size)
	{
		_objects = _Objects;
		_speed = _Speed;
		_size = _Size;
	}

	public GameObject[] Objects
	{
		get { return _objects; }
	}

	public float Speed
	{
		get { return _speed; }
	}

	public float Size
	{
		get { return _size; }
		set { _size = value; }
	}
}

public class BackgroundScroll : MonoBehaviour, IController
{
	[SerializeField] private float m_OverallSpeed = 1;
	[SerializeField] private CameraMovement m_Camera;
	[Header("Layers")]
	[SerializeField] private BackgroundLayer[] m_Layers = new BackgroundLayer[1];

	private Vector3 m_PrevCamPos;
	private float m_CamSize;

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
		m_PrevCamPos = m_Camera.transform.position;
		m_CamSize = m_Camera.GetCameraWidth() * 0.5f;
		for(int i = 0; i < m_Layers.Length; i++)
		{
			for (int j = 0; j < m_Layers[i].Objects.Length; j++)
			{
				m_Layers[i].Objects[j].transform.Translate(m_Layers[i].Size * j, 0, 0);
			}
		}
	}

	public void FrameMove()
	{
		
	}

	private void FixedUpdate()
	{
		Vector2 CamVel = m_Camera.GetVelocity();
		m_PrevCamPos = m_Camera.transform.position;
		BackgroundLayer Temp;

		for (int Layer = 0; Layer < m_Layers.Length; Layer++)
		{
			Temp = m_Layers[Layer];
			for (int i = 0; i < Temp.Objects.Length; i++)
			{
				Temp.Objects[i].transform.localPosition += new Vector3(CamVel.x * Temp.Speed, 0) * m_OverallSpeed;
				if (Temp.Objects[i].transform.position.x + Temp.Size * 0.5f <= m_Camera.transform.position.x - m_CamSize)
				{
					Temp.Objects[i].transform.Translate(Temp.Size * Temp.Objects.Length, 0, 0);
				}
			}
		}
	}
}
