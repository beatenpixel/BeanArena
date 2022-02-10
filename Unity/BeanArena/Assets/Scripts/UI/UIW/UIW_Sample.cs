using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;

public class UIW_Sample : UIWindow {
    private UIWData_Sample data;

    protected override void Awake() {
        base.Awake();

    }

    public override void OnInitBeforeOpen() {
        data = (UIWData_Sample)genericWindowData;
    }

    public override void InternalUpdate() {

    }

    public override void OnStartOpening() {

    }

    public override void OnOpened() {

    }

    public override void OnStartClosing() {

    }

    public override void OnClosed() {

    }

    public override Type GetPoolObjectType() {
        return typeof(UIWindow);
    }

    public override UIW_Opener GenerateWindowOpener() {
        return new IGNWindowSimpleOpener();
    }

    public override UIWindowInfo GetUIWindowInfo() {
        return new UIWindowInfo() {
            windowDataType = typeof(UIWData_Sample),
            windowType = typeof(UIWData_Sample),
            layer = UIWindowLayerType.Notification
        };
    }
}

public class UIWData_Sample : UIW_Data {
    public UIWData_Sample(string _someText) {

    }

    public UIWData_Sample(SerializationInfo info, StreamingContext sc) : base(info, sc) {

    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
    }

}
