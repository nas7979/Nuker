using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rader
{
    private Dictionary<Transform, Transform> m_TargetingIndex;
    private string[] targetingList = { };
    public Rader()
        => m_TargetingIndex = new Dictionary<Transform, Transform>();
    public Rader(string[] ps)
    {
        targetingList = ps;
        m_TargetingIndex = new Dictionary<Transform, Transform>();
    }

    public void SetTargetingTags(string[] ps)
        => targetingList = ps;

    public void AppendAndCheck(Collider2D[] overlapColliders)
    {
        foreach (var collider in overlapColliders)
        {
            foreach (var tag in targetingList)
            {
                if (collider.CompareTag(tag))
                {
					if (!m_TargetingIndex.ContainsKey(collider.transform))
					{
						GameObject @object = ObjectManager.Instance.AddObject("Targeting", collider.transform.position);
						m_TargetingIndex.Add(collider.transform, @object.transform);
					}
					else
					{
						Transform targetingTrans = m_TargetingIndex[collider.transform];
						targetingTrans.localPosition = collider.transform.position;
                    }
				}
            }
            
        }
		foreach(var iter in m_TargetingIndex)
		{
			if(!iter.Key.gameObject.activeInHierarchy)
			{
				iter.Value.gameObject.SetActive(false);
				m_TargetingIndex.Remove(iter.Key);
				break;
			}
		}
    }


    public bool ClearTargetingList()
    {
        if (m_TargetingIndex.Count > 0)
        {
            m_TargetingIndex.Values.ToList().ForEach(e =>
            {
                e.gameObject.SetActive(false);
            });
            m_TargetingIndex.Clear();
            return true;
        }
        return false;
    }
    public Collider2D[] OverlapColliderList(Vector2 position, LayerMask layerMask) => Physics2D.OverlapCircleAll(position, 50f, layerMask);
}

public class PlayerRadar : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;

    private PlayerInput m_PlayerInput;

    private Rader m_Rader;
    Collider2D[] overlapColliders;


    private void Awake()
    {
        m_Rader = new Rader();
        m_Rader.SetTargetingTags(new[] { "DestructibleWall" });
        m_PlayerInput = GetComponent<PlayerInput>();
    }
    private void OnEnable()
    {
        StartCoroutine(Rader());
    }

    private void OnDisable()
    {
    }

    IEnumerator Rader()
    {
        while (gameObject.activeInHierarchy)
        {
            switch (m_PlayerInput.virtualJoystick.touchState)
            {
                case JoystickState.TOUCH_NONE:
                    m_Rader.ClearTargetingList();
                    break;
                case JoystickState.TOUCH_DOWN:
                    break;
                case JoystickState.TOUCH_DRAG:
                    overlapColliders = m_Rader.OverlapColliderList(transform.position, layerMask);
                    m_Rader.AppendAndCheck(overlapColliders);
                    break;
                case JoystickState.TOUCH_UP:

                    break;
                default:
                    break;
            }
            yield return null;
        }
    }
}
