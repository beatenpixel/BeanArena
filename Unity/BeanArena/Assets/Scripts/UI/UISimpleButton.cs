using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISimpleButton : UIInteractableElement {

    public UnityEvent ClickUnityEvent;

    public Action OnClickEvent;
    public Action<int> OnClickEventInt;
    private int onClickEventIntArg;
    public AnimatedButtonConfig config;
    public RectTransform subRectT;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Image buttonIcon;
    [SerializeField] private TextMeshProUGUI buttonText;

    private List<Graphic> childGraphics = new List<Graphic>();
    private Dictionary<Graphic, Color> startColors;

    private Vector3 startScale;
    private Color startImageColor;

    public override Type GetPoolObjectType() {
        return typeof(UISimpleButton);
    }

    public override UIElementType GetElementType() {
        return UIElementType.Button;
    }

    protected override void Awake() {
        base.Awake();

        OnClickEvent += () => {
            ClickUnityEvent?.Invoke();
        };

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

    public void SetOnClick(Action action) {
        OnClickEvent = action;
    }

    public void SetOnClick(int arg, Action<int> action) {
        onClickEventIntArg = arg;
        OnClickEventInt = action;
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

    private void OnBecomePressed() {
        subRectT.DOKill(true);
        subRectT.DOScale(startScale * config.pressScale, config.pressDuration).SetUpdate(true).SetEase(Ease.OutBack);

        TintAllGraphics(config.tintOnPress);
    }

    private void OnBecomeUnpressed() {
        subRectT.DOKill(true);
        subRectT.DOScale(startScale, config.pressDuration).SetUpdate(true).SetEase(Ease.OutBack);

        TintAllGraphics(Color.white);
    }

    #region UIEvents

    public override void OnPointerClick(PointerEventData eventData) {
        OnClickEvent?.Invoke();
        OnClickEventInt?.Invoke(onClickEventIntArg);

        MSound.Play("click");
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

}

[System.Serializable]
public class AnimatedButtonConfig {
    public float pressScale = 0.87f;
    public float pressDuration = 0.2f;
    public Ease pressEase = Ease.OutBounce;

    public Color tintOnPress;
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