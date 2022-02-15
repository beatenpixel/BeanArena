using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroEquipment : HeroComponent {

	private List<EquipmentSlot> slots;

	public int GetSlotsCount() {
		return slots.Count;
    }

	public EquipmentSlot GetEquipmentSlot(int ind) {
		return slots[ind];
    }

	public void Init() {
		slots = new List<EquipmentSlot>() {
			new EquipmentSlot(hero, new ItemFilter(ItemCategory.Weapon)).SetLimb(hero[LimbType.LArm].First()),
			new EquipmentSlot(hero, new ItemFilter(ItemCategory.BottomGadget)).SetLimb(hero[LimbType.Body].First()),
			new EquipmentSlot(hero, new ItemFilter(ItemCategory.UpperGadget)).SetLimb(hero[LimbType.Body].First()),
			new EquipmentSlot(hero, new ItemFilter(ItemCategory.Weapon)).SetLimb(hero[LimbType.RArm].First()),
		};
	}

    public void LoadEquipmentFromGameData() {
        for (int i = 0; i < Game.data.inventory.items.Count; i++) {
            GD_Item item = Game.data.inventory.items[i];

            if (!item.isEquiped) {
                continue;
            }

            List<EquipmentSlot> heroFreeSlots = GetFreeSlots(item);
            if (heroFreeSlots.Count > 0) {
                EquipmentSlot equipSlot = heroFreeSlots[0];

                for (int x = 0; x < heroFreeSlots.Count; x++) {                    
                    PreviewItem(item, equipSlot);
                    break;
                }
            }
        }
    }

    public void DestroyEquipment() {
        for (int i = 0; i < slots.Count; i++) {
            slots[i].DestroyEquipment();
        }
    }

    public void PreviewItem(GD_Item item, EquipmentSlot slot) {
		Equipment itemPreviewInstance = Game.inst.equipmentFactory.Create(item, Vector2.zero);
		slot.EquipPreviewItem(item, itemPreviewInstance);
    }

	public void ClearPreviewItem(EquipmentSlot slot) {
		slot.ClearPreviewItem();
    }

	public List<EquipmentSlot> GetFreeSlots(GD_Item item) {
		List<EquipmentSlot> freeSlots = new List<EquipmentSlot>();

        for (int i = 0; i < slots.Count; i++) {
			if(slots[i].IsFree() && slots[i].ItemFits(item)) {
				freeSlots.Add(slots[i]);
            }
        }

		return freeSlots;
    }

	public void OnButtonInput(ButtonInputEventData inp) {
		HeroLimb limb;

		if (inp.buttonID == 0) {
			limb = hero[LimbType.RArm].First();
		} else {
			limb = hero[LimbType.LArm].First();
		}

        Equipment equipment =  slots[0].GetCurrentEquipment();
        if(equipment != null) {
            equipment.Use(new EquipmentUseArgs() {
                charge = inp.chargePercent
            });
        }
	}
}

[System.Serializable]
public class EquipmentSlot {
	private ItemFilter filter;

	private GD_Item equipedItem;
	private Equipment equipedEquipment;

	private GD_Item previewItem;
	private Equipment previewEquipment;

	private HeroBase hero;
	private HeroLimb limb;

	public EquipmentSlot(HeroBase hero, ItemFilter filter) {
		this.hero = hero;
		this.filter = filter;
	}

	public EquipmentSlot SetLimb(HeroLimb limb) {
		this.limb = limb;
		return this;
    }

    public void DestroyEquipment() {
        if (previewEquipment != null) {
            previewEquipment.UnattachFromHero();
            previewEquipment.Push();
            previewEquipment = null;
        }
    }

	public void ClearPreviewItem() {
		if (previewEquipment != null) {
			previewEquipment.UnattachFromHero();
			previewEquipment = null;
		}
		
		previewItem = null;
	}

	public void EquipPreviewItem(GD_Item item, Equipment equipmentinstance) {
		previewItem = item;
		previewEquipment = equipmentinstance;

		equipmentinstance.AttachToHero(hero, limb);
	}

    public Equipment GetCurrentEquipment() {
        return previewEquipment;
    }

	public bool ItemFits(GD_Item item) {
		return filter.Check(item);
    }

	public bool HasPreviewItem() {
		return previewItem != null;
    }

	public bool IsFree() {
		return equipedEquipment == null;
    }

}