using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct RankElementUI
{
    public Text rankText;
    public Text nickName;
    public Text scoreText;
    public Image tierImage;
}
public class RankElementSettings : MonoBehaviour
{
    [SerializeField] RankElementUI ElementUI;

    public RankElementSettings SetRank(int rank)
    {
        ElementUI.rankText.text = rank.ToString("N0");
        return this;
    }

    public RankElementSettings SetNickName(string text)
    {
        ElementUI.nickName.text = text;
        return this;
    }

    public RankElementSettings SetScore(int score)
    {
        ElementUI.scoreText.text = score.ToString("N0");
        return this;
    }

    public RankElementSettings SetTier(Sprite tier)
    {
        ElementUI.tierImage.sprite = tier;
        return this;
    }

    public RankElementUI Build()
    {
        return ElementUI;
    }
}
