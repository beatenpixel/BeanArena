using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

	public SO_Weapon weaponSO;

	public void Init() {
		
	}
	
	public void InternalStart() {
		
	}
	
	public void InternalUpdate() {
		
	}
	
	public void InternalLateUpdate() {
		
	}
	
	public void InternalFixedUpdate() {
		
	}
	
}

public enum WeaponCategory {
	None,
	Melee,
	Gun
}

public enum WeaponType {
	None,
	Pistol
}
