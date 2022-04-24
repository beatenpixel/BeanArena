using IngameDebugConsole;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_Menu : GameMode {

	public static GM_Menu inst;

	public MenuState menuState;

	private Map_Menu map;

    public HeroBase previewHero { get; private set; }

    public override void InitGame(Game game) {
        base.InitGame(game);
		inst = this;

		map = (Map_Menu)genericMap;
    }

    public override bool StartGame() {

		SpawnPreviewHero();

		GameUI.inst.Show(false);
		MenuUI.inst.Show(true);

        MCamera.inst.SetFixedArea(new Vector2(0, 0), new Vector2(10, 8), ScreenMatchType.Vertical, true);

        return base.StartGame();
    }

    public override void InternalUpdate() {
        base.InternalUpdate();
	}

    public void GoToFight() {
        Game.arenaLoadOptions = new ArenaLoadOptions() {
            vsType = GameModeVSType.Bot
        };

        genericMap.OnGameExit();
        SceneTransitionManager.inst.LoadLevelAsync(MSceneManager.ARENA_SCENE_NAME);
    }

    public void GoFightLocal() {
        Game.arenaLoadOptions = new ArenaLoadOptions() {
            vsType = GameModeVSType.Local
        };

        Debug.Log("GoFight: " +  Game.arenaLoadOptions.vsType);

        genericMap.OnGameExit();
        SceneTransitionManager.inst.LoadLevelAsync(MSceneManager.ARENA_SCENE_NAME);
    }

    public void GoFightOnline() {
        Game.arenaLoadOptions = new ArenaLoadOptions() {
            vsType = GameModeVSType.Online
        };

        Debug.Log("GoFight: " + Game.arenaLoadOptions.vsType);

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

    public void SpawnPreviewHero() {
        Debug.Log("GM_Menu:Spawn");

        Player player = new Player();

        GD_HeroItem equipedHero = Game.data.GetEquipedHero();

        previewHero = heroFactory.Create(new HeroConfig() {
			nickname = "Lorg",
			orientation = Orientation.Right,
			teamID = 0,
			role = HeroRole.Player,
            heroType = equipedHero.heroType,
            heroData = Game.data.inventory.heroes.Find(x => x.heroType == equipedHero.heroType)
        }, genericMap.GetArea("PlayerSpawn").GetRandomPosition());

		player.AssignHero(previewHero);
		player.Init();

		genericMap.AddHero(previewHero);

		MenuUI.inst.inventoryDrawer.worldUI.SetTargetHero(previewHero);

        previewHero.heroEquipment.LoadEquipmentFromGameData();
	}

    public void DestroyPreviewHero() {
        genericMap.RemoveHero(previewHero);
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

	public virtual void InitGame(Game game) {
		current = this;

		heroFactory = game.heroFactory;
		equipmentFactory = game.equipmentFactory;
		genericMap = game.map;

		state = GameModeState.Ready;

        HeroDieEvent.Register(OnGameEvent_HeroDie);
    }

	public virtual bool StartGame() {
		state = GameModeState.Running;

        heroesInputAllowed = true;

        return true;
	}

	public virtual void InternalUpdate() {
		
	}

	public virtual void InternalFixedUpdate() {

    }

	public virtual void PauseGame() {
		state = GameModeState.Paused;
    }

	public virtual void ExitGame() {
        OnPreExitGame();

        Game.arenaLoadOptions.vsType = GameModeVSType.None;
        HeroDieEvent.Unregister(OnGameEvent_HeroDie);
        genericMap.OnGameExit();
        SceneTransitionManager.inst.LoadLevelAsync(MSceneManager.MENU_SCENE_NAME);
    }

    protected virtual void OnPreExitGame() {

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
