using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : Singleton<Game> {

	public static GD_Game data;

    [SerializeField] private BeanNetwork beanNetwork;
	public HeroFactory heroFactory;
	public EquipmentFactory equipmentFactory;

	[HideInInspector] public Map map;
	[HideInInspector] public GameMode gameMode;

    public static ArenaLoadOptions arenaLoadOptions = new ArenaLoadOptions() {
        vsType = GameModeVSType.None
    };

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

        MServices.InitIfNeeded(null);

		equipmentFactory.Init();
		heroFactory.Init();

        BeanNetwork.InitIfNeeded(null);

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
        Debug.Log("OnSceneLoadEnd " + arenaLoadOptions.vsType);

        if (arenaLoadOptions.vsType == GameModeVSType.Bot) {
            GameObject gameModeGO = new GameObject("[GM_ArenaBot]");
            gameMode = gameModeGO.AddComponent<GM_ArenaBot>();
        } else if (arenaLoadOptions.vsType == GameModeVSType.Local) {
            GameObject gameModeGO = new GameObject("[GM_ArenaLocal]");
            gameMode = gameModeGO.AddComponent<GM_ArenaLocal>();
        } else if (arenaLoadOptions.vsType == GameModeVSType.Online) {
            GameObject gameModeGO = new GameObject("[GM_ArenaOnline]");
            gameMode = gameModeGO.AddComponent<GM_ArenaOnline>();
        } else {
            gameMode = FindObjectOfType<GM_Menu>();
        }

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

        beanNetwork.InternalUpdate();

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

        beanNetwork.InternalFixedUpdate();
	}

	public static int TeamIDToLayer(int teamID) {
		return LayerMask.NameToLayer("team" + teamID);
    }

    public static int TeamIDToBeanLayer(int teamID) {
        return LayerMask.NameToLayer("bean_team" + teamID);
    }

}

public class ArenaLoadOptions {
    public GameModeVSType vsType;
}

public enum GameModeVSType {
    None,
    Bot,
    Local,
    Online
}