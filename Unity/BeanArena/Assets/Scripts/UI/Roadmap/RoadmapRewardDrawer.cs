using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoadmapRewardDrawer : PoolObject {

    public RectTransform rectT;
    public IconDrawer iconDrawer;
    public TextMeshProUGUI currencyRewardText;
    public TextMeshProUGUI cupsText;

    public RoadmapReward currentReward { get; private set; }

    private Vector2 startSizeDelta;

    override protected void Awake() {
        startSizeDelta = rectT.sizeDelta;    
    }

    public void DrawReward(RoadmapReward reward) {
        currentReward = reward;

        cupsText.text = MFormat.GetTMProIcon(TMProIcon.Cup) + reward.mmrGlobalPosition;

        if (reward.rewardType == RewardType.Coins || reward.rewardType == RewardType.Gems) {
            rectT.sizeDelta = startSizeDelta.SetY(80).SetX(startSizeDelta.x * 0.7f);
            iconDrawer.Show(false);

            if(reward.rewardType == RewardType.Coins) {
                currencyRewardText.text = MFormat.CurrencyToStr(CurrencyType.Coin, (int)reward.coins);
            } else if(reward.rewardType == RewardType.Gems) {
                currencyRewardText.text = MFormat.CurrencyToStr(CurrencyType.Gem, (int)reward.gems);
            }
        } else {
            rectT.localScale = Vector3.one * 1f;
            rectT.sizeDelta = startSizeDelta;
            currencyRewardText.text = "";

            iconDrawer.DrawReward(reward);
            iconDrawer.Show(true);
        }
    }

    public void DrawRewardMenu(RoadmapReward reward) {
        currentReward = reward;

        cupsText.text = "";

        if (reward.rewardType == RewardType.Coins || reward.rewardType == RewardType.Gems) {            
            iconDrawer.Show(false);

            if (reward.rewardType == RewardType.Coins) {
                currencyRewardText.text = MFormat.CurrencyToStr(CurrencyType.Coin, (int)reward.coins);
            } else if (reward.rewardType == RewardType.Gems) {
                currencyRewardText.text = MFormat.CurrencyToStr(CurrencyType.Gem, (int)reward.gems);
            }
        } else {
            currencyRewardText.text = "";
            iconDrawer.DrawReward(reward);
            iconDrawer.Show(true);
        }
    }

    public override Type GetPoolObjectType() {
        return typeof(RoadmapRewardDrawer);
    }

}
