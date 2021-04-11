using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using LitJson;

public struct RankData
{
    public int rank;
    public string nickname;
    public int score;
    public int totalCount;

    public void Parsing(JsonData jsonData)
    {
        JsonData rows = jsonData["rows"][0];
        Debug.Log(rows.ToJson().ToString());
        nickname = rows["nickname"].ToString();
        score = int.Parse(rows["score"]["N"].ToString());
        rank = int.Parse(rows["rank"]["N"].ToString());
        totalCount = int.Parse(jsonData["totalCount"].ToString());
    }
}

public struct RTRankData
{
    public string gamer_id;
    public string tier;

    public RankData rankData;

    public void SetTier()
    {
        tier = GetTier();
    }

    public string GetTier()
    {
        float rankPercent = rankToPercent(rankData.rank, rankData.totalCount);
        if (100f >= rankPercent && rankPercent >= 60f)
            return BackEndRTRank.TIER_BRONZE;
        else if (60f > rankPercent && rankPercent >= 30f)
            return BackEndRTRank.TIER_SILVER;
        else if (30f > rankPercent && rankPercent >= 10f)
            return BackEndRTRank.TIER_GOLD;
        else
            return BackEndRTRank.TIER_LEGEND;
    }

    public float rankToPercent(int rank, int totalCount)
    {
        return ((float)rank / totalCount) * 100f;
    }
    public void PrintAll()
    {
        Debug.Log($"자신의 랭크 -> gamer_id : {gamer_id}, tier : {tier}, nickname : {rankData.nickname}, score : {rankData.score}," +
               $"rank : {rankData.rank}, totalCount : {rankData.totalCount}");
    }

}

public class BackEndRTRank : MonoSingleton<BackEndRTRank>
{
    public const string TIER_BRONZE = "BRONZE";
    public const string TIER_SILVER = "SILVER";
    public const string TIER_GOLD = "GOLD";
    public const string TIER_LEGEND = "LEGEND";

    public const string MANAGED_RANK_TABLE = "userInfo";
    public const string NICKNAME_TABLE = "nickname";
    public const string TIER_TABLE = "tier";
    public const string HIGHSCORE_TABLE = "highScore";

    public const string RT_RANK_UUID = "fe36dc10-514a-11eb-84f6-0f2837e17464";

    public RTRankData MyRTRank;
    public BackendReturnObject UserInfo;
    public List<RTRankData> UserRanks = new List<RTRankData>();
    public int firstValue = 3;
    public void Awake()
    {
        Menu_Login.OnLoginSuccess += () => {
            if (Backend.IsInitialized)
            {
                UserInfo = Backend.BMember.GetUserInfo();
                BackendReturnObject BRO = Backend.GameInfo.GetPrivateContents(MANAGED_RANK_TABLE);
                if (BRO.IsSuccess())
                {
                    if (BRO.GetReturnValuetoJSON()["rows"].ToJson() == "[]")    // 테이블이 없다면
                    {
                        InsertData();       // 테이블을 추가 후 실시간 랭킹 갱신
                    }
                    BRO.Clear();
                    BRO = Backend.GameInfo.GetPrivateContents(MANAGED_RANK_TABLE);
                    MyRTRank.tier = BRO.GetReturnValuetoJSON()["rows"][0]["tier"]["S"].ToString();
                    MyRTRank.rankData = GetMyRTRank(); // 자신의 랭크를 가져옴
                    MyRTRank.SetTier();
                    //MyRTRank.PrintAll();
                    Backend.RTRank.GetRTRankByUuid(RT_RANK_UUID, firstValue,  _ => {
                        foreach (JsonData row in _.Rows())
                        {
                            RTRankData rtRankData = new RTRankData();
                            rtRankData.gamer_id = row["gamer_id"].ToString();
                            rtRankData.rankData.nickname = row["nickname"].ToString();
                            rtRankData.rankData.score = int.Parse(row["score"]["N"].ToString());
                            rtRankData.rankData.rank = int.Parse(row["rank"]["N"].ToString());
                            rtRankData.rankData.totalCount = int.Parse(_.GetReturnValuetoJSON()["totalCount"].ToString());
                            
                            rtRankData.SetTier();
                            //rtRankData.PrintAll();
                            UserRanks.Add(rtRankData);
                        }
                    });
                }
                else
                {
                    Debug.Log("자기자신 테이블 로드 실패 : " + BRO.GetErrorCode() + BRO.GetMessage());
                }
                
            }
        };
    }

    int seq = 3;
    public void CreateRankElementContents(RankSettings rankSettings)
    {
        Backend.RTRank.GetRTRankByUuid(RT_RANK_UUID, firstValue + 3, firstValue, _ => {
            foreach (JsonData row in _.Rows())
            {
                RTRankData rtRankData = new RTRankData();
                rtRankData.gamer_id = row["gamer_id"].ToString();
                rtRankData.rankData.nickname = row["nickname"].ToString();
                rtRankData.rankData.score = int.Parse(row["score"]["N"].ToString());
                rtRankData.rankData.rank = int.Parse(row["rank"]["N"].ToString());
                
                rtRankData.rankData.totalCount = int.Parse(_.GetReturnValuetoJSON()["totalCount"].ToString());
                rtRankData.SetTier();
                //rtRankData.PrintAll();

                if (rtRankData.rankData.totalCount <= seq)
                    break;
                if (UserRanks.Contains(rtRankData))
                    continue;
                UserRanks.Add(rtRankData);
                seq++;
            }

            rankSettings.InstantiateRankElementContents(firstValue, firstValue + 3);
            firstValue = firstValue + 3;
        });
        
    }

    public bool IsRankAvailable(BackendReturnObject BRO)
    {
        if (BRO.IsSuccess())
            return true;
        else
        {
            string message = BRO.GetMessage();
            switch (BRO.GetStatusCode())
            {
                case "404":
                    if (message.Contains("rank not found"))
                        Debug.Log("랭킹 Uuid가 틀린 경우");
                    else if (message.Contains("gamer not found"))
                        Debug.Log("존재하지 않는 inDate일 경우");
                    else if (message.Contains("userRank not found"))
                        Debug.Log("랭킹에 해당 inDate의 유저가 등록되어있지 않은 경우");
                    break;
                default:
                    break;
            }
        }
        return false;
    }
    public void SetRTScore(int score)
    {
        UpdateRTRank(score);
    }

    [ContextMenu("GET_RT_RANK_LIST")]
    public void OnClickGetRankingList()
    {
        BackendReturnObject BRO = Backend.RTRank.GetRTRankByUuid(RT_RANK_UUID);
        if (BRO.IsSuccess())
        {
            Debug.Log(BRO.GetReturnValuetoJSON()["totalCount"]);
        }
    }


    [ContextMenu("RT_UPDATE")]
    public RankData GetMyRTRank()
    {
        RankData rankData = new RankData();

        BackendReturnObject BRO = Backend.RTRank.GetMyRTRank(RT_RANK_UUID);
        if (BRO.IsSuccess())
        {
            rankData.Parsing(BRO.GetReturnValuetoJSON());

            Debug.Log($"자신의 랭크 -> nickname : {rankData.nickname}, score : {rankData.score}," +
                $"rank : {rankData.rank}, totalCount : {rankData.totalCount}");
            return rankData;
        }
        else
        {
            switch (BRO.GetStatusCode())
            {
                case "404":
                    if (BRO.GetMessage().Contains("rank not found"))
                        Debug.Log("랭킹 Uuid가 틀린 경우");
                    else if (BRO.GetMessage().Contains("gamer not found"))
                    {
                        Debug.Log("존재하지 않는 inDate일 경우");
                        UpdateRTRank(0);        // 랭크 업데이트
                    }
                    else if (BRO.GetMessage().Contains("userRank not found"))
                    {
                        Debug.Log("랭킹에 해당 inDate의 유저가 등록되어있지 않은 경우");
                        UpdateRTRank(0);
                    }
                    break;
                default:
                    break;
            }
        }

        return new RankData();
    }
    public void UpdateRTRank(int score)
    {
        BackendReturnObject BRO = Backend.GameInfo.UpdateRTRankTable(MANAGED_RANK_TABLE, HIGHSCORE_TABLE, score, Backend.BMember.GetUserInfo().GetInDate());
        if (BRO.IsSuccess())
        {
            Debug.Log("실시간 랭킹 갱신 & 등록 성공");
        }
        else
        {
            switch (BRO.GetStatusCode())
            {
                case "403":
                    if (BRO.GetErrorCode().Contains("ForbiddenException"))
                        Debug.Log("콘솔에서 실시간 랭킹을 활성화 하지 않고 갱신 요청을 한 경우");
                    else if (BRO.GetErrorCode().Contains("ForbiddenError"))
                        Debug.Log("퍼블릭테이블의 타인정보를 수정하고자 하였을 경우");
                    break;
                case "400":
                    if (BRO.GetErrorCode().Contains("BadRankData"))
                        Debug.Log("콘솔에서 실시간 랭킹을 생성하지 않고 갱신 요청을 한 경우");
                    if (BRO.GetErrorCode().Contains("BadRankData"))
                        Debug.Log("테이블 명 혹은 colum명이 존재하지 않는 경우");
                    break;
                case "428":
                    Debug.Log("한국시간(UTC+9) 4시 ~ 5시 사이에 실시간 랭킹 갱신 요청을 한 경우");
                    break;
                case "404":
                    Debug.Log("존재하지 않는 tableName인 경우");
                    break;
                case "412":
                    Debug.Log("비활성화 된 tableName인 경우");
                    break;
                default:
                    break;
            }
        }
    }


    
    
    [ContextMenu("RT_INSERT_DATA")]
    public void InsertData()
    {
        Param param = new Param();
        param.Add("nickname", UserInfo.GetReturnValuetoJSON()["row"]["nickname"].ToString());
        param.Add("tier", TIER_BRONZE);
        param.Add("highScore", 0);
        param.Add("userInDate", UserInfo.GetReturnValuetoJSON()["row"]["inDate"].ToString());
        BackendReturnObject BRO = Backend.GameInfo.Insert(MANAGED_RANK_TABLE, param);


        if (BRO.IsSuccess())
        {
            Debug.Log("indate : " + BRO.GetInDate());
            BRO = Backend.GameInfo.UpdateRTRankTable(MANAGED_RANK_TABLE, HIGHSCORE_TABLE, 0, BRO.GetInDate());
            if (BRO.IsSuccess())
            {
                Debug.Log("실시간 랭킹 갱신 성공");
            }
            else
            {
                switch (BRO.GetStatusCode())
                {
                    case "403":
                        if (BRO.GetErrorCode().Contains("ForbiddenException"))
                            Debug.Log("콘솔에서 실시간 랭킹을 활성화 하지 않고 갱신 요청을 한 경우");
                        else if (BRO.GetErrorCode().Contains("ForbiddenError"))
                            Debug.Log("퍼블릭테이블의 타인정보를 수정하고자 하였을 경우");
                        break;
                    case "400":
                        if (BRO.GetErrorCode().Contains("BadRankData"))
                            Debug.Log("콘솔에서 실시간 랭킹을 생성하지 않고 갱신 요청을 한 경우");
                        if (BRO.GetErrorCode().Contains("BadRankData"))
                            Debug.Log("테이블 명 혹은 colum명이 존재하지 않는 경우");
                        break;
                    case "428":
                        Debug.Log("한국시간(UTC+9) 4시 ~ 5시 사이에 실시간 랭킹 갱신 요청을 한 경우");
                        break;
                    case "404":
                        Debug.Log("존재하지 않는 tableName인 경우");
                        break;
                    case "412":
                        Debug.Log("비활성화 된 tableName인 경우");
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            switch (BRO.GetStatusCode())
            {
                case "404":
                    Debug.Log("존재하지 않는 tableName인 경우");
                    break;
                case "412":
                    Debug.Log("비활성화 된 tableName인 경우");
                    break;
                case "413":
                    Debug.Log("하나의 row(column 들의 집합)이 400KB를 넘는 경우");
                    break;
                default:
                    Debug.Log("서버 공통 에러 발생 : " + BRO.GetMessage());
                    break;
            }
        }

    }
}
