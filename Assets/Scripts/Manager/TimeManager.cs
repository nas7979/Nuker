using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoSingleton<TimeManager>
{
	public void SetTimeScale(float _Time, float _Speed)
	{
		StartCoroutine(ISetTimeScale(_Time, _Speed));
		Time.timeScale = _Time;
		Time.fixedDeltaTime = 0.02f * _Time;
	}

	private IEnumerator ISetTimeScale(float _Time, float _Speed)
	{
		yield return new WaitForSecondsRealtime(_Time);
		Time.timeScale = 1;
		Time.fixedDeltaTime = 0.02f;
	}
}
