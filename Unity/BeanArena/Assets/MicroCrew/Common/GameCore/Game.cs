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

	[HideInInspector] public Map map;
	public Player player;

	public List<Enemy> enemies;

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

		didSetupGame = true;
	}

	private void StartGameLogic() {
		SetupGame();

		equipmentFactory.Init();
		heroFactory.Init();

		map.Init();

		SpawnHeroes();

		FX.inst.EnableDeathScreenEffect(false);

		GameUI.inst.Show(true);
	}

	private void SpawnHeroes() {
		Hero playerHero = heroFactory.Create(new HeroConfig() {
			nickname = "Lorg",
			orientation = Orientation.Right,
			teamID = 0,
			role = HeroRole.Player,
		}, map.GetArea("PlayerSpawn").GetRandomPosition());

		player.AssignHero(playerHero);
		player.Init();

		map.AddHero(playerHero);

		enemies = new List<Enemy>();

		MCamera.inst.ClearTargets();

		int enemyCount = 1;

		for (int i = 0; i < enemyCount; i++) {
			Enemy enemy = new Enemy();

			Hero enemyHero = heroFactory.Create(new HeroConfig() {
				nickname = "Enemy " + (i + 1),
				orientation = Orientation.Left,
				teamID = 1,
				role = HeroRole.Enemy,
			}, map.GetArea("EnemySpawn").GetRandomPosition());

			enemy.AssignHero(enemyHero);
			enemy.Init();

			map.AddHero(enemyHero);

			enemies.Add(enemy);

			MCamera.inst.AddTarget(new CameraTarget(enemy.hero.body.transform, new Vector2(0, -2), Vector2.one * 2));

			Pistol epistol = (Pistol)equipmentFactory.Create(new WeaponConfig(WeaponType.Pistol), Vector2.zero);
			enemyHero.AttachEquipment(epistol);
		}

		MCamera.inst.AddTarget(new CameraTarget(player.hero.body.transform, new Vector2(0, -2), Vector2.one * 2));

		playerHero.FindClosestTarget();
        for (int i = 0; i < enemies.Count; i++) {
			enemies[i].hero.FindClosestTarget();
        }

		//Sword sword = (Sword)equipmentFactory.Create(new WeaponConfig(WeaponType.Sword), Vector2.zero);
		//playerHero.AttachEquipment(sword);

		//Pistol waterPistol = (Pistol)equipmentFactory.Create(new WeaponConfig(WeaponType.WaterPistol), Vector2.zero);
		//playerHero.AttachEquipment(waterPistol);

		Pistol pistol2 = (Pistol)equipmentFactory.Create(new WeaponConfig(WeaponType.Pistol), Vector2.zero);
		playerHero.AttachEquipment(pistol2);

		GameUI.inst.playerPanels[0].SetHero(playerHero);
		GameUI.inst.playerPanels[1].SetHero(enemies[0].hero);
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

		player.InternalUpdate();

        for (int i = enemies.Count - 1; i >= 0; i--) {
			enemies[i].InternalUpdate();
        }

		if(Input.GetKeyDown(KeyCode.R)) {
			MSceneManager.ReloadScene();
        }
	}

	public void InternalFixedUpdate() {
		if (map != null) {
			map.InternalFixedUpdate();
		}
	}

	public static int TeamIDToLayer(int teamID) {
		return LayerMask.NameToLayer("team" + teamID);
    }

	public static int TeamIDToOnlyBeanLayer(int teamID) {
		return LayerMask.NameToLayer("team" + teamID + "_beanonly");
	}

}
