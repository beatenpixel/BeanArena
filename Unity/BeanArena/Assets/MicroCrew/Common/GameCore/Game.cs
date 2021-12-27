using MicroCrew.Economy;
using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : Singleton<Game> {

	public static GD_Game data;

	public override void Init() {
		
	}

	protected override void Shutdown() {

	}

	public void StartGame() {
		MAssets.InitIfNeeded(null);
		GameDataManager.InitIfNeeded(null);
		data = GameDataManager.inst.Load();

		Economy.InitIfNeeded(null);

		MGameLoop.Update.Register(InternalUpdate);

		MAppUI.InitIfNeeded(null);
		MUI.InitIfNeeded(null);

		StartGameLogic();
	}

	private void StartGameLogic() {

    }

	public void InternalUpdate() {
		
	}

}
