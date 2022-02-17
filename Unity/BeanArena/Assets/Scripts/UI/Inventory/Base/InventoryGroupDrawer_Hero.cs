using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGroupDrawer_Hero : InventoryGroupDrawer {

    private ObjectListSpawner<HeroItemButton> itemButtons;

    private List<GD_HeroItem> heroesToDraw;

    private HeroItemButton currentButton;

    public override void Init(InventoryGroupConfig config) {
        base.Init(config);

        config.tabButton.notificationDot.Enable(false);
        itemButtons = new ObjectListSpawner<HeroItemButton>(SpawnItemButton, Enable_ItemButton, Update_ItemButton, Destroy_ItemButton);
    }

    public override void Show(bool show) {
        base.Show(show);

        int newItemsCount = 0;
        for (int i = 0; i < Game.data.inventory.heroes.Count; i++) {
            if (Game.data.inventory.heroes[i].enoughCardsToUpgrade)
                newItemsCount += 1;
        }

        if (newItemsCount > 0) {
            config.tabButton.notificationDot.Enable(true, newItemsCount);
        } else {
            config.tabButton.notificationDot.Enable(false);
        }

        infoDrawer.Show(show);
    }

    public override void Draw() {
        base.Draw();

        heroesToDraw = Game.data.inventory.heroes;

        itemButtons.Update(heroesToDraw.Count);

        for (int i = 0; i < itemButtons.activeObjectsCount; i++) {
            GD_HeroItem item = heroesToDraw[i];
            HeroItemButton button = itemButtons[i];

            button.DrawItem(item);
            button.SetArg(i);
            button.rectT.SetAsLastSibling();

            if(item.isEquiped) {
                currentButton = button;
                infoDrawer.DrawHeroInfo(item, new HeroInfoDrawConfig() {
                    OnUpgradeButtonClickCallback = ButtonClick_HeroUpgrade
                });
            }
        }        
    }

    private void OnItemButtonEvent(UIEventType eventType, HeroItemButton button, object arg) {
        if (eventType == UIEventType.Click) {
            int inventorySlotID = (int)arg;
            GD_HeroItem item = heroesToDraw[inventorySlotID];

            if (item.isUnlocked) {
                currentButton = button;
                Game.data.SetEquipedHero(item);

                GM_Menu.inst.DestroyPreviewHero();
                GM_Menu.inst.SpawnPreviewHero();

                infoDrawer.DrawHeroInfo(item, new HeroInfoDrawConfig() {
                    OnUpgradeButtonClickCallback = ButtonClick_HeroUpgrade
                });
            }
        }

        config.inventoryUI.OnHeroItemButtonEvent(eventType, button, arg);
    }

    public void ButtonClick_HeroUpgrade() {
        GD_HeroItem heroItem = currentButton.currentItem;

        if(currentButton != null && currentButton.currentItem != null) {
            if(currentButton.currentItem.enoughCardsToUpgrade) {
                HeroLevelInfo levelInfo = heroItem.info.rarenessInfo.levelsInfo[heroItem.levelID];

                if (Economy.inst.HasCurrency(CurrencyType.Coin, levelInfo.coinsToLevel)) {
                    heroItem.Upgrade();

                    currentButton.DrawItem(currentButton.currentItem);

                    infoDrawer.DrawHeroInfo(heroItem, new HeroInfoDrawConfig() {
                        OnUpgradeButtonClickCallback = ButtonClick_HeroUpgrade
                    });
                } else {
                    
                }
            } else {

            }
        }
    }

    private HeroItemButton SpawnItemButton(int ind) {
        HeroItemButton button = MPool.Get<HeroItemButton>(null, itemsButtonsRootT);
        button.OnEvent += OnItemButtonEvent;
        return button;
    }

    private void Enable_ItemButton(HeroItemButton obj, int ind, bool enable) {
        obj.gameObject.SetActive(enable);
    }

    private void Update_ItemButton(HeroItemButton obj, int ind) {

    }

    private void Destroy_ItemButton(HeroItemButton obj, int ind) {

    }

}

public class HeroInfoDrawConfig {
    public Action OnUpgradeButtonClickCallback;
}