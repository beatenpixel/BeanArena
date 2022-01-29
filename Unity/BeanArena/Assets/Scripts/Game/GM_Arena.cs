using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_Arena : GameMode {

    public override bool StartGame() {

		SpawnHeroes();

		GameUI.inst.Show(true);

		return base.StartGame();
    }

    private void SpawnHeroes() {
		Hero playerHero = heroFactory.Create(new HeroConfig() {
			nickname = "Lorg",
			orientation = Orientation.Right,
			teamID = 0,
			role = HeroRole.Player,
		}, genericMap.GetArea("PlayerSpawn").GetRandomPosition());

		player.AssignHero(playerHero);
		player.Init();

		genericMap.AddHero(playerHero);

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
			}, genericMap.GetArea("EnemySpawn").GetRandomPosition());

			enemy.AssignHero(enemyHero);
			enemy.Init();

			genericMap.AddHero(enemyHero);

			enemies.Add(enemy);

			MCamera.inst.AddTarget(new CameraTarget(enemy.hero.body.transform, new Vector2(0, -2), Vector2.one * 2));

			//Pistol epistol = (Pistol)equipmentFactory.Create(MAssets.itemsInfo.GetAsset(ItemType.Pistol), Vector2.zero);
			//enemyHero.heroEquipment.CanAttachEquipment(epistol);
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

		//Pistol pistol2 = (Pistol)equipmentFactory.Create(MAssets.itemsInfo.GetAsset(ItemType.Pistol), Vector2.zero);
		//playerHero.heroEquipment.CanAttachEquipment(pistol2);

		GameUI.inst.playerPanels[0].SetHero(playerHero);
		GameUI.inst.playerPanels[1].SetHero(enemies[0].hero);
	}

}