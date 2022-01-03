using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "BeanArena/PlayerInput")]
public class SO_PlayerInput : ScriptableObject {

    public Action<UIJoystickEventData> OnMoveInput;
    public Action<UIJoystickEventData> OnArmInput;

    public void OnJoystickInput(UIJoystickEventData data) {
        OnMoveInput?.Invoke(data);
    }

    public void OnArmJoystickInput(UIJoystickEventData data) {
        OnArmInput?.Invoke(data);
    }

}
