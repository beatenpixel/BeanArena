using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class GD_Game : GD {

    public int timeInGame;
    public int gameEntersCount;
    public bool soundOn;
    public List<GD> objects;

    public GD_Inventory inventory;
    public GD_Player player;

    public GD_Game() : base(GDType.Game, GDLoadOrder.Pre_0) {
        SetDefaults(default);
    }

    public void RestoreGame() {
        inventory.Restore();
        player.Restore();
    }

    public GD_Game(SerializationInfo info, StreamingContext sc) : base(info, sc) {
        timeInGame = info.GetInt32("timeInGame");
        gameEntersCount = info.GetInt32("gameEntersCount");
        soundOn = info.GetBoolean("soundOn");
        inventory = (GD_Inventory)info.GetValue("playerInventory", typeof(GD_Inventory));
        player = (GD_Player)info.GetValue("player", typeof(GD_Player));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("timeInGame", timeInGame);
        info.AddValue("gameEntersCount", gameEntersCount);
        info.AddValue("soundOn", soundOn);
        info.AddValue("playerInventory", inventory);
        info.AddValue("player", player);
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds) {
        timeInGame = 0;
        gameEntersCount = 0;
        soundOn = true;
        objects = new List<GD>();

        inventory = new GD_Inventory();
        player = new GD_Player();
    }

}
