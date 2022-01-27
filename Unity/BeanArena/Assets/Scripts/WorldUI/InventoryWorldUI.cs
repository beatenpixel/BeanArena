using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWorldUI : MonoBehaviour {

	public Hero targetHero;

	public UIGroupAppear worldEditGroup;

	public Image areaRectImage;
	public RectTransform framesHolderT;
	public RectTransform areaRectT;

	public WUI_EquipmentSlotFrame slotFramePrefab;

	private Vector3[] areaWorldCorners = new Vector3[4];

	private ChangeCheck<bool> areaIsGlowing = new ChangeCheck<bool>(false);
	private float glowStartTime;

	private Dictionary<ItemButton, Equipment> spawnedEquipment = new Dictionary<ItemButton, Equipment>();
	private ObjectListSpawner<WUI_EquipmentSlotFrame> slotFrames;

	public void Init() {
		slotFrames = new ObjectListSpawner<WUI_EquipmentSlotFrame>(SlotFrame_Create, SlotFrame_Enable, SlotFrame_Update);
	}

    private void Update() {
        if(areaIsGlowing.CheckIsDirtyAndClear()) {
			if (areaIsGlowing.value) {
				areaRectT.gameObject.SetActive(true);
				glowStartTime = Time.time;
			} else {
				areaRectT.gameObject.SetActive(false);
			}
        }

		if(areaIsGlowing.value) {
			areaRectImage.color = Color.white.SetA(MMath.SinAnimation(glowStartTime, Time.time, 4f, 0.0f, 0.4f));
		}
    }

    public void Show(bool show) {
		worldEditGroup.Show(show);
	}

	public void SetAreaGlowing(bool glowing) {
		areaIsGlowing.Set(glowing);
	}

	public void SetTargetHero(Hero hero) {
		targetHero = hero;

		GenerateFrames();
    }

	private void GenerateFrames() {
		slotFrames.Update(targetHero.heroEquipment.slots.Count);

        for (int i = 0; i < targetHero.heroEquipment.slots.Count; i++) {
			EquipmentSlot slot = targetHero.heroEquipment.slots[i];
			WUI_EquipmentSlotFrame frame = slotFrames[i];

			frame.equipmentSlot = slot;
        }
    }

	private WUI_EquipmentSlotFrame SlotFrame_Create(int id) {
		WUI_EquipmentSlotFrame newFrame = Instantiate(slotFramePrefab, framesHolderT);
		newFrame.Init();
		return newFrame;
    }

	private void SlotFrame_Enable(WUI_EquipmentSlotFrame frame, int id, bool enable) {
		frame.gameObject.SetActive(enable);
    }

	private void SlotFrame_Update(WUI_EquipmentSlotFrame frame, int id) {

	}

	public Equipment EquipTempItem(ItemButton button, WUI_EquipmentSlotFrame slot) {
		Equipment newEquipment = Game.inst.equipmentFactory.Create(button.currentItem.info, Vector2.zero);

		slot.tempItemButton = button;
		targetHero.heroEquipment.AttachEquipment(newEquipment, slot.equipmentSlot);
		//spawnedEquipment[button] = newEquipment;

		return newEquipment;
	}

	public void UnequipTempItem(ItemButton button) {
        for (int i = 0; i < slotFrames.activeObjectsCount; i++) {
			if(slotFrames[i].tempItemButton == button) {
				Debug.Log("UnequipTempItem " + button.currentItem.itemType);
				targetHero.heroEquipment.UnattachEquipment(slotFrames[i].equipmentSlot);
				slotFrames[i].tempItemButton = null;
            }
        }
	}

	public void UnequipItem(ItemButton button) {
		for (int i = 0; i < slotFrames.activeObjectsCount; i++) {
			if (slotFrames[i].itemButton == button) {
				Debug.Log("UnequipItem " + button.currentItem.itemType);
				targetHero.heroEquipment.UnattachEquipment(slotFrames[i].equipmentSlot);
				slotFrames[i].itemButton = null;
				slotFrames[i].tempItemButton = null;
			}
		}
	}

	public Equipment EquipItem(ItemButton button, WUI_EquipmentSlotFrame slot) {
		Equipment newEquipment = Game.inst.equipmentFactory.Create(button.currentItem.info, Vector2.zero);

		slot.itemButton = button;
		targetHero.heroEquipment.AttachEquipment(newEquipment, slot.equipmentSlot);
		//spawnedEquipment[button] = newEquipment;

		return newEquipment;
		/*
		Equipment newEquipment = Game.inst.equipmentFactory.Create(button.currentItem.info, Vector2.zero);
		targetHero.heroEquipment.CanAttachEquipment(newEquipment);
		spawnedEquipment[button] = newEquipment;

		return newEquipment;
		*/
    }

	public bool CheckInsideDropArea() {
		Vector2 worldPos = MCamera.GetWorldPos2D(Input.mousePosition);
		areaRectT.GetWorldCorners(areaWorldCorners);

		bool isInside = MUtils.PointIsInsideCorners(worldPos, areaWorldCorners);

		return isInside;
	}

	public void ReturnItemButtonToFrame(ItemButton itemButton) {
		WUI_EquipmentSlotFrame frame = slotFrames.objects.Find(x => x.itemButton == itemButton);

		if(frame != null) {
			itemButton.AlignToWorldFrame(frame);
		}		
	}

	public TryPlaceItemResult TryPlaceItemButton(ItemButton button) {
		TryPlaceItemResult result = new TryPlaceItemResult();
		Debug.Log("TryPlaceItemButton");

        for (int i = 0; i < slotFrames.activeObjectsCount; i++) {
			SlotPlaceResult framePlaceResult = slotFrames[i].equipmentSlot.CheckPlace(button.currentItem);

			Debug.Log("slotFrameTry: i " + i+ " " + framePlaceResult );

			if ((int)framePlaceResult > (int)result.placeResult) {
				result.placeResult = framePlaceResult;
				result.slot = slotFrames[i];
			}
        }

		return result;
    }

	public void PlaceItemButton(ItemButton itemButton, TryPlaceItemResult placeResult) {
		placeResult.slot.itemButton = itemButton;
		itemButton.AlignToWorldFrame(placeResult.slot);
	}

	public void RemoveItemButton(ItemButton itemButton, bool clearItemButton) {
		WUI_EquipmentSlotFrame frame = slotFrames.objects.Find(x => x.itemButton == itemButton);		
		itemButton.AlignToItemList();
		if (clearItemButton) {
			frame.itemButton = null;
		}
	}

	public class TryPlaceItemResult {
		public SlotPlaceResult placeResult;
		public WUI_EquipmentSlotFrame slot;
    }

}

public enum SlotPlaceResult {
	None = 0,
	CanNotPlace = 1 << 0,
	CanReplace = 1 << 1,
	CanPlace = 1 << 2,	
}
