using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroItemButton : PoolObject {

    public RectTransform rectT;
    public UISimpleButton button;
    public IconDrawer iconDrawer;
    public Image lockImage;

    public event Action<UIEventType, HeroItemButton, object> OnEvent;
    private object onClickArg;

    protected override void Awake() {
        base.Awake();

        button.OnClickEvent += OnButtonClick;
    }

    public void Init() {

    }

    public void DrawItem(GD_HeroItem item) {
        iconDrawer.DrawHero(item);

        if(item.isUnlocked) {
            lockImage.enabled = false;
        } else {
            lockImage.enabled = true;
        }
    }

    public void SetArg(object arg) {
        onClickArg = arg;
    }

    private void OnButtonClick() {        
        OnEvent?.Invoke(UIEventType.Click, this, onClickArg);
    }

    public override Type GetPoolObjectType() {
        return typeof(HeroItemButton);
    }

}
