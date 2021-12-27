using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ImageRend : UI_Rend {

    public Image image;

    public override void ClearEventListener() {
        base.ClearEventListener();
        image.raycastTarget = false;
    }

    public override void EnableEventListener(Action<UIPointerEvent> callback) {
        base.EnableEventListener(callback);
        image.raycastTarget = true;
    }

    public override Type GetPoolObjectType() {
        return typeof(UI_ImageRend);
    }

}
