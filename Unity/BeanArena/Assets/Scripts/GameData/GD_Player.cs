using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class GD_Player : GD {

    public int mmr;
    public int coins;
    public int gems;
    public string nickname;

    public GD_Player() : base(GDType.Player, GDLoadOrder.Default) {
        SetDefaults(default);
    }

    public void Restore() {
        
    }

    public GD_Player(SerializationInfo info, StreamingContext sc) : base(info, sc) {
        mmr = info.GetInt32(MMR_KEY);
        coins = info.GetInt32("coins");
        gems = info.GetInt32("gems");
        nickname = info.GetString("playerNickname");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue(MMR_KEY, mmr);
        info.AddValue("coins", coins);
        info.AddValue("gems", gems);
        info.AddValue("playerNickname", nickname);
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds) {
        mmr = 0;
        coins = 0;
        gems = 10;

        System.Random rand = new System.Random();
        nickname = "Bean" + rand.Next(10000);
    }

    private const string MMR_KEY = "mmr";

}
