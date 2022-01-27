using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SO_Equipment : ScriptableObject {
	[Header("Eqiupment")]
	public EquipmentCategory category;
	public Sprite uiIcon;
	public string equipName_LKey;
	public string equipDescr_LKey;
}

public enum EquipmentCategory {
	None,
	Weapon,
	Boots
}