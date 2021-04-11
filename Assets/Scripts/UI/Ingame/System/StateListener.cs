using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilitys.BitFlag;

[System.Flags]
public enum EStateFlag : int
{
    None                        = 0,
    OnTouch                     = 1 << 0,
    OnTouchDrag                 = 1 << 1,
    OnTouchUp                   = 1 << 2,
    OnClick_JumpBtn             = 1 << 3,
    OnClick_StartBtn            = 1 << 4,
    OnPlayerColl_Obstacle       = 1 << 5,
    OnPlayerDashColl_Obstacle   = 1 << 6,
    OnPlayerDash                = 1 << 7,
    OnPlayerDashing             = 1 << 8,
	OnTutorialJumpGuide			= 1 << 9,
	OnTutorialDashGuide			= 1 << 10,
    All                         = int.MaxValue
}
public class StateListener : MonoSingleton<StateListener>
{
    public enum EListenerState
    {
        Set,
        SetOn,
        SetOff
    }


    private BitFlag<EStateFlag> eStateFlag = new BitFlag<EStateFlag>();
    public event System.Action<BitFlag<EStateFlag>> stateListener = delegate {};

    private void Awake()
    {
        if (instance != null)
            DestroyImmediate(this);

        DontDestroyOnLoad(this.gameObject);
    }
    private void OnEnable()
    {
        eStateFlag.Clear();
        //StartCoroutine(StateLoop());
    }

    public void SetFlag(EStateFlag eState, EListenerState eListenerState)
    {
        switch (eListenerState)
        {
            case EListenerState.Set:
                eStateFlag.Set(eState);
                break;
            case EListenerState.SetOn:
                eStateFlag.SetOn(eState);
                break;
            case EListenerState.SetOff:
                eStateFlag.SetOff(eState);
                break;
            default:
                break;
        }
        stateListener?.Invoke(eStateFlag);
    }

	public bool GetFlag(EStateFlag eState)
	{
		return eStateFlag.HasFlag(eState);
	}

    //IEnumerator StateLoop()
    //{
    //    while (gameObject.activeInHierarchy)
    //    {
    //        yield return new WaitForEndOfFrame();
    //        eStateFlag.Clear();
    //        yield return null;
    //    }
    //}
}
