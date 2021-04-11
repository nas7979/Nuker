using System;
using UnityEngine;

public class DataSystem : MonoBehaviour
{
    public const string Key_Vibrate = "Vibrate";
    public const string Key_Sound = "Sound";
    public const string Key_RightHand = "RightHand";

    public static bool IsVibrate = true;
    public static bool IsSound = true;
    public static bool IsRightHand = true;

    static Action<string, bool> OnOptionChanged = delegate { };

    private void Awake()
    {
        OnOptionChanged = delegate { };
        OnOptionChanged += UpdateLocalState;
    }

    public static void SetOption(in string key, bool active)
    {
        PlayerPrefs.SetInt(key, Utils.ConvertBoolToInt(active));
        PlayerPrefs.Save();
        OnOptionChanged.Invoke(key, active);
    }
    public static bool GetOption(in string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
        }

        return Utils.ConvertIntToBool(PlayerPrefs.GetInt(key));
    }

    public static void UpdateLocalState(string key, bool condition)
    {
        switch (key)
        {
            case Key_Vibrate:
                IsVibrate = condition;
                break;
            case Key_Sound:
                IsSound = condition;
				SoundManager.Instance.SetSFXVolume(condition ? 1 : 0);
				SoundManager.Instance.SetBGMVolume(condition ? 1 : 0);
                break;
            case Key_RightHand:
                IsRightHand = condition;
                break;
            default:
                break;
        }
    }
}