using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BeanArena/WeaponData")]
public class SO_Weapon : ScriptableObject {
	public WeaponCategory category;
	public WeaponType type;
	public Sprite uiIcon;
	public string weaponName_LKey;
	public string weaponDescr_LKey;
}
