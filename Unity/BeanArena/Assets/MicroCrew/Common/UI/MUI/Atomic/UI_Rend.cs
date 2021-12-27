using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_Rend : PoolObject {

    [SerializeField] protected RectTransform rectT;
    public CanvasGroup canvasGroup;
    [SerializeField] protected UIPointerEventListener eventListener;

    private UIData_Rect rectData;

    public override void OnCreate() {
        base.OnCreate();
        rectData = new UIData_Rect();
    }

    private void LateUpdate() {
        rectData.ApplyToRectIfDirty(rectT);
    }

    public override void OnPop() {
        base.OnPop();
        ClearEventListener();
    }

    public override void OnPush() {
        base.OnPush();
        rectT.SetParent(null);
    }

    public virtual void ClearEventListener() {
        eventListener.ClearEvent();
        eventListener.enabled = false;
    }

    public virtual void EnableEventListener(Action<UIPointerEvent> callback) {
        eventListener.enabled = true;
        eventListener.OnEvent += callback;
    }

    public RectTransform GetRectT() {
        return rectT;
    }

    public void SetParent(UI_Rend parentRend) {
        rectT.SetParent(parentRend.rectT);
    }

    public void SetParent(RectTransform parentT) {
        rectT.SetParent(parentT);
    }

    public override Type GetPoolObjectType() {
        return typeof(UI_Rend);
    }

    public UI_Rend ApplyRectData(UIData_Rect _rectData) {
        rectData = _rectData;
        return this;
    }

    public UI_Rend Anchor(float x, float y) {
        rectData.Anchor(x, y);
        return this;
    }

    public UI_Rend AnchorMin(float x, float y) {
        rectData.AnchorMin(x, y);
        return this;
    }

    public UI_Rend AnchorMax(float x, float y) {
        rectData.AnchorMax(x, y);
        return this;
    }

    public UI_Rend Offset(float x, float y) {
        rectData.Offset(x, y);
        return this;
    }

    public UI_Rend OffsetInset(float x, float y) {
        rectData.OffsetInset(x, y);
        return this;
    }

    public UI_Rend OffsetMin(float x, float y) {
        rectData.OffsetMin(x, y);
        return this;
    }

    public UI_Rend OffsetMax(float x, float y) {
        rectData.OffsetMax(x, y);
        return this;
    }

    public UI_Rend Pivot(float x, float y) {
        rectData.Pivot(x, y);
        return this;
    }

    public UI_Rend Size(float x, float y) {
        rectData.Size(x, y);
        return this;
    }

    public UI_Rend Pos(float x, float y) {
        rectData.Pos(x, y);
        return this;
    }

    public UI_Rend Scale(float x, float y) {
        rectData.Scale(x, y);
        return this;
    }

}

public struct UIData_Rect {
    private bool isDirty;

    private Vector2? anchorMin;
    private Vector2? anchorMax;
    private Vector2? offsetMin;
    private Vector2? offsetMax;
    private Vector2? pivot;
    private Vector2? anchoredPos;
    private Vector2? sizeDelta;
    private Vector2? localScale;

    public void ApplyToRectIfDirty(RectTransform rectT) {
        if (!isDirty)
            return;

        if (pivot != null) {
            rectT.pivot = (Vector2)pivot;
        }
        if (sizeDelta != null) {
            rectT.sizeDelta = (Vector2)sizeDelta;
        }
        if (anchoredPos != null) {
            rectT.anchoredPosition = (Vector2)anchoredPos;
        }
        if (anchorMin != null) {
            rectT.anchorMin = (Vector2)anchorMin;
        }
        if (anchorMax != null) {
            rectT.anchorMax = (Vector2)anchorMax;
        }
        if (offsetMin != null) {
            rectT.offsetMin = (Vector2)offsetMin;
        }
        if (offsetMax != null) {
            rectT.offsetMax = (Vector2)offsetMax;
        }
        if(localScale != null) {
            Vector2 s = (Vector2)localScale;
            rectT.localScale = new Vector3(s.x, s.y, 1);
        }

        Clear();
    }

    private void Clear() {
        pivot = null;
        sizeDelta = null;
        anchoredPos = null;
        anchorMin = null;
        anchorMax = null;
        offsetMin = null;
        offsetMax = null;

        isDirty = false;
    }

    public UIData_Rect Anchor(float x, float y) {
        isDirty = true;
        anchorMin = new Vector2(x, y);
        anchorMax = new Vector2(x, y);
        return this;
    }

    public UIData_Rect AnchorMin(float x, float y) {
        isDirty = true;
        anchorMin = new Vector2(x, y);
        return this;
    }

    public UIData_Rect AnchorMax(float x, float y) {
        isDirty = true;
        anchorMax = new Vector2(x, y);
        return this;
    }

    public UIData_Rect Offset(float x, float y) {
        isDirty = true;
        offsetMin = new Vector2(x, y);
        offsetMax = new Vector2(x, y);
        return this;
    }

    public UIData_Rect OffsetInset(float x, float y) {
        isDirty = true;
        offsetMin = new Vector2(x, y);
        offsetMax = new Vector2(-x, -y);
        return this;
    }

    public UIData_Rect OffsetMin(float x, float y) {
        isDirty = true;
        offsetMin = new Vector2(x, y);
        return this;
    }

    public UIData_Rect OffsetMax(float x, float y) {
        isDirty = true;
        offsetMax = new Vector2(x, y);
        return this;
    }

    public UIData_Rect Pivot(float x, float y) {
        isDirty = true;
        pivot = new Vector2(x, y);
        return this;
    }

    public UIData_Rect Size(float x, float y) {
        isDirty = true;
        sizeDelta = new Vector2(x, y);
        return this;
    }

    public UIData_Rect Pos(float x, float y) {
        isDirty = true;
        anchoredPos = new Vector2(x, y);
        return this;
    }

    public UIData_Rect Scale(float x, float y) {
        localScale = new Vector3(x, y);
        return this;
    }

}

public abstract class UI_Element {
    public string name;
    public UIElementType type;
    public List<UI_Element> childs;

    public UI_Rend rootRend;

    public UI_Element(string _name, UIElementType _type) {
        name = _name;
        type = _type;

        if (rootRend != null) {
            rootRend.Scale(1f, 1f);
        }
    }

    protected void SetRootRend(UI_Rend rend) {
        rootRend = rend;
        rootRend.gameObject.name = name;
    }

    protected UI_Image CreateImage(string _name, UI_Element parent) {
        UI_Image result = new UI_Image(_name, (parent==null)?null:parent.rootRend.GetRectT());
        result.rootRend.Scale(1f,1f);
        return result;
    }

    protected UI_Text CreateText(string _name, UI_Element parent) {
        UI_Text result = new UI_Text(_name, (parent == null) ? null : parent.rootRend.GetRectT());
        result.rootRend.Scale(1f, 1f);
        return result;
    }

    protected UI_Rect CreateRect(string _name, UI_Element parent) {
        UI_Rect result = new UI_Rect(_name, (parent == null) ? null : parent.rootRend.GetRectT());
        result.rootRend.Scale(1f, 1f);
        return result;
    }

}

public class UI_Rect : UI_Element {
    public UI_RectRend rectRend;

    public UI_Rect(string _name, RectTransform parent = null) : base(_name, UIElementType.Rect) {
        rectRend = MPool.Get<UI_RectRend>();
        if (parent != null) {
            rectRend.SetParent(parent);
        }
        rectRend.Scale(1f,1f);
        SetRootRend(rectRend);
    }
}

public class UI_Image : UI_Element {
    public UI_ImageRend imageRend;

    public UI_Image(string _name, RectTransform parent = null) : base(_name, UIElementType.Image) {
        imageRend = MPool.Get<UI_ImageRend>();
        if(parent != null) {
            imageRend.SetParent(parent);
        } else {
            imageRend.SetParent(MUI.inst.uiCanvas.canvasT);
        }
        imageRend.Scale(1f, 1f);
        SetRootRend(imageRend);
    }

    public UI_Image Sprite(Sprite sprite, Image.Type imageType = Image.Type.Simple, float pixelsPerUnit = 1f) {
        imageRend.image.type = imageType;
        imageRend.image.sprite = sprite;
        imageRend.image.pixelsPerUnitMultiplier = pixelsPerUnit;
        return this;
    }

    public UI_Image Sprite(string spriteName, Image.Type imageType = Image.Type.Simple, float pixelsPerUnit = 1f) {
        return Sprite(MAssets.ui_tex_0[spriteName], imageType, pixelsPerUnit);
    }

    public UI_Image Color(Color color) {
        imageRend.image.color = color;
        return this;
    }

}

public class UI_Text : UI_Element {
    public UI_TextRend textRend;

    public UI_Text(string _name, RectTransform parent = null) : base(_name, UIElementType.Text) {
        textRend = MPool.Get<UI_TextRend>();
        if (parent != null) {
            textRend.SetParent(parent);
        }
        textRend.Scale(1f, 1f);
        SetRootRend(textRend);
    }

    public UI_Text Align(TextAlignmentOptions option) {
        textRend.text.alignment = option;
        return this;
    }

    public UI_Text Text(string text) {
        textRend.text.text = text;
        return this;
    }

    public UI_Text Color(Color color) {
        textRend.text.color = color;
        return this;
    }

}

public enum UIElementType {
    None,
    // Atomic
    Rect,
    Image,
    Text,
    // Micro
    Button,
    // Medium
    SwitchPanel
    // Large
}
