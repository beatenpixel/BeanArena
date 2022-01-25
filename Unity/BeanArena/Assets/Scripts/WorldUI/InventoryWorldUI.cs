using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWorldUI : MonoBehaviour {

	public UIGroupAppear worldEditGroup;
	public List<ItemButtonWorldFrame> buttonFrames;

	public Image areaRectImage;
	public RectTransform areaRectT;
	private Vector3[] areaWorldCorners = new Vector3[4];

	private ChangeCheck<bool> areaIsGlowing = new ChangeCheck<bool>(false);
	private float glowStartTime;

	public void Init() {
		
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

	public bool CheckInsideDropArea() {
		Vector2 worldPos = MCamera.GetWorldPos2D(Input.mousePosition);
		areaRectT.GetWorldCorners(areaWorldCorners);

		bool isInside = MUtils.PointIsInsideCorners(worldPos, areaWorldCorners);


		return isInside;
	}

	public void PlaceItemButton(ItemButton itemButton, ItemCategory category) {
		ItemButtonWorldFrame frame = buttonFrames.Find(x => x.category == category);

		if(frame.button != null) {
			itemButton.AlignToWorldFrame(frame);
        }
	}
	
}

[System.Serializable]
public class ItemButtonWorldFrame {
	public ItemCategory category;
	public RectTransform rectT;
	[HideInInspector] public ItemButton button;
}
