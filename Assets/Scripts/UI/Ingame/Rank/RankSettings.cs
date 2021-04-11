using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

[System.Serializable]
public class PlayerInfoUI
{
    public Image objectImage;
    public Text nickName;
    public Image tierImage;
    public Text highScoreText;

    public PlayerInfoUI SetObject(Sprite sprite)
    {
        objectImage.sprite = sprite;
        return this;
    }
    public PlayerInfoUI SetNickName(string nickName)
    {
        this.nickName.text = nickName;
        return this;
    }
    public PlayerInfoUI SetTier(Sprite sprite)
    {
        tierImage.sprite = sprite;
        return this;
    }
    public PlayerInfoUI SetScore(int score)
    {
        highScoreText.text = score.ToString("N0");
        return this;
    }
}

[System.Serializable]
public struct TierUI
{
    public Sprite BRONZE;
    public Sprite SILLVER;
    public Sprite GOLD;
    public Sprite LEGEND;

    public Sprite GetStringToSprite(string tier)
    {
        switch (tier)
        {
            case BackEndRTRank.TIER_BRONZE:
                return BRONZE;
            case BackEndRTRank.TIER_SILVER:
                return SILLVER;
            case BackEndRTRank.TIER_GOLD:
                return GOLD;
            case BackEndRTRank.TIER_LEGEND:
                return LEGEND;
            default:
                Debug.Log("잘못된 티어 정보입니다 : " + tier);
                return null;
        }
    }
}

public class RankSettings : MonoBehaviour
{
    [SerializeField] PlayerInfoUI playerInfoUI;
    [Space(20)]
    [SerializeField] TierUI tierUI;
    [Space(20)]
    private BackEndRTRank backEndRTRank;
    private SwordManager swordManager;

    [SerializeField] Scrollbar scrollbar;

    [SerializeField] RankElementSettings rankElementPrefabs;
    [SerializeField] Transform rankContentsTranform;
    private void OnEnable()
    {
		if (Backend.IsInitialized)
        {
			SoundManager.Instance.PlaySound("SFX_Button8");
            backEndRTRank = BackEndRTRank.Instance;
            swordManager = SwordManager.Instance;
            RTRankData myRank = backEndRTRank.MyRTRank;
            playerInfoUI.SetNickName(myRank.rankData.nickname)
                .SetScore(myRank.rankData.score)
                .SetObject(swordManager.MySword.swordLocationSprites.rankShow)
                .SetTier(tierUI.GetStringToSprite(myRank.tier));
            foreach (var user in backEndRTRank.UserRanks)
            {
                var obj = Instantiate(rankElementPrefabs, rankContentsTranform);
                RankElementSettings element = obj;
                element.SetRank(user.rankData.rank)
                    .SetScore(user.rankData.score)
                    .SetNickName(user.rankData.nickname)
                    .SetTier(tierUI.GetStringToSprite(user.tier));
            }
            StartCoroutine(DetectVerticalScrollSize());
        }
    }
    private void OnDisable()
    {
        foreach(Transform obj in rankContentsTranform)
        {
            Destroy(obj.gameObject);
        }
		SoundManager.Instance.PlaySound("SFX_Button8");
	}
	public void InstantiateRankElementContents(int limit, int offset)
    {
        for (int i = limit; i < offset; i++)
        {
            if (backEndRTRank.UserRanks.Count < i + 1)
            {
                continue;
            }

            var user = backEndRTRank.UserRanks[i];

            var obj = Instantiate(rankElementPrefabs, rankContentsTranform);
            RankElementSettings element = obj;
            element.SetRank(user.rankData.rank)
                .SetScore(user.rankData.score)
                .SetNickName(user.rankData.nickname)
                .SetTier(tierUI.GetStringToSprite(user.tier));
        }
    }
    float startSize = 0.9f;
    IEnumerator DetectVerticalScrollSize()
    {
        while (gameObject.activeInHierarchy)
        {
            if (scrollbar.size <= startSize)
            {
                backEndRTRank.CreateRankElementContents(this);
                startSize /= 1.5f;
                yield return new WaitForSeconds(2.0f);
            }
            yield return null;
        }
    }
}
