using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIPointerEventListener : MonoBehaviour,
    IPointerDownHandler, IPointerEnterHandler, IPointerClickHandler,
    IPointerMoveHandler, IPointerExitHandler, IPointerUpHandler {

    public event Action<UIPointerEvent> OnEvent;

    private void Update() {
        
    }

    public void ClearEvent() {
        OnEvent = null;
    }

    public void OnPointerClick(PointerEventData eventData) {
        OnEvent?.Invoke(new UIPointerEvent() {
            type = PointerEventType.Click,
            data = eventData
        });
    }

    public void OnPointerDown(PointerEventData eventData) {
        OnEvent?.Invoke(new UIPointerEvent() {
            type = PointerEventType.Down,
            data = eventData
        });
    }

    public void OnPointerEnter(PointerEventData eventData) {
        OnEvent?.Invoke(new UIPointerEvent() {
            type = PointerEventType.Enter,
            data = eventData
        });
    }

    public void OnPointerExit(PointerEventData eventData) {
        OnEvent?.Invoke(new UIPointerEvent() {
            type = PointerEventType.Exit,
            data = eventData
        });
    }

    public void OnPointerMove(PointerEventData eventData) {
        OnEvent?.Invoke(new UIPointerEvent() {
            type = PointerEventType.Move,
            data = eventData
        });
    }

    public void OnPointerUp(PointerEventData eventData) {
        OnEvent?.Invoke(new UIPointerEvent() {
            type = PointerEventType.Up,
            data = eventData
        });
    }

}

public struct UIPointerEvent {
    public PointerEventType type;
    public PointerEventData data;
}

public enum PointerEventType {
    None,
    Down,
    Enter,
    Click,
    Move,
    Exit,
    Up
}
