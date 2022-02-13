using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGroupDrawer_Hero : InventoryGroupDrawer {

    private ObjectListSpawner<HeroItemButton> itemButtons;

    private List<GD_HeroItem> heroesToDraw;

    public override void Init(InventoryGroupConfig config) {
        base.Init(config);

        itemButtons = new ObjectListSpawner<HeroItemButton>(SpawnItemButton, Enable_ItemButton, Update_ItemButton);
    }

    public override void Show(bool show) {
        base.Show(show);
    }

    public override void Draw() {
        //base.Draw();
        heroesToDraw = Game.data.inventory.heroes;

        itemButtons.Update(heroesToDraw.Count);

        for (int i = 0; i < itemButtons.activeObjectsCount; i++) {
            GD_HeroItem item = heroesToDraw[i];
            HeroItemButton button = itemButtons[i];

            button.SetItem(item);
            button.SetArg(i);
            button.rectT.SetAsLastSibling();

            if (item.isEquiped) {
                //config.drawer.SetItemButtonEquiped(button);
                //button.SetState(ItemButton.ItemButtonState.InHeroSlot);
            } else {
                //button.SetState(ItemButton.ItemButtonState.InInventory);
            }
        }
    }

    private void OnItemButtonEvent(UIEventType eventType, HeroItemButton button, object arg) {
        if (eventType == UIEventType.Click) {
            int inventorySlotID = (int)arg;

            GD_HeroItem item = heroesToDraw[inventorySlotID];

            Game.data.SetEquipedHero(item);

            GM_Menu.inst.DestroyPreviewHero();
            GM_Menu.inst.SpawnPreviewHero();

            infoDrawer.DrawHeroInfo(item);
        }

        config.drawer.OnHeroItemButtonEvent(eventType, button, arg);
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

}
