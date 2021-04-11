using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoSingleton<ObjectManager>
{
	private class PoolData
	{
		public PoolData(GameObject _Prefab)
		{
			Prefab = _Prefab;
			Pool = new List<GameObject>();
		}
		public GameObject Prefab { get; }
		public List<GameObject> Pool { get; }
	}
	private SortedList<string, PoolData> m_Pools = new SortedList<string, PoolData>();
	public GameObject Player { get; set; }

	private void Awake()
	{
		GameObject[] Temp = Resources.LoadAll<GameObject>("Prefabs");
		for(int i = 0; i < Temp.Length; i++)
		{
			m_Pools.Add(Temp[i].name, new PoolData(Temp[i]));
		}
	}

	public GameObject AddObject(string _Name, Vector3 _Pos, float _Rot = 0)
	{
		PoolData Pool = m_Pools[_Name];
		GameObject Temp;
		for (int i = 0; i < Pool.Pool.Count; i++)
		{
			Temp = Pool.Pool[i];
			if (Temp.activeInHierarchy == false)
			{
				Temp.SetActive(true);
				Temp.transform.position = _Pos;
				Temp.transform.localRotation = Quaternion.Euler(0, 0, _Rot);
				return Temp;
			}
		}
		Temp = Instantiate(Pool.Prefab, _Pos, Quaternion.Euler(0, 0, _Rot));
		Temp.name = Pool.Prefab.name;
		Pool.Pool.Add(Temp);
		return Temp;
	}
}
