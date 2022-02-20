using Coffee.UIEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class IconDrawer : PoolObject {

    public RectTransform imageRectT;
    public RectTransform textRectT;
    public Image image;
    public Image rarenessImage;
    public Image redDotImage;
    public Image imageWithEffects;
    public UIEffect imageEffects;
    public RectTransform imageWithEffectRectT;

    public GameObject fuseBarRootGO;
    public RectTransform fuseBarRectT;
    public TextMeshProUGUI levelText;

    [SerializeField] private SO_IconContent config;

    public void Show(bool show) {
        go.SetActive(show);
    }

    public void SetIcon(SO_IconContent _config) {
        config = _config;
    }

    public void EnableRedDot(bool enable) {
        redDotImage.enabled = enable;
    }

    public void DrawIcon() {
        if (config != null) {
            image.enabled = true;
            imageWithEffects.enabled = false;
            imageEffects.enabled = false;

            imageRectT.anchorMin = config.anchorMin;
            imageRectT.anchorMax = config.anchorMax;
            imageRectT.offsetMin = config.offsetMin;
            imageRectT.offsetMax = -config.offsetMax;
            imageRectT.pivot = config.pivot;

            imageRectT.rotation = Quaternion.Euler(0, 0, config.rotation);

            SetTextRect(new Vector2(0.1f, 0.1f), new Vector2(0.9f, 0.4f), Vector2.zero, Vector2.zero);

            image.sprite = config.sprite;
            image.color = config.color;
        } else {
            Debug.LogError("IconDrawer: No icon to draw");
        }
    }

    private void SetTextRect(Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax) {
        textRectT.anchorMin = anchorMin;
        textRectT.anchorMax = anchorMax;
        textRectT.offsetMin = offsetMin;
        textRectT.offsetMax = offsetMax;
    }

    public void DrawIconWithEffects() {
        if (config != null) {
            image.enabled = false;
            imageWithEffects.enabled = true;
            imageEffects.enabled = true;

            imageEffects.effectMode = EffectMode.Grayscale;

            imageWithEffectRectT.anchorMin = config.anchorMin;
            imageWithEffectRectT.anchorMax = config.anchorMax;
            imageWithEffectRectT.offsetMin = config.offsetMin;
            imageWithEffectRectT.offsetMax = -config.offsetMax;
            imageWithEffectRectT.pivot = config.pivot;

            imageWithEffectRectT.rotation = Quaternion.Euler(0, 0, config.rotation);

            imageWithEffects.sprite = config.sprite;
            imageWithEffects.color = Color.Lerp(config.color, Color.black, 0.8f);
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

    public void DrawText(string levelStr, Color? color = null) {
        levelText.text = levelStr;
        levelText.color = color ?? Color.white;
    }

    public void DrawItemMerged(GD_Item item, MergedItemInfo info) {
        rarenessImage.enabled = true;
        rarenessImage.color = MAssets.colors[("rareness_" + item.rareness.ToString()).ToLower()].SetA(0.5f);

        SetIcon(item.info.icon);
        DrawIcon();

        DrawBar(info.progressBar);
        EnableRedDot(false);

        DrawText(MFormat.GetLVLString(info.newLevel, item.info.maxLevel), info.levelChanged ? MAssets.colors["STAT_MAGENTA"] : null);
    }

    public void DrawItem(GD_Item itemData, SO_ItemInfo itemInfo) {
        bool hasFuse = itemInfo.GetFusePointsPercent(itemData.fusePoints, itemData.levelID, out float fuseProgress);

        rarenessImage.enabled = true;
        rarenessImage.color = MAssets.colors[("rareness_" + itemData.rareness.ToString()).ToLower()].SetA(0.5f);

        SetIcon(itemInfo.icon);
        DrawIcon();

        if (itemData.isNew) {
            EnableRedDot(true);
        } else {
            EnableRedDot(false);
        }

        if (hasFuse) {
            DrawBar(fuseProgress);
        } else {
            DrawBar(null);
        }

        DrawText(MFormat.GetLVLString(itemData.levelID, itemInfo.maxLevel));
    }

    public void DrawItemPanelCircle(GD_Item item, PanelCircleItem panelCircle) {
        bool hasFuse = item.info.GetFusePointsPercent(item.fusePoints, item.levelID, out float fuseProgress);

        rarenessImage.enabled = false;
        //rarenessImage.color = MAssets.colors[("rareness_" + item.rareness.ToString()).ToLower()].SetA(0.5f);

        SetIcon(item.info.icon);
        DrawIcon();

        EnableRedDot(false);
        DrawBar(null);
        DrawText("");
    }

    public void DrawHeroDrop(HeroType heroType, int amount) {
        GD_HeroItem item = Game.data.inventory.heroes.Find(x => x.heroType == heroType);

        rarenessImage.enabled = true;
        rarenessImage.color = MAssets.colors[("rareness_" + item.info.rarenessInfo.heroRareness.ToString()).ToLower()].SetA(0.5f);

        SetIcon(item.info.icon);
        DrawIcon();
        SetTextRect(new Vector2(0.1f, 0.7f), new Vector2(0.9f, 1f), Vector2.zero, Vector2.zero);
        DrawText("x" + amount.ToString());
        
        EnableRedDot(false);

        DrawBar(null);
    }

    public void DrawHero(GD_HeroItem item) {
        rarenessImage.enabled = true;
        rarenessImage.color = MAssets.colors[("rareness_" + item.info.rarenessInfo.heroRareness.ToString()).ToLower()].SetA(0.5f);

        SetIcon(item.info.icon);
        if (item.isUnlocked) {
            DrawIcon();
            DrawText(MFormat.GetLVLString(item.levelID, item.info.rarenessInfo.maxLevel));
        } else {
            DrawIconWithEffects();
            DrawText("");
        }
        SetTextRect(new Vector2(0.1f, 0.7f), new Vector2(0.9f, 1f), Vector2.zero, Vector2.zero);
        EnableRedDot(false);

        DrawBar(null);        
    }

    public void DrawChest(GD_Chest chestData) {
        rarenessImage.enabled = true;
        rarenessImage.color = Color.white.SetA(0f);

        SetIcon(chestData.info.icon);
        DrawIcon();
        EnableRedDot(false);

        DrawBar(null);
        DrawText("");
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