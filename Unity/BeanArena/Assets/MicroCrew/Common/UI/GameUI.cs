using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {

    public static GameUI inst;
    public static UICanvas canvas => inst.m_Canvas;

    public GameObject rootGO;

    public SO_PlayerInput playerInput;

    public PlayerPanel[] playerPanels;
    public UIChargableButton chargeableButton;
    public UISimpleButton[] abilityButtons;

    [SerializeField] private UICanvas m_Canvas;

	public void Init() {
        inst = this;

        chargeableButton.OnOutput += (chargeOutput) => {
            playerInput.TriggerOnButtonInput(new ButtonInputEventData(0, chargeOutput.chargePercent));
        };

        for (int i = 0; i < abilityButtons.Length; i++) {
            abilityButtons[i].SetOnClick(i, (x) => playerInput.TriggerOnButtonInput(new ButtonInputEventData() {
                buttonID = (int)x
            }));
        }

        for (int i = 0; i < playerPanels.Length; i++) {
            playerPanels[i].Init();
        }

        MGameLoop.Update.Register(InternalUpdate);
        HeroDamageEvent.Register(OnHeroDamageEvent);
    }

    public void InternalUpdate() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            chargeableButton.OnPointerDown(null);
        }

        if (Input.GetKeyUp(KeyCode.Space)) {
            chargeableButton.OnPointerUp(null);
            chargeableButton.OnPointerClick(null);
        }

        /*
        if (Input.GetKeyDown(KeyCode.D)) {
            playerInput.TriggerOnButtonInput(new ButtonInputEventData() { buttonID = 0 });
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            playerInput.TriggerOnButtonInput(new ButtonInputEventData() { buttonID = 1 });
        }
        */
    }

    public void Show(bool show) {
        rootGO.SetActive(show);

        if (show) { 
            
        } else {

        }
    }

    public void ShowGameEndScreen() {

    }

    public void OnRoundStart() {

    }

    public void OnHeroDamageEvent(HeroDamageEvent e) {
        Debug.Log("HeroDamageEvent");

        for (int i = 0; i < playerPanels.Length; i++) {
            if(playerPanels[i].attachedHero == e.hero) {
                playerPanels[i].healthbar.SetValue(e.hero.info.health / (float)e.hero.info.maxHealth, false);
                Debug.Log("Damage 123");
            }
        }
    }

    public void RestartGame() {
        GameMode.current.ExitGame();
    }

}
