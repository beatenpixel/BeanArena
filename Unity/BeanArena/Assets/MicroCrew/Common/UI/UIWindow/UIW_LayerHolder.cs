using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIW_LayerHolder : MonoBehaviour {

    public RectTransform rectT;
    public Image backgroundImage;

    private Color backgroundImageStartColor;
    private bool backgroundImageEnabled;
    private bool backgroundImageChangingState;

    public void Init() {
        backgroundImageStartColor = backgroundImage.color;
    }

    public void EnableBackgroundImage(bool enable, bool fadeColor) {
        backgroundImage.enabled = enable;
        backgroundImage.DOKill(false);

        if(fadeColor) {
            backgroundImage.DOColor(enable ? backgroundImageStartColor : backgroundImageStartColor.SetA(0f), 0.3f).SetUpdate(true);
        } else {
            backgroundImage.color = backgroundImageStartColor.SetA(0f);
        }
    }

}
