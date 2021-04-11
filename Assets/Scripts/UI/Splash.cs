using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
	AsyncOperation m_Scene;
	private void Start()
	{
		m_Scene = SceneManager.LoadSceneAsync("Main");
		m_Scene.allowSceneActivation = false;
	}

	public void ChangeScene()
	{
		m_Scene.allowSceneActivation = true;
	}
}
