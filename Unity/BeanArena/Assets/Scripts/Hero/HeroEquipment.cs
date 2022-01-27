using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroEquipment : HeroComponent {

	public List<EquipmentSlot> slots;

	public void Init() {
		slots = new List<EquipmentSlot>() {
			new EquipmentSlot() {
				allowedCategory = ItemCategory.Weapon,
				limb = hero[LimbType.LArm].First()
			},
			new EquipmentSlot() {
				allowedCategory = ItemCategory.BottomGadget,
				limb = hero[LimbType.Body].First()
			},
			new EquipmentSlot() {
				allowedCategory = ItemCategory.UpperGadget,
				limb = hero[LimbType.Body].First()
			},
		};
	}

	public void OnButtonInput(ButtonInputEventData inp) {
		HeroLimb limb;

		if (inp.buttonID == 0) {
			limb = hero[LimbType.RArm].First();
		} else {
			limb = hero[LimbType.LArm].First();
		}

		/*
		if (limb.equipment.Count > 0) {
			limb.equipment[0].Use(new EquipmentUseArgs() {
				charge = inp.chargePercent
			});
		}
		*/
	}

	public void AttachEquipment(Equipment equip, EquipmentSlot slot) {
		equip.AttachToHero(hero, slot.limb);
		slot.AttachEquipment(equip);
	}

	public void UnattachEquipment(EquipmentSlot slot) {
		Debug.Log("UnattachEquipment: 2");
		
		slot.equipment.UnattachFromHero();
		slot.UnattachEquipment();
	}

	public bool CanAttachEquipment(Equipment equip) {
		/*
		var freeSlots = slots.Where(x =>x.allowedCategory == equip.itemInfo.category
		&& MUtils.EnumAnyInMask((int)equip.canAttachToLimbs, (int)x.limb.limbType)
		&& x.limb.CanEquip(equip));

		if(freeSlots.Count() > 0) {
			EquipmentSlot slot = freeSlots.First();


		}
		*/
		/*
		if (targetLimbs.Count() > 0) {
			var freeLimbs = targetLimbs.Where(x => x.CanEquip(equip));

			if (freeLimbs.Count() > 0) {
				HeroLimb freeLimb = freeLimbs.First();

				equip.AttachToHero(hero, freeLimb);

				attachedEquipment.Add(equip);

				return true;
			}
		}
		*/

		return false;
	}

	/*
	public void ClearEquipment(Equipment equip) {
        if(attachedEquipment.Contains(equip)) {
			equip.UnattachFromHero();
			attachedEquipment.Remove(equip);
        }
    }
	*/

}

[System.Serializable]
public class EquipmentSlot {	
	public ItemCategory allowedCategory;
	public HeroLimb limb;
	public Equipment equipment { get; private set; }

	public void AttachEquipment(Equipment equip) {
		equipment = equip;
    }

	public void UnattachEquipment() {
		equipment = null;
		Debug.Log("UnattachEquipment: 5");
	}

	public SlotPlaceResult CheckPlace(GD_Item item) {
		if (MUtils.EnumAllInMask((int)allowedCategory, (int)item.info.category)) {
			if (equipment == null) {
				return SlotPlaceResult.CanPlace;
			} else {
				return SlotPlaceResult.CanReplace;
            }
		} else {
			return SlotPlaceResult.CanNotPlace;
        }		
    }

	public bool IsFree() {
		return equipment == null;
    }
}