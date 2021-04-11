using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour, IController
{
    [SerializeField] private PlayerInput playerInput = null;

    [SerializeField] private Image m_hpImageSource = null;
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
	public void Start()
	{
		if (!TryGetComponent(out playerInput))
			Debug.Assert(playerInput);
	}
	public void Initialize()
    {
        
    }
    public void FrameMove()
    {
        m_hpImageSource.fillAmount = playerInput.Hp * 0.01f;
    }


}
