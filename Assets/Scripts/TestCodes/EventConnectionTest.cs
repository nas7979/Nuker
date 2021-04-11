using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConnectionTest : MonoBehaviour
{
    private void OnEnable()
    {
        //GameSystem.Instance.SubscribeUpdateListener(FrameMove);
    }
    public void FrameMove()
    {
        Debug.Log("EventConnectionTest");
    }
    private void OnDisable()
    {
        //GameSystem.Instance.UnSubscribeUpdateListener(FrameMove);
    }
}
