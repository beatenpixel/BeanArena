using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BeanArena/HeroRarenessInfo")]
public class SO_HeroRarenessInfo : ScriptableObject, ITypeKey<ItemRareness> {
    public ItemRareness heroRareness;
    public int maxLevel => levelsInfo.Length;
    public HeroLevelInfo[] levelsInfo;
    public int randomDropWeight = 1;
    public Vector2Int dropAmount = new Vector2Int(1, 3);

    public ItemRareness GetKey() {
        return heroRareness;
    }
}

[System.Serializable]
public struct HeroLevelInfo {
    public int cardsToLevel;
    public int coinsToLevel;
}
