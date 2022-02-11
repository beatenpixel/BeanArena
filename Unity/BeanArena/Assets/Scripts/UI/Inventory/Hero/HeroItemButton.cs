using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroItemButton : PoolObject {

    public RectTransform rectT;
    public UISimpleButton button;
    public IconDrawer iconDrawer;

    public event Action<UIEventType, HeroItemButton, object> OnEvent;
    private object onClickArg;

    protected override void Awake() {
        base.Awake();

        button.OnClickEvent += OnButtonClick;
    }

    public void Init() {

    }

    public void SetItem(GD_HeroItem item) {
        iconDrawer.DrawHero(item);
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
