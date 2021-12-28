using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class JoystickInputUI : MonoBehaviour, IPointerDownHandler {

    public bool joystickEnabled = true;
    public JoystickRange rangeType;
    public float joystickRangeValue = 0.2f;
    public float joystickDrift = 0.2f;
    public float cancelRange = 0.2f;

    public UIJoystickEvent OnEvent;

    public RectTransform discRectT;
    public RectTransform dotRectT;

    private bool isTouchBased => MInput.inputType == MInput.InputType.Mobile;

    private Touch? trackedTouch = null;
    private bool isPressed;
    private Vector2 lastMousePos;

    private Vector2 joystickPos;
    private Vector2 dotPos;

    public void OnJoystickInput(UIJoystickEventData data) {
        Debug.Log($"{data.type} {data.value}");
    }

    private void Update() {

        if (isPressed) {
            bool released = false;
            Vector2 screenDotPos = Vector2.zero;

            if (isTouchBased) {
                for (int i = 0; i < Input.touchCount; i++) {
                    Touch touch = Input.touches[i];

                    if(touch.fingerId == trackedTouch.Value.fingerId) {
                        if (touch.phase == TouchPhase.Ended) {
                            screenDotPos = touch.position;
                            released = true;
                            break;
                        } else {
                            screenDotPos = touch.position;
                        }
                    }
                }
            } else {
                if(Input.GetMouseButton(0)) {
                    screenDotPos = Input.mousePosition;
                }

                if(Input.GetMouseButtonUp(0)) {
                    screenDotPos = lastMousePos;
                    released = true;
                }
            }

            dotPos = GameUI.canvas.WorldToCanvasPos(screenDotPos);
            (Vector2 fixedPos, Vector2 normalizedValue) = NormalizeDotPos(dotPos);

            if (released) {
                isPressed = false;
                Show(false);

                OnEvent?.Invoke(new UIJoystickEventData(UIJoystickEventData.EventType.End, normalizedValue));
            } else {
                discRectT.anchoredPosition = Vector2.Lerp(joystickPos, dotPos, normalizedValue.magnitude * joystickDrift);
                dotRectT.anchoredPosition = dotPos;

                OnEvent?.Invoke(new UIJoystickEventData(UIJoystickEventData.EventType.Update, normalizedValue));
            }
        }

        lastMousePos = Input.mousePosition;
    }

    private (Vector2 newPos, Vector2 normalizedValue) NormalizeDotPos(Vector2 pointerPos) {
        float maxDst = 0;
        if (rangeType == JoystickRange.ScreenWidthPercent) {
            maxDst = GameUI.canvas.realCanvasSize.x * joystickRangeValue;
        } else if (rangeType == JoystickRange.ScreenHeightPercent) {
            maxDst = GameUI.canvas.realCanvasSize.y * joystickRangeValue;
        } else if (rangeType == JoystickRange.JoystickRectWidthPercent) {
            maxDst = discRectT.rect.width * joystickRangeValue;
        }

        Vector2 dd = dotPos - joystickPos;
        if (dd.magnitude > maxDst) {
            dotPos = joystickPos + dd.normalized * maxDst;
            dd = dotPos - joystickPos;
        }

        return (dotPos, dd.normalized * (dd.magnitude / maxDst));
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (isPressed || !joystickEnabled)
            return;

        if (isTouchBased) {
            trackedTouch = MInput.GetTouchClosestToPosition(eventData.position);
        } else {
            lastMousePos = eventData.position;
        }

        joystickPos = GameUI.canvas.WorldToCanvasPos(eventData.position);

        isPressed = true;

        Show(true);

        OnEvent?.Invoke(new UIJoystickEventData(UIJoystickEventData.EventType.Start, Vector2.zero));
    }

    private void Show(bool show) {
        discRectT.gameObject.SetActive(show);
        dotRectT.gameObject.SetActive(show);
    }

    public enum JoystickRange {
        JoystickRectWidthPercent,
        ScreenHeightPercent,
        ScreenWidthPercent
    }

}

[System.Serializable]
public class UIJoystickEvent : UnityEvent<UIJoystickEventData> {

}

[System.Serializable]
public struct UIJoystickEventData {
    public EventType type;
    public Vector2 value;

    public UIJoystickEventData(EventType type, Vector2 value) {
        this.type = type;
        this.value = value;
    }

    public enum EventType {
        Start,
        Update,
        End,
        Cancel
    }
}
