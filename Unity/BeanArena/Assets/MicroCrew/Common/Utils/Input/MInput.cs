using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class MInput {

    public static InputType inputType;

    public static bool inputAllowed {
        get {
            return _inputAllowed;
        }
    }

    private static bool _inputAllowed;
    public static Dictionary<PlayerInputAllowFlag, bool> inputAllowedFlags = new Dictionary<PlayerInputAllowFlag, bool>() {
        { PlayerInputAllowFlag.DEV_CHEAT, true },
        { PlayerInputAllowFlag.IN_GAME_NOTIFIACTION, true },
        { PlayerInputAllowFlag.UI_WINDOW, true }
    };

    private static List<RaycastResult> raycastResults = new List<RaycastResult>();

    static MInput() {
        CalculateInputAllowed();
    }

    public static void SetInputAllowed(PlayerInputAllowFlag key, bool allow) {
        inputAllowedFlags[key] = allow;
        CalculateInputAllowed();
    }

    private static void CalculateInputAllowed() {
        _inputAllowed = true;
        foreach (var item in inputAllowedFlags) {
            if (!item.Value) {
                _inputAllowed = false;
                break;
            }
        }
    }

    #region Input

    public static Vector2 GetWalkInput() {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public static Vector2 GetMouseDeltaInput() {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    #endregion

    public static FuncTick CreateMouseAction(string funcName, int mouseButton, InputAction inputAction, Action<Vector2> onMouse, Func<bool> destroyFunc = null) {
        return MFunc.StartTick(funcName, FuncUpdateType.UPDATE, () => {
            bool isInput = false;

            switch (inputAction) {
                case InputAction.DOWN:
                    isInput |= Input.GetMouseButtonDown(mouseButton);
                    break;
                case InputAction.UP:
                    isInput |= Input.GetMouseButtonUp(mouseButton);
                    break;
                case InputAction.HOLD:
                    isInput |= Input.GetMouseButton(mouseButton);
                    break;
            }

            if (isInput) {
                onMouse(Input.mousePosition);
            }
        }, destroyFunc);
    }

    public static FuncTick CreateKeyCodeAction(string funcName, KeyCode keyCode, InputAction inputAction, Action onKey, Func<bool> destroyFunc = null) {
        return MFunc.StartTick(funcName, FuncUpdateType.UPDATE, () => {
            bool isInput = false;

            switch (inputAction) {
                case InputAction.DOWN:
                    isInput |= Input.GetKeyDown(keyCode);
                    break;
                case InputAction.UP:
                    isInput |= Input.GetKeyUp(keyCode);
                    break;
                case InputAction.HOLD:
                    isInput |= Input.GetKey(keyCode);
                    break;
            }

            if (isInput) {
                onKey();
            }
        }, destroyFunc);
    }

    public static bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        EventSystem.current.RaycastAll(eventDataCurrentPosition, raycastResults);
        return raycastResults.Count > 0;
    }

    public static void ToggleCursorLock() {
        if (Cursor.lockState == CursorLockMode.Locked) {
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public enum InputAction {
        DOWN,
        UP,
        HOLD
    }

    public enum PlayerInputAllowFlag {
        NONE,
        DEV_CHEAT,
        IN_GAME_NOTIFIACTION,
        UI_WINDOW
    }

    public enum InputType {
        None,
        PC,
        Mobile
    }

}
