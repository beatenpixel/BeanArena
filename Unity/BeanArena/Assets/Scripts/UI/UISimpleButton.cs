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
    public Action<int> OnClickEventInt;
    private int onClickEventIntArg;
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

    public void SetOnClick(int arg, Action<int> action) {
        onClickEventIntArg = arg;
        OnClickEventInt = action;
    }

    protected override void OnBecomePressed() {
        base.OnBecomePressed();

        subRectT.DOKill(true);
        subRectT.DOScale(startScale * config.pressScale, config.pressDuration).SetUpdate(true).SetEase(Ease.OutBack);
    }

    protected override void OnBecomeUnpressed() {
        base.OnBecomeUnpressed();

        subRectT.DOKill(true);
        subRectT.DOScale(startScale, config.pressDuration).SetUpdate(true).SetEase(Ease.OutBack);
    }

    protected override void OnClick() {
        base.OnClick();

        OnClickEvent?.Invoke();
        OnClickEventInt?.Invoke(onClickEventIntArg);

        MSound.Play("click");
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
    public Color tintOnPress = new Color(1,1,1,1);
}

[System.Serializable]
public class AnimatedButtonConfig : UIButtonConfig {
    public float pressScale = 0.87f;
    public float pressDuration = 0.2f;
    public Ease pressEase = Ease.OutBounce;
}

public interface IUIInteractableElement : IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

}

public abstract class UIInteractableElement : UIElement, IUIInteractableElement {

    protected bool isHovered;
    protected bool isPressed;

    public virtual void OnPointerClick(PointerEventData eventData) {
        isHovered = false;
        isPressed = false;
    }

    public virtual void OnPointerDown(PointerEventData eventData) {
        isHovered = true;
        isPressed = true;
    }

    public virtual void OnPointerEnter(PointerEventData eventData) {
        isHovered = true;
    }

    public virtual void OnPointerExit(PointerEventData eventData) {
        isHovered = false;
    }

    public virtual void OnPointerUp(PointerEventData eventData) {
        isHovered = false;
        isPressed = false;
    }

}

public abstract class UIElement : PoolObject {

    public abstract UIElementType GetElementType();

    public RectTransform rectT;
    [HideInInspector] public bool elementEnabled;

    protected override void Awake() {
        base.Awake();

        if (go == null) {
            go = gameObject;
        }
        elementEnabled = go.activeSelf;
    }

    public virtual void Enable(bool enable) {
        elementEnabled = enable;
        go.SetActive(enable);
    }

}