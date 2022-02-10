using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MicroCrew.Utils;
using System.Runtime.Serialization;

public class UIWindowManager : SingletonFromResources<UIWindowManager> {

    public UICanvas uiCanvas;
    public RectTransform layersHolderT;
    public UIW_LayerHolder layerHolderPrefab;

    private List<UIWindow> windowsPrefabs;
    private Dictionary<Type, UIWindow> windowsByIGNData;

    private Dictionary<UIWindowLayerType, UIWindowLayer> layers;

    public override void Init() {
        windowsByIGNData = new Dictionary<Type, UIWindow>();

        layers = new Dictionary<UIWindowLayerType, UIWindowLayer>() {
            { UIWindowLayerType.Main, new UIWindowLayer() {
                info = new UIWindowLayerInfo() {layer = UIWindowLayerType.Main, allowMultipleWindows = false, drawPriority = 0},
                queuedWindows = new List<UIW_Data>()
            } },
            { UIWindowLayerType.Notification, new UIWindowLayer() {
                info = new UIWindowLayerInfo() {layer = UIWindowLayerType.Notification, allowMultipleWindows = false, drawPriority = 1},
                queuedWindows = new List<UIW_Data>()
            } },
        };

        foreach (var layer in layers.Values) {
            UIW_LayerHolder newLayerHolder = Instantiate(layerHolderPrefab, layersHolderT);
            newLayerHolder.rectT.localPosition = Vector3.zero;
            newLayerHolder.rectT.localScale = Vector3.one;
            newLayerHolder.rectT.SetMargin(Vector2.zero, Vector2.zero);
            newLayerHolder.Init();

            layer.layerHolder = newLayerHolder;

            layer.Init(this, newLayerHolder);
        }

        windowsPrefabs = MPool.GetPrefabs<UIWindow>(true);
        Debug.Log("prefabs added: " + windowsPrefabs.Count);

        for (int i = 0; i < windowsPrefabs.Count; i++) {
            windowsByIGNData[windowsPrefabs[i].GetUIWindowInfo().windowDataType] = windowsPrefabs[i];
        }

        MGameLoop.Update.Register(100, InternalUpdate);
    }

    protected override void Shutdown() {

    }

    public void InternalUpdate() {
        foreach (var layer in layers.Values) {
            if (layer.currentExecutingWindow != null) {
                layer.currentExecutingWindow.InternalUpdate();
            }
        }
    }

    public static UIWindow CreateWindow<D>(D data, string prefabSubType = null) where D : UIW_Data {
        InitIfNeeded(null);

        UIWindow prefab = inst.windowsByIGNData[typeof(D)];

        UIWindowInfo windowInfo = prefab.GetUIWindowInfo();
        UIWindowLayer layer = inst.layers[windowInfo.layer];

        Debug.Log("PopOrCreate " + windowInfo.windowType.Name);
        UIWindow window = inst.PopOrCreate(prefab, layer.layerHolder.rectT, windowInfo.windowType.Name);

        data.executeWindow = window;

        inst.layers[windowInfo.layer].OpenWindow(data);

        return window;
    }

    private T PopOrCreate<T>(T prefab, RectTransform parentRect, string prefabSubType = null) where T : UIWindow {
        T result = MPool.Get<T>(prefabSubType);
        result.rectT.SetParent(parentRect);
        result.rectT.localPosition = Vector3.zero;
        result.rectT.localScale = Vector3.one;
        result.rectT.SetMargin(Vector2.zero, Vector2.zero);
        result.rectT.gameObject.SetActive(false);

        return result;
    }

}

public class UIWindowLayer {
    public List<UIW_Data> queuedWindows;
    public UIW_Data currentExecutingWindow;
    public UIWindowLayerInfo info;
    public UIW_LayerHolder layerHolder;

    private UIWindowManager manager;
    private UIW_SessionInfo sessionInfo;

    public void Init(UIWindowManager _manager, UIW_LayerHolder _layerHolder) {
        manager = _manager;
        layerHolder = _layerHolder;

        sessionInfo = new UIW_SessionInfo() {
            OnWindowStateChanged = OnCurrentIGNWindowStateChanged,
            canvas = manager.uiCanvas
        };
    }

    public void OpenWindow(UIW_Data data) {
        queuedWindows.Add(data);

        if (info.allowMultipleWindows) {
            Debug.Log("openigntype=" + queuedWindows[0].GetType());
            OpenIGN(queuedWindows[0]);
        } else {
            if (queuedWindows.Count == 1) {
                Debug.Log("openigntype=" + queuedWindows[0].GetType());
                OpenIGN(queuedWindows[0]);
            }
        }
    }

    private void OpenIGN(UIW_Data ignData) {
        currentExecutingWindow = ignData;
        currentExecutingWindow.executeWindow.Init(ignData, sessionInfo);
        currentExecutingWindow.BeforeOpenWindow();
        currentExecutingWindow.executeWindow.Open(true);

        //PlayerInput.SetInputAllowed(PlayerInputAllowFlag.IN_GAME_NOTIFIACTION, false);

        layerHolder.EnableBackgroundImage(true, ignData.fadeBlackBackground);
    }

    private void TryOpenNextIGN() {
        if (queuedWindows.Count > 0) {
            OpenIGN(queuedWindows[0]);
        } else {
            //PlayerInput.SetInputAllowed(PlayerInputAllowFlag.IN_GAME_NOTIFIACTION, true);
            layerHolder.EnableBackgroundImage(false, true);
        }
    }

    private void OnCurrentIGNWindowStateChanged(UIWindowState state) {
        Debug.Log("OnStateCanged " + state);

        switch (state) {
            case UIWindowState.CLOSED:
                currentExecutingWindow = null;
                queuedWindows.RemoveAt(0);

                MFunc.Wait(() => {
                    TryOpenNextIGN();
                }, 0.15f);
                break;
        }
    }

}

public interface IUIWindow {
    UIWindowInfo GetUIWindowInfo();
    void Open(bool open);
}

public struct UIWindowLayerInfo {
    public UIWindowLayerType layer;
    public bool allowMultipleWindows;
    public int drawPriority;
}

public struct UIWindowInfo {
    public Type windowDataType;
    public Type windowType;
    public UIWindowLayerType layer;
}

public enum UIWindowLayerType {
    None,
    Main,
    Notification
}

public enum UIWindowState {
    NONE,
    OPENING,
    OPENED,
    CLOSING,
    CLOSED
}

public class UIW_SessionInfo {
    public UICanvas canvas;
    public Action<UIWindowState> OnWindowStateChanged;
}

[System.Serializable]
public abstract class UIW_Data : GD {

    [NonSerialized] public UIWindow executeWindow;
    public bool fadeBlackBackground = true;

    public virtual void BeforeOpenWindow() {

    }

    public virtual void InternalUpdate() {
        executeWindow.InternalUpdate();
    }

    // Serialization

    public UIW_Data() : base(GDType.UIWindowData, GDLoadOrder.Default) {
        SetDefaults(default);
    }

    public void Restore_UIWindowData() {
        UIWindowManager.CreateWindow(this);
    }

    public UIW_Data(SerializationInfo info, StreamingContext sc) : base(info, sc) {

    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds) {

    }

}