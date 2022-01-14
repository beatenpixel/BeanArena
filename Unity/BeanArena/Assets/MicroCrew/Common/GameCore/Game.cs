using MicroCrew.Economy;
using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : Singleton<Game> {

	public static GD_Game data;

	public HeroFactory heroFactory;
	public EquipmentFactory equipmentFactory;

	public Player player;

	[HideInInspector] public Map map;
	[HideInInspector] public GameMode gameMode;

	private bool didSetupGame;

	public override void Init() {
		MSceneManager.OnSceneChangeEnd.Add(-1000,OnSceneLoadEnd);

		HeroDieEvent.Register(GameEvent_HeroDie);
	}

	protected override void Shutdown() {

	}

	private void GameEvent_HeroDie(HeroDieEvent e) {
		if(e.hero == player.hero) {
			FX.inst.EnableDeathScreenEffect(true);
        }
    } 

	public void SetupGame() {
		if (didSetupGame)
			return;

		MAssets.InitIfNeeded(null);
		GameDataManager.InitIfNeeded(null);
		data = GameDataManager.inst.Load();

		Economy.InitIfNeeded(null);

		MGameLoop.Update.Register(InternalUpdate);
		MGameLoop.FixedUpdate.Register(InternalFixedUpdate);

		MAppUI.InitIfNeeded(null);
		MUI.InitIfNeeded(null);

		equipmentFactory.Init();
		heroFactory.Init();

		didSetupGame = true;
	}

	private void StartGameLogic() {
		SetupGame();

		map.Init();

		gameMode.InitGame(this);
		gameMode.StartGame();

		FX.inst.EnableDeathScreenEffect(false);
	}	

	private void OnSceneLoadEnd(SceneEvent e) {
		Debug.Log("LoadScene");

		gameMode = FindObjectOfType<GameMode>();
		map = FindObjectOfType<Map>();

		StartGameLogic();
    }

	public void InternalUpdate() {
		if(map != null) {
			map.InternalUpdate();
        }

		if(gameMode != null) {
			gameMode.InternalUpdate();
        }

		player.InternalUpdate();

		if(Input.GetKeyDown(KeyCode.R)) {
			MSceneManager.ReloadScene();
        }
	}

	public void InternalFixedUpdate() {
		if (map != null) {
			map.InternalFixedUpdate();
		}

		if (gameMode != null) {
			gameMode.InternalFixedUpdate();
		}
	}

	public static int TeamIDToLayer(int teamID) {
		return LayerMask.NameToLayer("team" + teamID);
    }

	public static int TeamIDToOnlyBeanLayer(int teamID) {
		return LayerMask.NameToLayer("team" + teamID + "_beanonly");
	}

}
