using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Slice : MonoBehaviour
{
	public Vector3 m_StartPosition = Vector3.zero;

	private void Awake()
	{
		m_StartPosition = transform.localPosition;
	}

	private void OnEnable()
	{
		transform.localPosition = m_StartPosition;
		transform.localRotation = Quaternion.Euler(0, 0, 0);
	}

	private void OnBecameInvisible()
	{
		transform.parent.GetComponent<Obstacle_DestroyEffect>().InvisibleCount--;
		gameObject.SetActive(false);
	}
}
