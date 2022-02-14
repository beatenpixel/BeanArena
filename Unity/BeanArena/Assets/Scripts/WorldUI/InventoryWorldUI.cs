using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWorldUI : MonoBehaviour {

	public HeroBase targetHero;

	public UIGroupAppear worldEditGroup;

	public Image areaRectImage;
	public RectTransform framesHolderT;
	public RectTransform areaRectT;

	public WorldItemSlot slotFramePrefab;

	private Vector3[] areaWorldCorners = new Vector3[4];

	private ChangeCheck<bool> areaIsGlowing = new ChangeCheck<bool>(false);
	private float glowStartTime;

	private ObjectListSpawner<WorldItemSlot> worldSlots;

	public void Init() {
		worldSlots = new ObjectListSpawner<WorldItemSlot>(SlotFrame_Create, SlotFrame_Enable, SlotFrame_Update, SlotFrame_Destroy);
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

	public void SetTargetHero(HeroBase hero) {
		targetHero = hero;

		GenerateFrames();
    }

	private void GenerateFrames() {
        Debug.Log("Generate Frames: " + targetHero.heroEquipment.GetSlotsCount());

		worldSlots.Update(targetHero.heroEquipment.GetSlotsCount());

        for (int i = 0; i < targetHero.heroEquipment.GetSlotsCount(); i++) {
			worldSlots[i].Init(targetHero.heroEquipment.GetEquipmentSlot(i));
        }
    }

	private WorldItemSlot SlotFrame_Create(int id) {
		WorldItemSlot newFrame = Instantiate(slotFramePrefab, framesHolderT);
		return newFrame;
    }

	private void SlotFrame_Enable(WorldItemSlot frame, int id, bool enable) {
		frame.gameObject.SetActive(enable);
    }

	private void SlotFrame_Update(WorldItemSlot frame, int id) {
        
    }

    private void SlotFrame_Destroy(WorldItemSlot frame, int id) {

    }

    public WorldItemSlot GetWorldSlot(ItemButton item) {
		for (int i = 0; i < worldSlots.activeObjectsCount; i++) {
			if (worldSlots[i].CompareItemButton(item)) {
				return worldSlots[i];
			}
		}

		return null;
	}

	public WorldItemSlot GetWorldSlot(EquipmentSlot heroSlot) {
        for (int i = 0; i < worldSlots.activeObjectsCount; i++) {
			if(worldSlots[i].CompareHeroEquipmentSlots(heroSlot)) {
				return worldSlots[i];
            }
        }

		return null;
    }

	public bool CheckInsideDropArea() {
		Vector2 worldPos = MCamera.GetWorldPos2D(Input.mousePosition);
		areaRectT.GetWorldCorners(areaWorldCorners);

		bool isInside = MUtils.PointIsInsideCorners(worldPos, areaWorldCorners);

		return isInside;
	}

}