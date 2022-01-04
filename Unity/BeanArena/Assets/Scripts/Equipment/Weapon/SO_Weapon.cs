using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BeanArena/WeaponData")]
public class SO_Weapon : SO_Equipment {
	[Header("Weapon")]
	public WeaponCategory weaponCategory;
	public WeaponType weaponType;
}
