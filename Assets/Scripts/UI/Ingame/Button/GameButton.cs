using InGame.Manager;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameButton : ButtonEvent
{
    void Start()
    {
        base.Start();
    }

    public void _reStart()
    {
        PauseSystem.OnResume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void _ClickRankButton()
    {
        SoundManager.Instance.PlaySound("SFX_Button8");
    }

    // Click Settings Button
    public void _SettingIn(PopOutWall t)
    {
        if (t.IMoveCoroutine != null)
            t.StopCoroutine(t.IMoveCoroutine);
        t.gameObject.SetActive(true);
        t.TargetPosition = new Vector2(300, 0);
        t.Move();
		SoundManager.Instance.PlaySound("SFX_Button8");
	}

	//Click Exit Button
	public void _SettingOut(PopOutWall t)
    {
        if (t.IMoveCoroutine != null)
            t.StopCoroutine(t.IMoveCoroutine);
        t.TargetPosition = new Vector2(-300f, 0);
        t.SpeedMultiplyer = 1.05f;
        t.Move();
        StartCoroutine(Active(t.gameObject, false, 0.2f));
		SoundManager.Instance.PlaySound("SFX_Button8");
	}
	public IEnumerator Active(GameObject @object, bool condition, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        @object.SetActive(condition);
    }
}
