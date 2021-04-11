using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BackEnd;

public abstract class SwordLocation<T> where T : Object
{
    public T IdleHand;
    public T rightHand;
    public T leftHand01;
    public T leftHand02;
    public T leftHand03;
}

[System.Serializable]
public class SwordLocationSprites : SwordLocation<Sprite> {
    public Sprite rankShow;
}

[System.Serializable]
public class SwordLocationRenderer : SwordLocation<SpriteRenderer> { }

public class SwordManager : MonoSingleton<SwordManager>
{
    [SerializeField] uint playerSwordID = 0;
    [SerializeField] SwordLocationRenderer swordLocationRenderer;

    private List<Sword> swords;

    public uint SwordID { get => playerSwordID; set => playerSwordID = value; }
    public Sword MySword { get; private set; }

    public List<Sword> GetSwords => swords;
    public SwordLocationRenderer GetSwordLocationRenderer => swordLocationRenderer;

    [SerializeField]
    private string m_userSwordIndate = string.Empty;

    public const string MANAGED_SWORD_TABLE = "userSword";
    public const string SWORD_TABLE = "SwordID";

    public static System.Action<uint> OnSwordLoadSuccess = delegate { };

    private void Awake()
    {
        OnSwordLoadSuccess = delegate { };

        swords = Resources.LoadAll<Sword>("Data/Tables/Swords").ToList();

        if (swords == null)
            Debug.LogError("[NONE] : Swords ScriptableObject");

        Menu_Login.OnLoginSuccess += () => {
            if (string.IsNullOrEmpty(m_userSwordIndate))
                LoadSwordData();
        };

        OnSwordLoadSuccess += (SwordID) => {
            Sword sword = FindSwordOfId(SwordID);
            MySword = sword;
            SetProperty(sword);
        };
    }


    [ContextMenu("CHANGED_SWORD")]
    public void SetSword()
    {
        ChangeSword(0);
    }

    public void SetProperty(Sword sword)
    {
        Debug.Log("적용된 소드 이름 : " + sword.name);
        SwordLocationSprites sprites = sword.swordLocationSprites;
        GetSwordLocationRenderer.IdleHand.sprite = sprites.IdleHand;
        GetSwordLocationRenderer.rightHand.sprite = sprites.rightHand;
        GetSwordLocationRenderer.leftHand01.sprite = sprites.leftHand01;
        GetSwordLocationRenderer.leftHand02.sprite = sprites.leftHand02;
        GetSwordLocationRenderer.leftHand03.sprite = sprites.leftHand03;
    }
    public void ChangeSword(uint changeSwordID)
    {
        Param param = new Param();
        param.Add(SWORD_TABLE, changeSwordID);
        Backend.GameInfo.Update(MANAGED_SWORD_TABLE, m_userSwordIndate, param, (BackendReturnObject bro) => { Debug.Log(bro); });

        Sword sword = FindSwordOfId(changeSwordID);
        MySword = sword;
        SetProperty(sword);
    }
    public void LoadSwordData()
    {
        BackendReturnObject BRO = Backend.GameInfo.GetPrivateContents(MANAGED_SWORD_TABLE);
        if (BRO.IsSuccess())
        {
            if (BRO.GetReturnValuetoJSON()["rows"].ToJson() == "[]")
            {
                Param param = new Param();
                param.Add(SWORD_TABLE, 0);
                param.Add("userInDate", Backend.BMember.GetUserInfo().GetReturnValuetoJSON()["row"]["inDate"].ToString());
                Backend.GameInfo.Insert(MANAGED_SWORD_TABLE, param, (BackendReturnObject bro) => {
                    m_userSwordIndate = bro.GetReturnValuetoJSON()["inDate"].ToString();
                });
            }
            else
            {
                Debug.Log(BRO.Rows().ToJson().ToString());
                SwordID = uint.Parse(BRO.Rows()[0][SWORD_TABLE]["N"].ToString());
                m_userSwordIndate = BRO.Rows()[0]["inDate"]["S"].ToString();
            }
            OnSwordLoadSuccess.Invoke(SwordID);
        }
    }

    public Sword FindSwordOfId(uint ID)
    {
        Sword[] sword = GetSwords.Where(t => t.ID == ID).ToArray();
        if (sword.Length == 0)
        {
            Debug.Log("해당되는 소드를 찾을 수 없습니다");
            if (GetSwords.Count > 0)
            {
                Debug.Log("기본 소드로 대처합니다");
                return GetSwords[0];
            }
        }
        return sword[0];
    }
    public Sword[] FindSwordsOfId(uint ID)
    {
        Sword[] sword = GetSwords.Where(t => t.ID == ID).ToArray();
        if (sword.Length == 0)
        {
            Debug.Log("해당되는 소드 리스트를 찾을 수 없습니다");
            if (GetSwords.Count > 0)
            {
                Debug.Log("기존에 존재하는 리스트를 반환합니다");
                return GetSwords.ToArray();
            }
        }
        return sword;
    }
}


