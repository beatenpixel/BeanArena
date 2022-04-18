using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;

public class UIW_Message : UIWindow {

    public TextMeshProUGUI messageText;
    public RectTransform buttonsRoot;

    private RefreshableObjectsList<UISimpleButton> simpleButtons;
    private UIWData_Message data;

    protected override void Awake() {
        base.Awake();
        simpleButtons = new RefreshableObjectsList<UISimpleButton>(Buttons_Spawn, Buttons_Enable, Buttons_Refresh);
    }

    public override void OnInitBeforeOpen() {
        data = (UIWData_Message)genericWindowData;
    }

    public override void InternalUpdate() {

    }

    public override void OnStartOpening() {
        messageText.text = data.messageText;
        simpleButtons.Refresh(data.buttons.Length);
    }

    public override void OnOpened() {

    }

    public override void OnStartClosing() {

    }

    public override void OnClosed() {

    }

    private UISimpleButton Buttons_Spawn(int id) {
        UISimpleButton button = MPool.Get<UISimpleButton>();
        button.rectT.SetParent(buttonsRoot);
        button.rectT.localScale = Vector3.one;
        return button;
    }

    private void Buttons_Enable(int id, UISimpleButton button, bool enable) {
        button.Enable(enable);
    }

    private void Buttons_Refresh(int id, UISimpleButton button) {
        UIW_ButtonConfig buttonConfig = data.buttons[id];
        button.SetText(buttonConfig.text);
        button.SetBackgroundColor(buttonConfig.color);
        button.EnableIcon(false);

        button.SetOnClick(new ButtonClickArg() {
            buttonID = id,
            callbackArg = buttonConfig.arg,
            callback = buttonConfig.callback,
            closeOnPress = buttonConfig.closeWindowOnPress
        }, OnButtonClick);
    }

    private void OnButtonClick(object arg) {
        ButtonClickArg buttonClickArg = (ButtonClickArg)arg;
        buttonClickArg.callback?.Invoke(buttonClickArg.callbackArg);

        if (buttonClickArg.closeOnPress) {
            Open(false);
        }
    }

    public override Type GetPoolObjectType() {
        return typeof(UIWindow);
    }

    public override UIW_Opener GenerateWindowOpener() {
        return new IGNWindowSimpleOpener();
    }

    public override UIWindowInfo GetUIWindowInfo() {
        return new UIWindowInfo() {
            windowDataType = typeof(UIWData_Message),
            windowType = typeof(UIW_Message),
            layer = UIWindowLayerType.Notification
        };
    }

    public struct ButtonClickArg {
        public int buttonID;
        public object callbackArg;
        public Action<object> callback;
        public bool closeOnPress;
    }
}

public class UIWData_Message : UIW_Data {
    public string messageText;
    public UIW_ButtonConfig[] buttons;

    public UIWData_Message(string someText) {
        UIWindowManager.CreateWindow(new UIWData_Message(someText,
        new UIW_ButtonConfig(MLocalization.OK, MAssets.inst.colors[MAssets.COLOR_BUTTON_GRAY], (x) => {

        }, null)
        ));
    }

    public UIWData_Message(string _someText, params UIW_ButtonConfig[] _buttons) {
        messageText = _someText;
        buttons = _buttons;
    }

    public UIWData_Message(SerializationInfo info, StreamingContext sc) : base(info, sc) {
        messageText = info.GetString("messageText");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("messageText", messageText);
    }

}

public class UIW_ButtonConfig {
    public string text;
    public Color color;
    public Action<object> callback;
    public object arg;
    public bool closeWindowOnPress = true;

    public UIW_ButtonConfig(string _text, Color _color, Action<object> _callback, object _arg) {
        arg = _arg;
        text = _text;
        color = _color;
        callback = _callback;
    }

    public UIW_ButtonConfig SetCloseOnPress(bool closeOnPress) {
        closeWindowOnPress = closeOnPress;
        return this;
    }

}