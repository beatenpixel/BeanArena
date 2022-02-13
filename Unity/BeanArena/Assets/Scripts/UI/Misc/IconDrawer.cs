using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class IconDrawer : PoolObject {

	public RectTransform rectT;
	public Image image;
    public Image rarenessImage;

    public GameObject fuseBarRootGO;
    public RectTransform fuseBarRectT;
    public TextMeshProUGUI levelText;

	[SerializeField] private SO_IconContent config;

    public void SetIcon(SO_IconContent _config ) {
        config = _config;
    }

    public void DrawIcon() {
        if (config != null) {
            rectT.anchorMin = config.anchorMin;
            rectT.anchorMax = config.anchorMax;
            rectT.offsetMin = config.offsetMin;
            rectT.offsetMax = -config.offsetMax;
            rectT.pivot = config.pivot;

            image.sprite = config.sprite;
            image.color = config.color;
        } else {
            Debug.LogError("IconDrawer: No icon to draw");
        }
    }

    public void DrawBar(float? fuseBarP = null) {
        if(fuseBarP == null) {
            fuseBarRootGO.SetActive(false);
        } else {
            fuseBarRootGO.SetActive(true);
            fuseBarRectT.anchorMax = new Vector2(Mathf.Clamp01((float)fuseBarP), 1f);
            fuseBarRectT.anchorMin = new Vector2(0, 0);
            fuseBarRectT.offsetMax = Vector2.zero;
            fuseBarRectT.offsetMin = Vector2.zero;
        }                
    }

    public void DrawLevel(string levelStr, Color? color = null) {
        levelText.text = levelStr;
        levelText.color = color ?? Color.white;
    }

    public void DrawItemMerged(GD_Item item, MergedItemInfo info) {        
        rarenessImage.color = MAssets.colors[("rareness_" + item.rareness.ToString()).ToLower()].SetA(0.5f);

        SetIcon(item.info.icon);
        DrawIcon();

        DrawBar(info.progressBar);

        DrawLevel(MFormat.GetLVLString(info.newLevel, item.info.maxLevel), info.levelChanged ? MAssets.colors["STAT_MAGENTA"] : null);
    }

    public void DrawItem(GD_Item itemData, SO_ItemInfo itemInfo) {
        bool hasFuse = itemInfo.GetFusePointsPercent(itemData.fusePoints, itemData.levelID, out float fuseProgress);

        rarenessImage.color = MAssets.colors[("rareness_" + itemData.rareness.ToString()).ToLower()].SetA(0.5f);

        SetIcon(itemInfo.icon);
        DrawIcon();

        if (hasFuse) {
            DrawBar(fuseProgress);
        } else {
            DrawBar(null);
        }

        DrawLevel(MFormat.GetLVLString(itemData.levelID, itemInfo.maxLevel));
    }

    public void DrawHero(GD_HeroItem item) {
        rarenessImage.color = MAssets.colors[("rareness_" + item.info.heroRareness.ToString()).ToLower()].SetA(0.5f);

        SetIcon(item.info.icon);
        DrawIcon();

        DrawBar(null);
        DrawLevel(MFormat.GetLVLString(item.levelID, item.info.maxLevelsCount));
    }

    public void DrawChest(GD_Chest chestData, SO_ChestInfo chestInfo) {
        rarenessImage.color = Color.white.SetA(0f);

        SetIcon(chestInfo.icon);
        DrawIcon();

        DrawBar(null);
        DrawLevel("");
    }

    public override Type GetPoolObjectType() {
        return typeof(IconDrawer);
    }

#if UNITY_EDITOR
    private void Update() {
        if(!Application.isPlaying) {
            if (config != null) {
                if (transform.hasChanged) {
                    DrawIcon();
                }
            }
        }
    }
#endif

}

public struct MergedItemInfo {
    public int prevLevel;
    public int newLevel;
    public bool levelChanged;
    public float progressBar;
}