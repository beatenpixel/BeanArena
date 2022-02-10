using DG.Tweening;
using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;

public class UIW_ChestReward : UIWindow {

    public Animator anim;
    public IconDrawer iconDrawer;
    public RectTransform totalIconDrawersRoot;

    private UIWData_ChestReward data;
    private List<GD_Item> openedItems;
    private RefreshableObjectsList<IconDrawer> totalIcons;

    private bool waitingForClickToClose;

    protected override void Awake() {
        base.Awake();
        totalIcons = new RefreshableObjectsList<IconDrawer>(Icons_Spawn, Icons_Enable, Icons_Refresh);
    }

    public override void OnInitBeforeOpen() {
        data = (UIWData_ChestReward)genericWindowData;
        openedItems = new List<GD_Item>();
        totalIcons.Refresh(data.items.Count);
        waitingForClickToClose = false;
    }

    public override void InternalUpdate() {        

        if(Input.GetMouseButtonDown(0)) {

            if (waitingForClickToClose) {
                data.CloseChestOpenerCallback?.Invoke();
                Open(false);
                return;
            }

            NextItem();
        }
    }

    private void NextItem() {
        if (openedItems.Count < data.items.Count) {
            GD_Item nextItem = data.items[openedItems.Count];
            openedItems.Add(nextItem);

            iconDrawer.gameObject.SetActive(true);
            iconDrawer.DrawItem(nextItem, nextItem.info);
            anim.Play("next_item", 0, 0f);

            if (openedItems.Count == data.items.Count) {
                this.WaitRealtime(() => {
                    iconDrawer.gameObject.SetActive(false);
                    ShowTotalIcons();

                    waitingForClickToClose = true;
                }, 0.3f);
            }
        }
    }

    public override void OnStartOpening() {

    }

    public override void OnOpened() {
        NextItem();
    }

    public override void OnStartClosing() {

    }

    public override void OnClosed() {

    }

    private void ShowTotalIcons() {
        for (int i = 0; i < totalIconDrawersRoot.childCount; i++) {
            Transform t = totalIconDrawersRoot.GetChild(i);            
            t.DOScale(1f, 0.3f).SetEase(Ease.OutBounce).SetDelay(i * 0.1f);
        }
    }

    private IconDrawer Icons_Spawn(int id) {
        IconDrawer icon = MPool.Get<IconDrawer>();
        icon.t.SetParent(totalIconDrawersRoot);        
        return icon;
    }

    private void Icons_Enable(int id, IconDrawer icon, bool enable) {
        icon.gameObject.SetActive(enable);
    }

    private void Icons_Refresh(int id, IconDrawer icon) {
        icon.t.localScale = new Vector3(0, 1, 1);
        icon.DrawItem(data.items[id], data.items[id].info);
    }

    public override Type GetPoolObjectType() {
        return typeof(UIWindow);
    }

    public override UIW_Opener GenerateWindowOpener() {
        return new IGNWindowSimpleOpener();
    }

    public override UIWindowInfo GetUIWindowInfo() {
        return new UIWindowInfo() {
            windowDataType = typeof(UIWData_ChestReward),
            windowType = typeof(UIW_ChestReward),
            layer = UIWindowLayerType.Notification
        };
    }
}

public class UIWData_ChestReward : UIW_Data {

    public List<GD_Item> items;
    public Action CloseChestOpenerCallback;

    public UIWData_ChestReward() {

    }

    public UIWData_ChestReward(SerializationInfo info, StreamingContext sc) : base(info, sc) {

    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
    }

}
