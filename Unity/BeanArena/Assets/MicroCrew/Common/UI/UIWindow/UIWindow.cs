using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindow : PoolObject, IUIWindow {

    public RectTransform rectT;

    private UIW_SessionInfo windowSessionInfo;
    private UIW_Opener cachedWindowOpener;

    protected UIW_Data genericWindowData;

    public abstract UIWindowInfo GetUIWindowInfo();
    public abstract UIW_Opener GenerateWindowOpener();

    protected virtual void Awake() {
        gameObject.SetActive(false);
    }

    public void Init(UIW_Data _ignData, UIW_SessionInfo _windowSessionInfo) {
        genericWindowData = _ignData;
        windowSessionInfo = _windowSessionInfo;

        if (cachedWindowOpener == null) {
            cachedWindowOpener = GenerateWindowOpener();
        }

        cachedWindowOpener.Init(this, windowSessionInfo);

        OnInitBeforeOpen();
    }

    public void Open(bool open) {
        gameObject.SetActive(true);
        cachedWindowOpener.Open(open);
    }

    public abstract void OnInitBeforeOpen();
    public abstract void InternalUpdate();
    public abstract void OnStartOpening();
    public abstract void OnOpened();
    public abstract void OnStartClosing();
    public abstract void OnClosed();
}
