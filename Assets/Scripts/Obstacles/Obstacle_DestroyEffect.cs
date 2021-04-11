using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_DestroyEffect : MonoBehaviour
{
	private int m_InvisibleCount = 100;
	public int InvisibleCount
	{
		get => m_InvisibleCount;
		set
		{
			m_InvisibleCount = value;
			if(m_InvisibleCount == 0)
			{
				gameObject.SetActive(false);
			}
		}
	}

	private void OnEnable()
	{
		for(int i = 1; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(true);
			transform.GetChild(i).GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			transform.GetChild(i).GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle * 12500);
		}
		m_InvisibleCount = transform.childCount - 1;
		SoundManager.Instance.PlaySound("SFX_Obstacle_Destroy");
	}
}
