using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "BeanArena/PlayerInput")]
public class SO_PlayerInput : ScriptableObject {

    public Action<UIJoystickEventData> OnMoveInput;

    public void OnJoystickInput(UIJoystickEventData data) {
        OnMoveInput?.Invoke(data);
    }

}
