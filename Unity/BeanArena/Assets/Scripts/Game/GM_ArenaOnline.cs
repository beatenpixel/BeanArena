using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_ArenaOnline : GameMode {

    public const int ROUNDS_TO_WIN = 2;

    public List<OpponentInfo> opponentsInfo;

    private bool isRespawningHeroes;

    private List<Player> players;

    private List<JoystickInputUI> joystickInputs;

    public override void InitGame(Game game) {
        base.InitGame(game);

        opponentsInfo = new List<OpponentInfo>() {
            new OpponentInfo(),
            new OpponentInfo()
        };
    }

    public override bool StartGame() {
        SpawnHeroes_VsOnline();
        Debug.Log("Start Online");

        GameUI.inst.Show(true);
        MenuUI.inst.Show(false);

        return base.StartGame();
    }

    public override void InternalUpdate() {
        base.InternalUpdate();
    }

    private void SpawnHeroes_VsOnline() {
        int playersCount = 2;

        players = new List<Player>();
        joystickInputs = new List<JoystickInputUI>();

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

            JoystickInputUI joystickInput = MPool.Get<JoystickInputUI>(null, GameUI.inst.gameModeStuffHolderT);
            joystickInput.OnEvent.AddListener((x) => {
                player.MoveJoystickInput(x);
            });

            joystickInput.joystickDrift = 0f;
            joystickInput.cancelRange = 0f;

            if (i == 0) {
                joystickInput.rectT.SetAnchor(Vector2.zero, new Vector2(0.5f, 1f));
                joystickInput.rectT.SetOffset(Vector2.zero, Vector2.zero);
            } else if (i == 1) {
                joystickInput.rectT.SetAnchor(new Vector2(0.5f, 0f), Vector2.one);
                joystickInput.rectT.SetOffset(Vector2.zero, Vector2.zero);
            }

            joystickInputs.Add(joystickInput);
        }

        for (int i = 0; i < players.Count; i++) {
            players[i].hero.FindClosestTarget();
        }
    }

    protected override void OnPreExitGame() {
        base.OnPreExitGame();

        for (int i = 0; i < joystickInputs.Count; i++) {
            joystickInputs[i].DestroyJoystick();
        }
        joystickInputs.Clear();
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

                Player winPlayer = players.Find(x => x.hero != e.hero);

                int coinsGain = MRandom.Range(10, 15);
                Economy.inst.AddCurrency(CurrencyType.Coin, coinsGain);

                GD_Chest m_EarnedChest = null;

                if (Game.data.inventory.chests.Count < 4) {
                    m_EarnedChest = GameBalance.GenerateRoundRewardChest();
                    Game.data.inventory.chests.Add(m_EarnedChest);
                }

                UIWindowManager.CreateWindow(new UIWData_RoundEnd(GameModeVSType.Local) {
                    coinCount = coinsGain,
                    earnedChest = m_EarnedChest,
                    wonPlayerName = winPlayer.hero.info.nickname
                });

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