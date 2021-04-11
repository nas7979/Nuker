using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using UnityEngine.UI;
using BackEnd;

public class SaveSystem : MonoSingleton<SaveSystem>
{
	[System.Serializable]
	public class SaveData
	{
		public bool IsAdRemoved = false;
		public int HighScore = 0;
	}

	private SaveData m_Data = null;
	public SaveData Data { get => m_Data; }
	private string m_AdRemoveTableInDate;

	public void LoadData(GameObject _AdRemoveButton)
	{
		if (m_Data != null)
			return;
		m_Data = new SaveData();
		DontDestroyOnLoad(gameObject);
		Menu_Buttons.OnClickStartButton += () => {
			m_Data.HighScore = BackEndRTRank.Instance.MyRTRank.rankData.score;
		};
		BackendReturnObject Getbro = Backend.GameInfo.GetPrivateContents("adRemove");
		if (Getbro.IsSuccess())
		{
			if (Getbro.GetReturnValuetoJSON()["rows"].ToJson() == "[]")
			{
				m_Data.IsAdRemoved = false;
				Param param = new Param();
				param.Add("isAdRemoved", false);
				param.Add("userInDate", Backend.BMember.GetUserInfo().GetReturnValuetoJSON()["row"]["inDate"].ToString());
				Backend.GameInfo.Insert("adRemove", param, (BackendReturnObject bro) => { m_AdRemoveTableInDate = bro.GetReturnValuetoJSON()["inDate"].ToString(); });
			}
			else
			{
				m_Data.IsAdRemoved = bool.Parse(Getbro.Rows()[0]["isAdRemoved"]["BOOL"].ToString());
				if(m_Data.IsAdRemoved == true)
				{
					_AdRemoveButton.SetActive(false);
				}
				m_AdRemoveTableInDate = Getbro.Rows()[0]["inDate"]["S"].ToString();
			}
		}
	}

	public void BuyAdRemove()
	{
		Param param = new Param();
		param.Add("isAdRemoved", true);
		m_Data.IsAdRemoved = true;
		Backend.GameInfo.Update("adRemove", m_AdRemoveTableInDate, param);
	}
}
