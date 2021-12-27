using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Button : UI_Element {

    public UI_Image mainImage;
    public UI_Image backgroundImage;
    public UI_Text text;
    public UI_Image icon;

    private Action OnClickEvent;
    private Action<object> OnClickEventObj;
    private object clickObj;

    private bool isPressed;
    private bool isHovered;
    private UIData_Button data;

    public UI_Button(string _name) : base(_name, UIElementType.Button) {
        mainImage = CreateImage("main", null).Sprite("square").Color(Color.white.SetA(1f / 255));
        SetRootRend(mainImage.imageRend);

        backgroundImage = CreateImage("background", this).Sprite("circle", Image.Type.Sliced, 10).Color(Color.red);
        text = CreateText("text", backgroundImage).Text("microcrew").Color(Color.black).Align(TextAlignmentOptions.Center);

        mainImage.rootRend.EnableEventListener(OnPointerEvent);

        rootRend.Anchor(0.5f, 0.5f).Pivot(0.5f, 0.5f).Pos(0, 0).Size(MUI.sizeX * 0.3f, MUI.sizeX * 0.1f);
        backgroundImage.rootRend.AnchorMin(0f, 0f).AnchorMax(1f, 1f).Offset(0, 0).Scale(1f, 1f);
       
    }

    public UI_Button(string _name, RectTransform parentT, UIData_Button buttonData) : base(_name, UIElementType.Button) {
        data = buttonData;

        if(data.highlightColor == null) {
            Color hightlight = Color.Lerp(data.defaultColor, Color.white, 0.2f).SetA(data.defaultColor.a);
            data.highlightColor = hightlight;
        }

        if (data.pressedColor == null) {
            Color pressed = Color.Lerp(data.defaultColor, Color.black, 0.2f).SetA(data.defaultColor.a);
            data.pressedColor = pressed;
        }

        SetClick(data.OnClick);

        mainImage = CreateImage("main", null).Sprite("square").Color(Color.white.SetA(1f / 255));
        SetRootRend(mainImage.imageRend);
        rootRend.SetParent(parentT);

        mainImage.rootRend.EnableEventListener(OnPointerEvent);

        backgroundImage = CreateImage("background", this).Sprite("circle", Image.Type.Sliced, 10).Color(data.defaultColor);

        rootRend.Anchor(0.5f, 0.5f).Pivot(0.5f, 0.5f).Pos(0, 0).Size(MUI.sizeX * 0.3f, MUI.sizeX * 0.1f);
        backgroundImage.rootRend.AnchorMin(0f, 0f).AnchorMax(1f, 1f).Offset(0, 0).Scale(1f, 1f);
    }

    public void SetClick(Action action) {
        OnClickEvent = action;
    }

    public void SetClick(Action<object> action, object obj) {
        clickObj = obj;
        OnClickEventObj = action;
    }

    public UI_Text CreateText(string _text, Color _color) {
        text = CreateText("text", backgroundImage).Text(_text).Color(_color).Align(TextAlignmentOptions.Center);
        text.rootRend.AnchorMin(0.1f, 0.1f).AnchorMax(0.9f, 0.9f).Offset(0, 0);
        return text;
    }

    private void OnPointerEvent(UIPointerEvent e) {
        if (e.type == PointerEventType.Enter) {
            isHovered = true;
            backgroundImage.rootRend.GetRectT().DOKill(true);
            backgroundImage.Color((Color)data.highlightColor);
        } else if (e.type == PointerEventType.Down) {
            isPressed = true;
            backgroundImage.rootRend.GetRectT().DOKill(true);
            backgroundImage.rootRend.GetRectT().DOScale(0.95f, 0.05f);
            backgroundImage.Color((Color)data.pressedColor);
        } else if (e.type == PointerEventType.Up) {            
            if (isHovered) {
                isPressed = false;
                backgroundImage.Color((Color)data.highlightColor);
            } 
        } else if (e.type == PointerEventType.Exit) {
            isHovered = false;
            backgroundImage.Color(data.defaultColor);
            if (isPressed) {
                backgroundImage.rootRend.GetRectT().DOKill(true);
                backgroundImage.rootRend.GetRectT().DOScale(1f, 0.05f);
            }
        } else if (e.type == PointerEventType.Click) {
            isPressed = false;
            backgroundImage.Color((Color)data.highlightColor);
            backgroundImage.rootRend.GetRectT().DOKill(true);
            backgroundImage.rootRend.GetRectT().DOScale(1f, 0.5f).SetEase(Ease.OutElastic);

            OnClickEvent?.Invoke();
            OnClickEventObj?.Invoke(clickObj);
        }
    }

    public struct UIData_Button {
        public Color defaultColor;
        public Color? highlightColor;
        public Color? pressedColor;
        public Action OnClick;
    }

    /*
    public struct UIData_Button {
        public Color defaultColor;
        public Color highlightColor;
        public Color pressedColor;
        public Action OnClick;

        public UIData_Image backgroundData;
        public UIData_Image? iconData;
        public UIData_Text? textData;

        public UIData_Rect rectData;
    }
    */

    public struct UIData_Image {
        public Sprite sprite;
        public Color color;
        public UIData_Rect rectData;
    }

    public struct UIData_Text {
        public Color color;
        public string text;
        public UIData_Rect rectData;
    }

}