using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {

    public static GameUI inst;
    public static UICanvas canvas => inst.m_Canvas;

    public SO_PlayerInput playerInput;

    public UISimpleButton[] abilityButtons;

    [SerializeField] private UICanvas m_Canvas;

	public void Init() {
        inst = this;

        for (int i = 0; i < abilityButtons.Length; i++) {
            abilityButtons[i].SetOnClick(i, (x) => playerInput.TriggerOnButtonInput(new ButtonInputEventData() {
                buttonID = x
            }));
        }

        MGameLoop.Update.Register(InternalUpdate);
    }

    public void InternalUpdate() {
        if(Input.GetKeyDown(KeyCode.D)) {
            playerInput.TriggerOnButtonInput(new ButtonInputEventData() { buttonID = 0 });
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            playerInput.TriggerOnButtonInput(new ButtonInputEventData() { buttonID = 1 });
        }
    }

}
