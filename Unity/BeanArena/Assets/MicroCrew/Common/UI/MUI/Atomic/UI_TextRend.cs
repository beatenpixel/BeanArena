using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TextRend : UI_Rend {

    public TextMeshProUGUI text;

    public override void ClearEventListener() {
        base.ClearEventListener();
        text.raycastTarget = false;
    }

    public override void EnableEventListener(Action<UIPointerEvent> callback) {
        base.EnableEventListener(callback);
        text.raycastTarget = true;
    }

    public override Type GetPoolObjectType() {
        return typeof(UI_TextRend);
    }

}
