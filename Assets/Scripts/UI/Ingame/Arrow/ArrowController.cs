using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput = null;

    [SerializeField] GameObject dot = null;
    [SerializeField] GameObject lastArrow = null;

    [Range(1.0f, 100.0f)]
    [SerializeField] float dotSpacing = 40.0f;
    List<GameObject> dots = new List<GameObject>();
    float maxDistnace = 0;
    float timeStamp = 0;
    float stamp = 0;

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject inst_dot = MonoBehaviour.Instantiate(dot, this.transform);
            inst_dot.transform.position = playerInput.GetTransform.position;
            dots.Add(inst_dot);
        }
        GameObject inst_arrow = MonoBehaviour.Instantiate(lastArrow, this.transform);
        inst_arrow.transform.position = playerInput.GetTransform.position;
        dots.Add(inst_arrow);
    }
    private void OnEnable()
    {
        GameSystem.Instance.SubscribeUpdateListener(FrameMove);
    }
    private void OnDisable()
    {
        GameSystem.Instance.UnSubscribeUpdateListener(FrameMove);
    }
    
    public void FrameMove()
    {
        maxDistnace = playerInput.GetInputDistanceToPercent();
        timeStamp = maxDistnace / dots.Count;
        timeStamp /= dotSpacing;
        stamp = 0;

        Vector3 dotPositon = Vector3.zero;
        Vector3 playerPosition = playerInput.GetTransform.position;
        for (int i = 0; i < dots.Count; i++)
        {
            Vector3 dir = (playerPosition + playerInput.InputDir);
            dotPositon = Camera.main.WorldToScreenPoint(dir + (playerInput.InputDir * stamp));
            dots[i].transform.position = dotPositon;
            stamp += timeStamp;
        }
    }
}
