using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public Hero hero;

	public SO_PlayerInput playerInput;

	public void Init() {
		playerInput.OnMoveInput += MoveJoystickInput;
	}
	
	public void InternalStart() {
		
	}
	
	public void InternalUpdate() {
		
	}
	
	public void InternalLateUpdate() {
		
	}
	
	public void InternalFixedUpdate() {
		
	}

	private void MoveJoystickInput(UIJoystickEventData e) {
		if (e.type == UIJoystickEventData.EventType.Cancel || e.type == UIJoystickEventData.EventType.End) {
			hero.MoveInput(Vector2.zero);
		} else {
			hero.MoveInput(e.value);
		}
    }
	
}
