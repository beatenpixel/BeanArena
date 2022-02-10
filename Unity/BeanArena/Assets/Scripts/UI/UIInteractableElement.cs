using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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