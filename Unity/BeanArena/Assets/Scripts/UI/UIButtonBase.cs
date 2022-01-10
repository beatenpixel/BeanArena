using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UIButtonBase : UIInteractableElement {

    public abstract UIButtonConfig baseConfig { get; }

    public RectTransform subRectT;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Image buttonIcon;
    [SerializeField] private TextMeshProUGUI buttonText;

    protected List<Graphic> childGraphics = new List<Graphic>();
    protected Dictionary<Graphic, Color> startColors;

    protected Vector3 startScale;
    protected Color startImageColor;

    public override Type GetPoolObjectType() {
        return typeof(UIButtonBase);
    }

    public override UIElementType GetElementType() {
        return UIElementType.Button;
    }

    protected override void Awake() {
        base.Awake();

        startScale = subRectT.localScale;

        childGraphics = new List<Graphic>(GetComponentsInChildren<Graphic>(true));
        startColors = new Dictionary<Graphic, Color>();
        foreach (var g in childGraphics) {
            startColors.Add(g, g.color);
            if (g.gameObject != gameObject) {
                g.raycastTarget = false;
            }
        }

        startImageColor = buttonImage.color;
    }

    public void TintAllGraphics(Color tint) {
        foreach (var g in childGraphics) {
            g.color = startColors[g] * tint;
        }
    }

    public void SetBackgroundColor(Color color) {
        startColors[buttonImage] = color;
        buttonImage.color = color;
    }

    public void SetIcon(Sprite sprite, Color color) {
        buttonIcon.sprite = sprite;
        buttonIcon.color = color;
        startColors[buttonIcon] = color;
    }

    public void SetText(string text) {
        buttonText.text = text;
    }

    protected virtual void OnBecomePressed() {
        if (baseConfig.doTint) {
            TintAllGraphics(baseConfig.tintOnPress);
        }
    }

    protected virtual void OnBecomeUnpressed() {
        if (baseConfig.doTint) {
            TintAllGraphics(Color.white);
        }
    }

    protected virtual void OnClick() {

    }

    #region UIEvents

    public override void OnPointerClick(PointerEventData eventData) {
        OnClick();
    }

    public override void OnPointerDown(PointerEventData eventData) {
        base.OnPointerDown(eventData);

        OnBecomePressed();
    }

    public override void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);

        OnBecomeUnpressed();
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData) {
        base.OnPointerUp(eventData);

        OnBecomeUnpressed();
    }

    #endregion

}