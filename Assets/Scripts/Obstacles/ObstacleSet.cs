using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSet : MonoBehaviour, IController
{
	[SerializeField] private GameObject[] m_NextObstacles = new GameObject[1];
	[SerializeField] private float _size = 0;
	public float Size
	{
		get { return _size; }
	}

	private CameraMovement _camera;
	public CameraMovement Camera
	{
		set { _camera = value; }
	}

	private ObstacleGenerator m_Generator;
	private bool m_CanCreateAnotherObstacle = true;


	private void OnEnable()
	{
		GameSystem.Instance.SubscribeInitalizeListener(Initialize);
		GameSystem.Instance.SubscribeUpdateListener(FrameMove);

		for(int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(true);
		}
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
		if (_camera.transform.position.x + _camera.GetCameraWidth() * 0.5f >= transform.position.x + _size && m_CanCreateAnotherObstacle == true)
		{
			m_CanCreateAnotherObstacle = false;
			m_Generator.CreateObstacle(transform.position.x + _size, m_NextObstacles);
		}

		if (_camera.transform.position.x - _camera.GetCameraWidth() * 0.5f >= transform.position.x + _size)
		{
			gameObject.SetActive(false);
			m_CanCreateAnotherObstacle = true;
		}
	}

	public void SetGenerator(ObstacleGenerator _Generator)
	{
		m_Generator = _Generator;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Vector3 Pos = transform.position + new Vector3(_size, 0, 0);
		Gizmos.DrawLine(Pos + new Vector3(-1, 0), Pos + new Vector3(1, 0));
		Gizmos.DrawLine(Pos + new Vector3(0, -1), Pos + new Vector3(0, 1));

		Gizmos.DrawLine(new Vector2(0, 0), new Vector2(1000, 0));
	}

}
