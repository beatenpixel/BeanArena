using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class WUI_Text : PoolObject {

    public TextMeshPro text;
    private Transform target;
    private Vector2 offset;

    public void SetTarget(string label, Transform _target, Vector2 _offset, WUI_TextStyle style) {
        target = _target;
        offset = _offset;
        text.text = label;
        text.color = style.textColor;
        text.fontSize = style.fontSize;

        MAssets.TextMeshProPreset preset = MAssets.textPresets[style.preset];

        text.font = preset.fontAsset;
        text.fontSharedMaterial = preset.material;
    }

    internal void InternalUpdate() {
        if (target != null) {
            t.position = target.position + (Vector3)offset;
        } else {
            WorldUI.inst.RemoveText(this);
        }
    }

    public override Type GetPoolObjectType() {
        return typeof(WUI_Text);
    }

}


public struct WUI_TextStyle {
    public Color textColor;
    public float fontSize;
    public TextStylePreset preset;

    public static WUI_TextStyle beanNickname;
    public static WUI_TextStyle outlined;

    public static void InitializeStyles() {
        beanNickname = new WUI_TextStyle() { fontSize = 5, textColor = Color.white, preset = TextStylePreset.Bold_Normal };
        outlined = new WUI_TextStyle() { fontSize = 5, textColor = Color.white, preset = TextStylePreset.Bold_Outlined };
    }

}

public enum TextStylePreset {
    Normal,
    Outlined,
    Bold_Normal,
    Bold_Outlined
}