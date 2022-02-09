using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class IconDrawer : MonoBehaviour {

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

    public void DrawLevel(string levelStr) {
        levelText.text = levelStr;
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

    public void DrawChest(GD_Chest chestData, SO_ChestInfo chestInfo) {

        rarenessImage.color = Color.white.SetA(0f);

        SetIcon(chestInfo.icon);
        DrawIcon();

        DrawBar(null);
        DrawLevel("");
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
