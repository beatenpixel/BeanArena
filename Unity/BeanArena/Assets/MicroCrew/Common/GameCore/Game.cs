using MicroCrew.Economy;
using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : Singleton<Game> {

	public static GD_Game data;

	[HideInInspector] public Map map;
	public Player player;

	private bool didSetupGame;

	public override void Init() {
		MSceneManager.OnSceneChangeEnd.Add(-1000,OnSceneLoadEnd);
	}

	protected override void Shutdown() {

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

		didSetupGame = true;
	}

	private void StartGameLogic() {
		SetupGame();

		map.Init();

		player.Init();
    }

	private void OnSceneLoadEnd(SceneEvent e) {
		Debug.Log("LoadScene");

		if(e.next.name != "menu") {
			map = FindObjectOfType<Map>();

			StartGameLogic();
        }
    }

	public void InternalUpdate() {
		if(map != null) {
			map.InternalUpdate();
        }
	}

	public void InternalFixedUpdate() {
		if (map != null) {
			map.InternalFixedUpdate();
		}
	}

}
