using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public HeroBase hero { get; private set; }

	public SO_PlayerInput playerInput;

	private Vector2 fakeMoveInput;

	public void Init() {
		//playerInput.OnMoveInput += MoveJoystickInput;
		playerInput.OnArmInput += MoveJoystickInput;
		playerInput.OnButtonInput += ButtonInput;
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

	public void AssignHero(HeroBase _hero) {
		hero = _hero;
	}

	private void MoveJoystickInput(UIJoystickEventData e) {
		if (e.type == UIJoystickEventData.EventType.Cancel || e.type == UIJoystickEventData.EventType.End) {
			//hero.ArmInput(Vector2.zero);

			fakeMoveInput = GetMoveInputByAim(e.value);
			//hero.MoveInput(fakeMoveInput);
			hero.MoveInput(Vector2.zero);

			ButtonInput(new ButtonInputEventData(0, e.value.magnitude));
		} else {
			hero.ArmInput(e.value);

			fakeMoveInput = GetMoveInputByAim(e.value);
			hero.MoveInput(fakeMoveInput);
		}

		/*
		if (e.type == UIJoystickEventData.EventType.Cancel || e.type == UIJoystickEventData.EventType.End) {
			//hero.MoveInput(Vector2.zero);
		} else {
			//hero.MoveInput(e.value);
		}
		*/
    }

	private void ArmJoystickInput(UIJoystickEventData e) {
		/*
		if (e.type == UIJoystickEventData.EventType.Cancel || e.type == UIJoystickEventData.EventType.End) {
			hero.ArmInput(Vector2.zero);
		} else {
			hero.ArmInput(e.value);
		}
		*/
	}

	private Vector2 GetMoveInputByAim(Vector2 aimInput) {
		float angle = Vector2.SignedAngle(Vector2.up, aimInput);
		return (Quaternion.Euler(0, 0, angle * 0.4f) * Vector3.up).normalized;
	}

	private void ButtonInput(ButtonInputEventData e) {
		hero.ButtonInput(e);
	}

}
