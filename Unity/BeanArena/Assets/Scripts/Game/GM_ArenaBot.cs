using IngameDebugConsole;
using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_ArenaBot : GameMode {

    public const int ROUNDS_TO_WIN = 2;

    public List<OpponentInfo> opponentsInfo;

    private bool isRespawningHeroes;

    protected Player player;
    protected List<Enemy> enemies;

    private JoystickInputUI joystickInput;

    public override void InitGame(Game game) {
        base.InitGame(game);

        opponentsInfo = new List<OpponentInfo>() {
            new OpponentInfo(),
            new OpponentInfo()
        };
    }

    public override bool StartGame() {

        if (Game.arenaLoadOptions.vsType == GameModeVSType.Bot) {
            SpawnHeroes_VsBot();
        } else if(Game.arenaLoadOptions.vsType == GameModeVSType.Local) {

        }

		GameUI.inst.Show(true);
        MenuUI.inst.Show(false);

        joystickInput = MPool.Get<JoystickInputUI>(null, GameUI.inst.gameModeStuffHolderT);
        joystickInput.OnEvent.AddListener((x) => {
            player.MoveJoystickInput(x);
        });

        joystickInput.joystickDrift = 0f;
        joystickInput.cancelRange = 0f;
        joystickInput.rectT.SetAnchor(Vector2.zero, Vector2.one);
        joystickInput.rectT.SetOffset(Vector2.zero, Vector2.zero);

        return base.StartGame();
    }

    protected override void OnPreExitGame() {
        base.OnPreExitGame();

        joystickInput.DestroyJoystick();
    }

    public override void InternalUpdate() {
        base.InternalUpdate();

        for (int i = 0; i < enemies.Count; i++) {
            enemies[i].InternalUpdate();
        }

        if(Input.GetKeyDown(KeyCode.Keypad7)) {
            foreach (var hero in genericMap.heroes) {
                if(hero.info.role == HeroRole.Player) {
                    hero.TakeDamage(new DevDamage());
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad8)) {
            foreach (var hero in genericMap.heroes) {
                if (hero.info.role != HeroRole.Player) {
                    hero.TakeDamage(new DevDamage());
                }
            }
        }
    }

    private void SpawnHeroes_VsBot() {
        GD_HeroItem equipedHero = Game.data.GetEquipedHero();

        player = new Player();

        HeroBase playerHero = heroFactory.Create(new HeroConfig() {
			nickname = "Lorg",
			orientation = Orientation.Right,
			teamID = 0,
			role = HeroRole.Player,
            heroType = equipedHero.heroType,
            heroData = Game.data.inventory.heroes.Find(x => x.heroType == equipedHero.heroType)
        }, genericMap.GetArea("PlayerSpawn").GetRandomPosition());

        player.AssignHero(playerHero);
        player.Init();
        player.hero.InitForUI(GameUI.inst.playerPanels[0]);
        player.hero.heroEquipment.LoadEquipmentFromGameData();
        player.hero.InitFinish();

        genericMap.AddHero(playerHero);

        enemies = new List<Enemy>();

		MCamera.inst.ClearTargets();

		int enemyCount = 1;

		for (int i = 0; i < enemyCount; i++) {
			Enemy enemy = new Enemy();

            GD_HeroItem enemyData = GameBalance.GenerateEnemyData_Arena();

			HeroBase enemyHero = heroFactory.Create(new HeroConfig() {
				nickname = "Evil Bean " + (i + 1),
				orientation = Orientation.Left,
				teamID = 1,
				role = HeroRole.Enemy,
                heroType = enemyData.heroType,
                heroData = enemyData,
			}, genericMap.GetArea("EnemySpawn").GetRandomPosition());

            enemyHero.info.mmr = Mathf.Clamp(Game.data.player.mmr + MRandom.Range(-30, 60),0,int.MaxValue);

			enemy.AssignHero(enemyHero);
			enemy.Init();
            enemyHero.InitForUI(GameUI.inst.playerPanels[1]);

            genericMap.AddHero(enemyHero);

			enemies.Add(enemy);

			MCamera.inst.AddTarget(new CameraTarget(enemy.hero.body.transform, new Vector2(0, -2), Vector2.one * 2));

            GD_Item enemyPistol = new GD_Item() {
                itemType = ItemType.Weapon_Pistol,
                levelID = (Game.data.player.mmr / 30),
                rareness = ItemRareness.Common,
                info = MAssets.inst.itemsInfo.GetAsset(ItemType.Weapon_Pistol)
            };

            enemyHero.heroEquipment.PreviewItem(enemyPistol, enemyHero.heroEquipment.GetFreeSlots(enemyPistol)[0]);
            enemyHero.InitFinish();
        }

		MCamera.inst.AddTarget(new CameraTarget(player.hero.body.transform, new Vector2(0, -2), Vector2.one * 2));

		playerHero.FindClosestTarget();
		for (int i = 0; i < enemies.Count; i++) {
			enemies[i].hero.FindClosestTarget();
		}

        //Sword sword = (Sword)equipmentFactory.Create(new WeaponConfig(WeaponType.Sword), Vector2.zero);
        //playerHero.AttachEquipment(sword);

        playerHero.info.nickname = Game.data.player.nickname;
        playerHero.info.mmr = Game.data.player.mmr;

        opponentsInfo[0].connectedHero = playerHero;
        opponentsInfo[1].connectedHero = enemies[0].hero;

        opponentsInfo[0].panel = GameUI.inst.playerPanels[0];
        opponentsInfo[1].panel = GameUI.inst.playerPanels[1];

        GameUI.inst.playerPanels[0].SetHero(playerHero);
		GameUI.inst.playerPanels[1].SetHero(enemies[0].hero);
	}  

    protected override void OnGameEvent_HeroDie(HeroDieEvent e) {
        base.OnGameEvent_HeroDie(e);

        if (isRespawningHeroes) {
            return;
        }

        isRespawningHeroes = true;

        heroesInputAllowed = false;

        if (e.hero == player.hero) {
            FX.inst.EnableDeathScreenEffect(true);
        }

        this.Wait(() => {

            OpponentInfo opInfo = opponentsInfo.Find(x => x.connectedHero != e.hero);
            opInfo.score += 1;

            opInfo.panel.winCounter.SetWinsCount(opInfo.score);                  

            if(opInfo.score >= ROUNDS_TO_WIN) {
                GameUI.inst.Show(false);

                if(e.hero == player.hero) {
                    // LOOSE

                    int mmrPenalty = MRandom.Range(5, 10);
                    int coinsGain = MRandom.Range(10, 15);

                    Economy.inst.AddCurrency(CurrencyType.Coin, coinsGain);
                    Game.data.player.mmr = Mathf.Clamp(Game.data.player.mmr - mmrPenalty, 0, int.MaxValue);

                    UIWindowManager.CreateWindow(new UIWData_RoundEnd() {
                        win = false,
                        mmrCount = mmrPenalty,
                        coinCount = coinsGain,
                        earnedChest = null
                    });
                } else {
                    // WIN

                    int mmrGain = MRandom.Range(10, 15);
                    int coinsGain = MRandom.Range(20, 30);

                    Economy.inst.AddCurrency(CurrencyType.Coin, coinsGain);
                    Game.data.player.mmr = Mathf.Clamp(Game.data.player.mmr + mmrGain, 0, int.MaxValue);

                    GD_Chest m_EarnedChest = null; 
                    
                    if (Game.data.inventory.chests.Count < 4) {
                        m_EarnedChest = GameBalance.GenerateRoundRewardChest();
                        Game.data.inventory.chests.Add(m_EarnedChest);
                    }

                    UIWindowManager.CreateWindow(new UIWData_RoundEnd() {
                        win = true,
                        mmrCount = mmrGain,
                        coinCount = coinsGain,
                        earnedChest = m_EarnedChest
                    });                    
                }             
            } else {             
                // Next round
                genericMap.ResetMap();

                isRespawningHeroes = false;
                heroesInputAllowed = true;
            }

            FX.inst.EnableDeathScreenEffect(false);
        }, 1f);
    }

    [ConsoleMethod("ai", "Enable/disable all AI")]
    public static void EnableAI(bool enable) {
        if (GameMode.current is GM_ArenaBot gmArenaBot) {
            foreach (var enemy in gmArenaBot.enemies) {
                enemy.enabledAI = enable;
            }
        }
    }

}

public class OpponentInfo {
    public int score;
    public HeroBase connectedHero;
    public PlayerPanel panel;

    public OpponentInfo() {
        score = 0;
    }

}