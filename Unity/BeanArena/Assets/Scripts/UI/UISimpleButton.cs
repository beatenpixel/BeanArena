using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISimpleButton : UIButtonBase {

    public UnityEvent ClickUnityEvent;

    public Action OnClickEvent;
    public Action<object> OnClickEventWithArg;
    private object onClickEventArg;
    public AnimatedButtonConfig config;

    public override Type GetPoolObjectType() {
        return typeof(UISimpleButton);
    }

    public override UIButtonConfig baseConfig => config;

    protected override void Awake() {
        base.Awake();

        OnClickEvent += () => {
            ClickUnityEvent?.Invoke();
        };
    }

    public void SetOnClick(Action action) {
        OnClickEvent = action;
    }

    public void SetOnClick(object arg, Action<object> action) {
        onClickEventArg = arg;
        OnClickEventWithArg = action;
    }

    protected override void OnBecomePressed(PointerEventData eventData) {
        base.OnBecomePressed(eventData);

        if (config.doScale) {
            subRectT.DOKill(true);
            subRectT.DOScale(startScale * config.pressScale, config.pressDuration).SetUpdate(true).SetEase(Ease.OutBack);
        }
    }

    protected override void OnBecomeUnpressed(PointerEventData eventData) {
        base.OnBecomeUnpressed(eventData);

        if (config.doScale) {
            subRectT.DOKill(true);
            subRectT.DOScale(startScale, config.pressDuration).SetUpdate(true).SetEase(Ease.OutBack);
        }
    }

    protected override void OnClick(PointerEventData eventData) {
        base.OnClick(eventData);

        OnClickEvent?.Invoke();
        OnClickEventWithArg?.Invoke(onClickEventArg);

        MSound.Play("click", SoundConfig.randVolumePitch01);
    }

}

[System.Serializable]
public class UIGraphicsElement {
    public Image image;
    public TextMeshProUGUI text;
    [HideInInspector] public Color startColor;

    public void GetColorFromComponents() {
        startColor = GetColor();
    }

    public void SetColor(Color _color) {
        if (image != null) {
            image.color = _color;
        }

        if (text != null) {
            text.color = _color;
        }
    }

    public Color GetColor() {
        if (image != null) {
            return image.color;
        }

        if (text != null) {
            return text.color;
        }

        return new Color(1, 0, 1, 1);
    }

}

[System.Serializable]
public class UIButtonConfig {
    public bool doTint = true;
    public Color tintOnPress = new Color(1, 1, 1, 1);
}

[System.Serializable]
public class AnimatedButtonConfig : UIButtonConfig {
    public bool doScale = true;
    public float pressScale = 0.87f;
    public float pressDuration = 0.2f;
    public Ease pressEase = Ease.OutBounce;
}