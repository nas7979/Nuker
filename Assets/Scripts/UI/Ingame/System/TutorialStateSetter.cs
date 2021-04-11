using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStateSetter : MonoBehaviour
{
	public void SetFlag(int eState)
	{
		StateListener.Instance.SetFlag(EStateFlag.All, StateListener.EListenerState.SetOff);
		StateListener.Instance.SetFlag((EStateFlag)eState, StateListener.EListenerState.SetOn);
	}
}
