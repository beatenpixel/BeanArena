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

	private MLog logger = new MLog("Game");
    private MAudioSource musicSource;

    public override void Init() {
        MSceneManager.OnSceneChangeEnd.Add(-1000, OnSceneLoadEnd);
    }

    protected override void Shutdown() {

	}

	public void SetupGame() {
		if (didSetupGame)
			return;

		MLog.Log("Start setting up game..." % MLogColor.Green, true);

		MAssets.InitIfNeeded(null);
		GameDataManager.InitIfNeeded(null);
		data = GameDataManager.inst.Load();

		Economy.InitIfNeeded(null);
        GameBalance.Init();

		MGameLoop.Update.Register(InternalUpdate);
		MGameLoop.FixedUpdate.Register(InternalFixedUpdate);

		MAppUI.InitIfNeeded(null);
		MUI.InitIfNeeded(null);

		equipmentFactory.Init();
		heroFactory.Init();

        this.Wait(() => {
            if (musicSource == null) {
                musicSource = MSound.Play("music_0", new SoundConfig() {
                    loop = true,
                    threeDimensional = false
                });
            }
        }, 3);

		didSetupGame = true;
	}

	private void StartGameLogic() {
		MLog.Log("StartGameLogic" % MLogColor.Green, true);

		SetupGame();

		map.Init();

		gameMode.InitGame(this);
		gameMode.StartGame();

		FX.inst.EnableDeathScreenEffect(false);        
	}	

	private void OnSceneLoadEnd(SceneEvent e) {		
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

    public static int TeamIDToBeanLayer(int teamID) {
        return LayerMask.NameToLayer("bean_team" + teamID);
    }

}