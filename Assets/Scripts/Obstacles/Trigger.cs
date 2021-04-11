using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
	[SerializeField] private UnityEvent m_OnTriggered = null;
	[SerializeField] private string[] m_Tags = new string[1];
	private BoxCollider2D m_Collider = null;

	private void Awake()
	{
		m_Collider = GetComponent<BoxCollider2D>();
	}

	private void OnEnable()
	{
		m_Collider.enabled = true;
	}

	private void OnTriggerStay2D(Collider2D collider)
	{
		for(int i = 0; i < m_Tags.Length; i++)
		{
			if (collider.tag.Equals(m_Tags[i]))
			{
				m_OnTriggered.Invoke();
				return;
			}
		}
	}
}
