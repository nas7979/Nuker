using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
	[SerializeField] private string m_DestroyEffectName = "";
	
	public void CreateDestroyEffect()
	{
		ObjectManager.Instance.AddObject(m_DestroyEffectName, transform.position);
	}
}
