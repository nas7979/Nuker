using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartAction : MonoBehaviour
{
    public static bool IsHomeStart = false;
    public UnityEvent OnStartAction;

    public void Start()
    {
        if (IsHomeStart)
            ClickStart();
        IsHomeStart = false;
    }
    public void SetHomeStart(bool isHomeStart)
    {
        IsHomeStart = isHomeStart;
    }
    public void ClickStart()
    {
        OnStartAction.Invoke();
    }
}
