using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemSlot : MonoBehaviour {

	[SerializeField] private RectTransform rectT;

	private EquipmentSlot heroEquipmentSlot;
    private ItemButton currentItemButton;

	public void Init(EquipmentSlot heroEquipmentSlot) {
		this.heroEquipmentSlot = heroEquipmentSlot;
    }

    public void SetItemButton(ItemButton button) {
        currentItemButton = button;

        button.rectT.SetParent(rectT);
        button.rectT.localScale = Vector3.one;
        button.subRectT.localScale = Vector3.one;

        button.rectT.SetAnchor(Vector2.zero, Vector2.one);
        button.rectT.SetOffset(Vector2.zero, Vector2.zero);

        button.SetDragDirection(Orientation.Left);
        button.SetState(ItemButton.ItemButtonState.InHeroSlot);
    }

    public void ClearItemButton() {
        currentItemButton.SetState(ItemButton.ItemButtonState.InInventory);
        currentItemButton.AlignToItemList();

        currentItemButton = null;        
    }

    public void ClearPreviewItem() {
        heroEquipmentSlot.ClearPreviewItem();
    }

    public bool CompareHeroEquipmentSlots(EquipmentSlot slot) {
        return heroEquipmentSlot == slot;
    }

    public bool CompareItemButton(ItemButton button) {
        return currentItemButton == button;
    }

}
