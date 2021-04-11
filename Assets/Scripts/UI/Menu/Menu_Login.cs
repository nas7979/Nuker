using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class Menu_Login : MonoBehaviour
{
	[SerializeField] private InputField m_Input_Name = null;
	[SerializeField] private Button m_Button_Confirm = null;
	[SerializeField] private GameObject m_NicknameMask = null;
	[SerializeField] private GameObject m_Mask = null;
	[SerializeField] private GameObject m_Error_Retry = null;
	[SerializeField] private GameObject m_Error = null;
	[SerializeField] private GameObject m_AdRemoveButton = null;
	public static System.Action OnLoginSuccess;
    private void Awake()
    {
		OnLoginSuccess = delegate { };
	}
    void Start()
    {
		if(Backend.IsInitialized == false)
		{
#if UNITY_ANDROID
			PlayGamesClientConfiguration Config = new PlayGamesClientConfiguration.Builder().RequestServerAuthCode(false).RequestIdToken().Build();
			PlayGamesPlatform.InitializeInstance(Config);
			PlayGamesPlatform.DebugLogEnabled = true;
			PlayGamesPlatform.Activate();
#endif
			Backend.Initialize(OnBackendInitialized);
			AdsManager.Instance.ShowInterstitialAd();
		}
        else
        {
			m_Mask.SetActive(false);
			OnBackendInitialized();
		}
    }
	
	void OnBackendInitialized()
	{
		if (Backend.BMember.LoginWithTheBackendToken().IsSuccess())
		{
			OnLogined();
		}
		else
		{
			Login();
		}
	}
	[ContextMenu("DELETE_GUESTINFO")] public void DeleteGuestLogin() => Backend.BMember.DeleteGuestInfo();
	[ContextMenu("LOGOUT")] public void LogOut() => Backend.BMember.Logout();
	public void Login()
	{
		#if UNITY_EDITOR
		BackendReturnObject BRO = Backend.BMember.GuestLogin();
		Debug.Log(BRO);
		if (BRO.IsSuccess())
		{
			Debug.Log("Logined With Guest ID");
			OnLogined();
		}
#else
		Debug.Log("Login");
		if (Social.localUser.authenticated == true)
		{
		Debug.Log("Social.localUser.authenticated == true");
			BackendReturnObject bro = Backend.BMember.AuthorizeFederation(GetGoogleToken(), FederationType.Google);
			OnLogined();
		}
		else
		{
			Social.localUser.Authenticate((bool _Success) =>
			{
				if (_Success)
				{
					BackendReturnObject bro = Backend.BMember.AuthorizeFederation(GetGoogleToken(), FederationType.Google);
					Debug.Log("AuthorizeFederation");
					OnLogined();
				}
				else
				{
					m_Error_Retry.SetActive(true);
				}
			});
		}
#endif
	}

	string GetGoogleToken()
	{
		if(PlayGamesPlatform.Instance.localUser.authenticated)
		{
			return PlayGamesPlatform.Instance.GetIdToken();
		}
		else
		{
			m_Error_Retry.SetActive(true);
			return null;
		}
	}

	void OnLogined()
	{
		m_Mask.SetActive(false);
		BackendReturnObject bro = Backend.BMember.GetUserInfo();
		//DataManager.Instance.InDate = bro.GetReturnValuetoJSON()["row"]["inDate"].ToString(); 레플리카에서 뜯어온 코드인데 어디던간에 inDate를 저장해둬야함
		if (bro.GetReturnValuetoJSON()["row"]["nickname"] == null)
		{
			m_Input_Name.gameObject.SetActive(true);
			m_Button_Confirm.gameObject.SetActive(true);
			m_NicknameMask.SetActive(true);
		}
		else
		{
			m_NicknameMask.SetActive(false);
			OnLoginSuccess.Invoke();
			SaveSystem.Instance.LoadData(m_AdRemoveButton);
		}
	}

	public void OnNicknameEntered()
	{
		if(m_Input_Name.text.Equals(""))
		{
			DisplayError("Please enter your nickname.");
			return;
		}
		if(!Backend.BMember.CheckNicknameDuplication(m_Input_Name.text).IsSuccess())
		{
			DisplayError("The nickname you've entered is not available.");
			return;
		}

		Backend.BMember.CreateNickname(m_Input_Name.text);
		OnLoginSuccess.Invoke();
		m_Input_Name.gameObject.SetActive(false);
		m_Button_Confirm.gameObject.SetActive(false);
		m_NicknameMask.SetActive(false);
		SaveSystem.Instance.LoadData(m_AdRemoveButton);
	}

	void DisplayError(string _Message)
	{
		m_Error.SetActive(true);
		m_Error.transform.GetChild(0).GetComponent<Text>().text = _Message;
	}
}
