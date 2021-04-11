using System.Diagnostics;
using System.Collections;
using UnityEngine;

public class Utils
{
    [Conditional("UNITY_EDITOR")]
    public static void Log(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogError(object message)
    {
        UnityEngine.Debug.LogError(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

    public static int ConvertBoolToInt(bool condition)
    {
        return (condition) ? 1 : 0;
    }

    public static bool ConvertIntToBool(int condition)
    {
        if (condition == 0 || condition == 1)
            return (condition == 1) ? true : false;
        LogError("Invalid content");
        return false;
    }
}

public static class Vibration
{

#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaObject vibrator;
#endif

    public static void Vibrate()
    {
        if (!DataSystem.IsVibrate)
            return;
        if (isAndroid())
            vibrator.Call("vibrate");
#if UNITY_ANDROID
		else
            Handheld.Vibrate();
#endif
    }


    public static void Vibrate(long milliseconds)
    {
        if (!DataSystem.IsVibrate)
            return;
        if (isAndroid())
            vibrator.Call("vibrate", milliseconds);
#if UNITY_ANDROID
		else
            Handheld.Vibrate();
#endif
	}

	public static void Vibrate(long[] pattern, int repeat)
    {
        if (!DataSystem.IsVibrate)
            return;
        if (isAndroid())
            vibrator.Call("vibrate", pattern, repeat);
#if UNITY_ANDROID
		else
            Handheld.Vibrate();
#endif
	}

	public static bool HasVibrator()
    {
        return isAndroid();
    }

    public static void Cancel()
    {
        if (isAndroid())
            vibrator.Call("cancel");
    }

    private static bool isAndroid()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
	return true;
#else
        return false;
#endif
    }
}