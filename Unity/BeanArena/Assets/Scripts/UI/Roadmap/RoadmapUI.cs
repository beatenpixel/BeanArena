using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadmapUI : MonoBehaviour {

    public UICanvas canvas;

    public UISimpleButton claimRewardButton;

    public RectTransform segementsHolderRectT;
    public RectTransform progressLineRectT;
    public RectTransform progressLineBGRectT;

    public RoadmapRewardDrawer previewDrawer;
    public NotificationDot calimableRewardsDot;

    private RoadmapContainer container;
    private bool generated;

    private List<RoadmapLineSegment> lineSegments;
    private List<RoadmapRewardDrawer> rewardDrawers;

    private List<RoadmapReward> claimableRewards;

    public void Init() {
        container = GameBalance.GenerateRoadmapContainer();
        
        Generate();
    }

    public void Show(bool show) {
        claimableRewards = container.GetClaimableRewards(Game.data.player.mmr);

        canvas.Show(show);
        Draw();
        DrawPreview();
    }

    public void DrawPreview() {
        RoadmapReward nextReward = container.GetNextReward(Game.data.player.mmr);

        if (nextReward != null) {
            previewDrawer.DrawReward(nextReward);
        }

        if(claimableRewards != null && claimableRewards.Count > 0) {
            calimableRewardsDot.Enable(true, claimableRewards.Count);
        } else {
            calimableRewardsDot.Enable(false);
        }
    }

    public void Draw() {
        int playerMMR = Game.data.player.mmr;

        progressLineRectT.sizeDelta = progressLineRectT.sizeDelta.SetX(MMRToSizeDelta(playerMMR));

        if (claimableRewards.Count > 0) {
            claimRewardButton.Enable(true);
            RoadmapRewardDrawer drawer = rewardDrawers.Find(x => x.currentReward == claimableRewards[0]);
            claimRewardButton.rectT.position = drawer.rectT.position;
        } else {
            claimRewardButton.Enable(false);
        }
    }

    private void Generate() {
        if (generated) return;

        lineSegments = new List<RoadmapLineSegment>();
        rewardDrawers = new List<RoadmapRewardDrawer>();

        int lastMmr = 0;

        Vector3[] worldCorners = new Vector3[4];

        for (int i = 0; i < container.rewardsCount; i++) {
            RoadmapReward reward = container.GetReward(i);

            RoadmapLineSegment segment = MPool.Get<RoadmapLineSegment>(null, segementsHolderRectT);
            lineSegments.Add(segment);

            segment.rectT.anchoredPosition = new Vector2(MMRToSizeDelta(lastMmr), 0);
            segment.rectT.sizeDelta = new Vector2(MMRToSizeDelta(reward.mmrGlobalPosition - lastMmr), progressLineRectT.sizeDelta.y);

            RoadmapRewardDrawer drawer = MPool.Get<RoadmapRewardDrawer>(null,segementsHolderRectT);
            rewardDrawers.Add(drawer);

            drawer.DrawReward(reward);

            segment.rectT.GetWorldCorners(worldCorners);
            drawer.rectT.position = worldCorners[2];

            lastMmr = reward.mmrGlobalPosition;            
        }

        progressLineBGRectT.sizeDelta = new Vector2(MMRToSizeDelta(lastMmr), progressLineBGRectT.sizeDelta.y);
    }

    public static float MMRToSizeDelta(int mmr) {
        return mmr * 12;
    }

}

public class RoadmapContainer {

    private List<RoadmapReward> rewards = new List<RoadmapReward>();

    public int rewardsCount => rewards.Count;
    private int maxMMR;

    private System.Random rand;

    public RoadmapContainer() {
        maxMMR = 0;
        rand = new System.Random(GameBalance.REWARDS_SEED);
    }

    public void AddReward(RoadmapReward reward) {
        rewards.Add(reward);
        maxMMR += reward.mmrPrice;
        reward.mmrGlobalPosition = maxMMR;
        reward.rewardUID = rand.Next(int.MaxValue / 2);
    }

    public RoadmapReward GetNextReward(int currentMMR) {
        for (int i = 0; i < rewards.Count; i++) {
            if(currentMMR < rewards[i].mmrGlobalPosition) {
                return rewards[i];
            }
        }

        return rewards[rewards.Count - 1];
    }

    public List<RoadmapReward> GetClaimableRewards(int mmr) {
        List<RoadmapReward> result = new List<RoadmapReward>();

        for (int i = 0; i < rewards.Count; i++) {
            var rewardData = Game.data.inventory.roadMapClaimedRewards.Find(x => x.rewardUID == rewards[i].rewardUID);

            if (rewards[i].mmrGlobalPosition <= mmr) {
                if(rewardData == null) {
                    Game.data.inventory.roadMapClaimedRewards.Add(new GD_RoadmapReward() {
                        rewardUID = rewards[i].rewardUID,
                        isClaimed = false
                    });
                }

                result.Add(rewards[i]);
            } else {
                if(rewardData != null) {
                    if(!rewardData.isClaimed) {
                        result.Add(rewards[i]);
                    }
                }
            }
        }

        return result;
    }

    public void Claim(RoadmapReward reward) {
        
    }    

    public RoadmapReward GetReward(int ind) {
        return rewards[ind];
    }

}

public class RoadmapReward {
    public int rewardUID;
    public int mmrPrice;
    public int mmrGlobalPosition;
    public RewardType rewardType;

    public GD_Chest chestData { get; private set; }
    public HeroCardsContainer heroData { get; private set; }
    public GD_Item itemData { get; private set; }
    public int? gems { get; private set; }
    public int? coins { get; private set; }

    public RoadmapReward(GD_Chest chest) {
        chestData = chest;
        rewardType = RewardType.Chest;
    }

    public RoadmapReward(HeroCardsContainer hero) {
        heroData = hero;
        rewardType = RewardType.Hero;
    }

    public RoadmapReward(GD_Item item) {
        itemData = item;
        rewardType = RewardType.Item;
    }

    public RoadmapReward(CurrencyType currency, int amount) {
        if(currency == CurrencyType.Coin) {
            rewardType = RewardType.Coins;
            coins = amount;
        } else if(currency == CurrencyType.Gem) {
            rewardType = RewardType.Gems;
            gems = amount;
        }
    }

    public RoadmapReward MMR(int _mmr) {
        mmrPrice = _mmr;
        return this;
    }
}

public enum RewardType {
    Gems,
    Coins,
    Chest,
    Hero,
    Item
}
