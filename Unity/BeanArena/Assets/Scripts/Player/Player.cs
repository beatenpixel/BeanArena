using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public Hero hero { get; private set; }

	public SO_PlayerInput playerInput;

	public void Init() {
		playerInput.OnMoveInput += MoveJoystickInput;
		playerInput.OnArmInput += ArmJoystickInput;
		playerInput.OnButtonInput += ButtonInput;
	}
	
	public void InternalStart() {
		
	}
	
	public void InternalUpdate() {
		if(MInput.inputType == MInput.InputType.PC) {
			Vector2 keyboardMoveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));			
			//hero.MoveInput(Vector2.ClampMagnitude(keyboardMoveInput, 1f));
        }
	}
	
	public void InternalLateUpdate() {
		
	}
	
	public void InternalFixedUpdate() {
		
	}

	public void AssignHero(Hero _hero) {
		hero = _hero;
	}

	private void MoveJoystickInput(UIJoystickEventData e) {
		if (e.type == UIJoystickEventData.EventType.Cancel || e.type == UIJoystickEventData.EventType.End) {
			hero.MoveInput(Vector2.zero);
		} else {
			hero.MoveInput(e.value);
		}
    }

	private void ArmJoystickInput(UIJoystickEventData e) {
		if (e.type == UIJoystickEventData.EventType.Cancel || e.type == UIJoystickEventData.EventType.End) {
			hero.ArmInput(Vector2.zero);
		} else {
			hero.ArmInput(e.value);
		}
	}

	private void ButtonInput(ButtonInputEventData e) {
		hero.ButtonInput(e);
	}

}
