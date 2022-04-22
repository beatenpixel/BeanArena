using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_ArenaLocal : GameMode {

    public const int ROUNDS_TO_WIN = 2;

    public List<OpponentInfo> opponentsInfo;

    private bool isRespawningHeroes;

    private List<Player> players;

    public override void InitGame(Game game) {
        base.InitGame(game);

        opponentsInfo = new List<OpponentInfo>() {
            new OpponentInfo(),
            new OpponentInfo()
        };
    }

    public override bool StartGame() {
        SpawnHeroes_VsLocal();

        GameUI.inst.Show(true);
        MenuUI.inst.Show(false);

        return base.StartGame();
    }

    public override void InternalUpdate() {
        base.InternalUpdate();

        if (Input.GetKeyDown(KeyCode.Keypad7)) {
            foreach (var hero in genericMap.heroes) {
                if (hero.info.role == HeroRole.Player) {
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

    private void SpawnHeroes_VsLocal() {
        int playersCount = 2;

        players = new List<Player>();

        GD_HeroItem equipedHero = Game.data.GetEquipedHero();

        MCamera.inst.ClearTargets();

        for (int i = 0; i < playersCount; i++) {
            Player player = new Player();
            players.Add(player);

            HeroBase playerHero = heroFactory.Create(new HeroConfig() {
                nickname = "Lorg" + i,
                orientation = (i == 0) ? Orientation.Right : Orientation.Left,
                teamID = i,
                role = HeroRole.Player,
                heroType = equipedHero.heroType,
                heroData = Game.data.inventory.heroes.Find(x => x.heroType == equipedHero.heroType)
            }, genericMap.GetArea((i == 0) ? "PlayerSpawn" : "EnemySpawn").GetRandomPosition());

            player.AssignHero(playerHero);
            player.Init();
            player.hero.InitForUI(GameUI.inst.playerPanels[i]);
            player.hero.heroEquipment.LoadEquipmentFromGameData();
            player.hero.InitFinish();
            genericMap.AddHero(playerHero);

            playerHero.info.nickname = Game.data.player.nickname + i;
            playerHero.info.mmr = Game.data.player.mmr;

            MCamera.inst.AddTarget(new CameraTarget(player.hero.body.transform, new Vector2(0, -2), Vector2.one * 2));

            opponentsInfo[i].connectedHero = playerHero;
            opponentsInfo[i].panel = GameUI.inst.playerPanels[i];

            GameUI.inst.playerPanels[i].SetHero(playerHero);
        }
    }


    protected override void OnGameEvent_HeroDie(HeroDieEvent e) {
        base.OnGameEvent_HeroDie(e);

        if (isRespawningHeroes) {
            return;
        }

        isRespawningHeroes = true;

        heroesInputAllowed = false;


        this.Wait(() => {

            OpponentInfo opInfo = opponentsInfo.Find(x => x.connectedHero != e.hero);
            opInfo.score += 1;

            opInfo.panel.winCounter.SetWinsCount(opInfo.score);

            if (opInfo.score >= ROUNDS_TO_WIN) {
                GameUI.inst.Show(false);

                /*
                if (e.hero == players.hero) {
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
                */
            } else {
                // Next round

                genericMap.ResetMap();

                isRespawningHeroes = false;
                heroesInputAllowed = true;
            }

            FX.inst.EnableDeathScreenEffect(false);
        }, 1f);
    }

}


/*

      private void SpawnHeroes_VsLocal() {
        int playersCount = 2;

        GD_HeroItem equipedHero = Game.data.GetEquipedHero();

        Game.inst.ClearPlayers();
        enemies = new List<Enemy>();        
        MCamera.inst.ClearTargets();

        for (int i = 0; i < playersCount; i++) {
            Player player = Game.inst.AddPlayer();

            HeroBase playerHero = heroFactory.Create(new HeroConfig() {
                nickname = "Lorg",
                orientation = (i == 0)?Orientation.Right:Orientation.Left,
                teamID = i,
                role = HeroRole.Player,
                heroType = equipedHero.heroType,
                heroData = Game.data.inventory.heroes.Find(x => x.heroType == equipedHero.heroType)
            }, genericMap.GetArea((i == 0)?"PlayerSpawn":"EnemySpawn").GetRandomPosition());

            player.AssignHero(playerHero);
            player.Init();
            player.hero.InitForUI(GameUI.inst.playerPanels[i]);
            player.hero.heroEquipment.LoadEquipmentFromGameData();
            player.hero.InitFinish();
            genericMap.AddHero(playerHero);

            playerHero.info.nickname = Game.data.player.nickname;
            playerHero.info.mmr = Game.data.player.mmr;

            MCamera.inst.AddTarget(new CameraTarget(player.hero.body.transform, new Vector2(0, -2), Vector2.one * 2));

            opponentsInfo[i].connectedHero = playerHero;
            opponentsInfo[i].panel = GameUI.inst.playerPanels[i];

            GameUI.inst.playerPanels[i].SetHero(playerHero);
        }



    }

    */