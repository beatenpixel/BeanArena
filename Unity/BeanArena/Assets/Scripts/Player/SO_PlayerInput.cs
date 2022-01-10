using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "BeanArena/PlayerInput")]
public class SO_PlayerInput : ScriptableObject {

    public Action<UIJoystickEventData> OnMoveInput;
    public Action<UIJoystickEventData> OnArmInput;
    public Action<ButtonInputEventData> OnButtonInput;

    public void OnJoystickInput(UIJoystickEventData data) {
        OnMoveInput?.Invoke(data);
    }

    public void OnArmJoystickInput(UIJoystickEventData data) {
        OnArmInput?.Invoke(data);
    }

    public void TriggerOnButtonInput(ButtonInputEventData data) {
        OnButtonInput?.Invoke(data);
    }

}

public struct ButtonInputEventData {
    public int buttonID;
    public bool isCharge;
    public float chargePercent;

    public ButtonInputEventData(int _buttonID) {
        isCharge = false;
        buttonID = _buttonID;
        chargePercent = -1;
    }

    public ButtonInputEventData(int _buttonID, float charge) {
        isCharge = true;
        buttonID = _buttonID;
        chargePercent = charge;
    }
}