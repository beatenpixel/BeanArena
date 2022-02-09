using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class GD_Chest : GD {

    public ChestType type;
    public string chestGUID;
    public DateTime openTime;

    [NonSerialized] public SO_ChestInfo info;

    public GD_Chest() : base(GDType.ChestData, GDLoadOrder.Default) {
        SetDefaults(default);
    }

    public void Restore() {
        
    }

    public GD_Chest(SerializationInfo info, StreamingContext sc) : base(info, sc) {
        type = (ChestType)info.GetByte("chestType");
        chestGUID = info.GetString("chestGUID");
        openTime = info.GetValueSafe<DateTime>("openTime");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("chestType", (byte)type);
        info.AddValue("chestGUID", chestGUID);
        info.AddValue("openTime", openTime);
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds) {
        type = ChestType.Common;
        chestGUID = Guid.NewGuid().ToString();
        openTime = DateTime.UtcNow.AddSeconds(30);
    }

}

public enum ChestType : byte {
    Common,
    Epic,
    Legendary
}