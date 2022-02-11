using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_Menu : GameMode {

	public static GM_Menu inst;

	public MenuState menuState;

	private Map_Menu map;

    public override void InitGame(Game game) {
        base.InitGame(game);
		inst = this;

		map = (Map_Menu)genericMap;
    }

    public override bool StartGame() {

		Spawn();

		GameUI.inst.Show(false);
		MenuUI.inst.Show(true);

		return base.StartGame();
    }

    public override void InternalUpdate() {
        base.InternalUpdate();
	}

    public void GoToFight() {
        genericMap.OnGameExit();
        SceneTransitionManager.inst.LoadLevelAsync(MSceneManager.ARENA_SCENE_NAME);
    }

	public void SwitchMenuState(MenuState newState) {
		menuState = newState;

		if(menuState == MenuState.CustomizingCharacter) {
			MCamera.inst.SetFixedArea(new Vector2(-3,0), new Vector2(7, 7), ScreenMatchType.Vertical, false);
		} else if(menuState == MenuState.Idle) {
			MCamera.inst.SetFixedArea(new Vector2(0, 0), new Vector2(10, 8), ScreenMatchType.Vertical, false);
		}
    }

    private void Spawn() {
        Debug.Log("GM_Menu:Spawn");

		Hero playerHero = heroFactory.Create(new HeroConfig() {
			nickname = "Lorg",
			orientation = Orientation.Right,
			teamID = 0,
			role = HeroRole.Player,
		}, genericMap.GetArea("PlayerSpawn").GetRandomPosition());

		player.AssignHero(playerHero);
		player.Init();

		genericMap.AddHero(playerHero);

		MCamera.inst.SetFixedArea(new Vector2(0, 0), new Vector2(10, 8), ScreenMatchType.Vertical, true);

		MenuUI.inst.inventoryDrawer.worldUI.SetTargetHero(playerHero);

        playerHero.heroEquipment.LoadEquipmentFromGameData();
	}

    public enum MenuState {
        Idle,
        CustomizingCharacter
    }

}

public abstract class GameMode : MonoBehaviour {

	public static GameMode current;

	public GameModeType type;
	public GameModeState state { get; private set; }

    public bool heroesInputAllowed;

	protected HeroFactory heroFactory;
	protected EquipmentFactory equipmentFactory;
	protected Map genericMap;
	protected Player player;

	public List<Enemy> enemies;

	public virtual void InitGame(Game game) {
		current = this;

		heroFactory = game.heroFactory;
		equipmentFactory = game.equipmentFactory;
		genericMap = game.map;
		player = game.player;

		state = GameModeState.Ready;

		enemies = new List<Enemy>();

        HeroDieEvent.Register(OnGameEvent_HeroDie);
    }

	public virtual bool StartGame() {
		state = GameModeState.Running;

        heroesInputAllowed = true;

        return true;
	}

	public virtual void InternalUpdate() {
		for (int i = enemies.Count - 1; i >= 0; i--) {
			enemies[i].InternalUpdate();
		}
	}

	public virtual void InternalFixedUpdate() {

    }

	public virtual void PauseGame() {
		state = GameModeState.Paused;
    }

	public virtual void ExitGame() {
        HeroDieEvent.Unregister(OnGameEvent_HeroDie);

        genericMap.OnGameExit();
        SceneTransitionManager.inst.LoadLevelAsync(MSceneManager.MENU_SCENE_NAME);
    }

    public virtual void RestartGame() {
        genericMap.OnGameExit();
        SceneTransitionManager.inst.LoadLevelAsync(MSceneManager.ARENA_SCENE_NAME);
    }

    protected virtual void OnGameEvent_HeroDie(HeroDieEvent e) {
        
    }

}

public class GameModeEvent : MGameEvent<GameModeEvent> {

	public GameModeEventType type;

	public GameModeEvent(GameModeEventType eventType) {
		type = eventType;
	}

	public enum GameModeEventType {
		OnReady,
		OnStart,
		OnFinish,
		OnExiting
    }
}

public enum GameModeState {
	None,
	Ready,
	Running,
	Paused,
	Finished
}

public enum GameModeType {
	None,
	Menu,
	Arena
}
