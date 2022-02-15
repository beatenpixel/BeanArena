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
    private List<ItemToOpen> itemsToOpen;
    private RefreshableObjectsList<IconDrawer> totalIcons;

    private bool waitingForClickToClose;
    private int openedItemsCount;

    protected override void Awake() {
        base.Awake();
        totalIcons = new RefreshableObjectsList<IconDrawer>(Icons_Spawn, Icons_Enable, Icons_Refresh);
    }

    public override void OnInitBeforeOpen() {
        data = (UIWData_ChestReward)genericWindowData;
        itemsToOpen = new List<ItemToOpen>();
        waitingForClickToClose = false;
        openedItemsCount = 0;

        Game.data.inventory.items.AddRange(data.content.items);


        for (int i = 0; i < data.content.items.Count; i++) {
            itemsToOpen.Add(new ItemToOpen() {
                item = data.content.items[i]
            });
        }

        for (int i = 0; i < data.content.heroCards.Count; i++) {
            GD_HeroItem itemData = Game.data.inventory.heroes.Find(x => x.heroType == data.content.heroCards[i].heroType);
            itemData.cardsCollected += data.content.heroCards[i].amount; 

            itemsToOpen.Add(new ItemToOpen() {
                item = null,
                heroCard = data.content.heroCards[i]
            });
        }

        totalIcons.Refresh(itemsToOpen.Count);
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
        if (openedItemsCount < itemsToOpen.Count) {
            ItemToOpen itemToOpen = itemsToOpen[openedItemsCount];
            
            iconDrawer.gameObject.SetActive(true);
            if (itemToOpen.item != null) {
                iconDrawer.DrawItem(itemToOpen.item, itemToOpen.item.info);
            } else {
                iconDrawer.DrawHeroDrop(itemToOpen.heroCard.heroType, itemToOpen.heroCard.amount);
            }
            
            iconDrawer.EnableRedDot(false);
            anim.Play("next_item", 0, 0f);

            openedItemsCount++;
        } else {
            this.WaitRealtime(() => {
                iconDrawer.gameObject.SetActive(false);
                ShowTotalIcons();

                waitingForClickToClose = true;
            }, 0.3f);
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

        ItemToOpen itemToOpen = itemsToOpen[id];

        if (itemToOpen.item != null) {
            icon.DrawItem(itemToOpen.item, itemToOpen.item.info);
        } else {
            icon.DrawHeroDrop(itemToOpen.heroCard.heroType, itemToOpen.heroCard.amount);
        }
        //icon.DrawItem(data.content.items[id], data.content.items[id].info);
        icon.EnableRedDot(false);
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

    public class ItemToOpen {
        public GD_Item item;
        public HeroCardsContainer heroCard;
    }

}

public class UIWData_ChestReward : UIW_Data {

    public ChestContent content;
    public Action CloseChestOpenerCallback;

    public UIWData_ChestReward() {

    }

    public UIWData_ChestReward(SerializationInfo info, StreamingContext sc) : base(info, sc) {

    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
    }

}