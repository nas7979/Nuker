using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilitys.BitFlag;

public class StateListenerTest : MonoBehaviour
{
    private void OnEnable()
    {
        StateListener.Instance.stateListener += StateListener_stateListener;
    }
    private void OnDisable()
    {
        StateListener.Instance.stateListener -= StateListener_stateListener;
    }
    private void StateListener_stateListener(BitFlag<EStateFlag> obj)
    {
        //if (obj.HasFlag(EStateFlag.OnTouchUp))
        //{
        //    Debug.Log("터치 업");
        //}

        //if (obj.HasFlag(EStateFlag.OnPlayerDash))
        //{
        //    Debug.Log("대쉬");
        //}
        //if (obj.HasFlag(EStateFlag.OnClick_JumpBtn))
        //{
        //    Debug.Log("점프 버튼 클릭");
        //}
    }
}
