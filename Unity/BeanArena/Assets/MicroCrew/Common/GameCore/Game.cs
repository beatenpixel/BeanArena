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

		equipmentFactory.Init();
		heroFactory.Init();

		map.Init();

		SpawnHeroes();
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

		for (int i = 0; i < 1; i++) {
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

			MCamera.inst.AddTarget(new CameraTarget(enemy.hero.t, new Vector2(0, -2), Vector2.one * 2));
		}

		MCamera.inst.AddTarget(new CameraTarget(player.hero.t, new Vector2(0, -2), Vector2.one * 2));

		ITarget enemyTarget = map.GetClosestTarget(playerHero.GetPosition(), (t) => t != (ITarget)playerHero, out TargetAimPoint aimPoint);
		playerHero.SetTarget(enemyTarget, aimPoint);

		Sword sword = (Sword)equipmentFactory.Create(new WeaponConfig(WeaponType.Sword), Vector2.zero);
		playerHero.AttachEquipment(sword);

		Pistol pistol = (Pistol)equipmentFactory.Create(new WeaponConfig(WeaponType.Pistol), Vector2.zero);
		playerHero.AttachEquipment(pistol);
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
