using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class GD_Chest : GD {

    public ChestType type;
    public string chestGUID;
    public bool isOpening;
    public DateTime openTime;

    [NonSerialized] public SO_ChestInfo info;

    public TimeSpan timeLeft {
        get {
            return openTime - DateTime.UtcNow; ;
        }
    }

    public int gemSkipPrice {
        get {
            float p = (float)(timeLeft.TotalSeconds / info.openDurationInSec);
            int gemSkipPrice = Mathf.RoundToInt(Mathf.Lerp(info.gemSkipPrice.x, info.gemSkipPrice.y, p));
            return gemSkipPrice;
        }
    }

    public GD_Chest() : base(GDType.ChestData, GDLoadOrder.Default) {
        SetDefaults(default);
    }

    public void Restore() {
        
    }

    public GD_Chest(SerializationInfo info, StreamingContext sc) : base(info, sc) {
        type = (ChestType)info.GetByte("chestType");
        chestGUID = info.GetString("chestGUID");
        openTime = info.GetValueSafe<DateTime>("openTime");
        isOpening = info.GetBoolean("isOpening");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("chestType", (byte)type);
        info.AddValue("chestGUID", chestGUID);
        info.AddValue("openTime", openTime);
        info.AddValue("isOpening", isOpening);
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds) {
        type = ChestType.Common;
        chestGUID = Guid.NewGuid().ToString();
        openTime = DateTime.UtcNow.AddSeconds(15);
        isOpening = false;
    }

    public bool IsReadyToOpen() {
        return DateTime.UtcNow > openTime;
    }

    public void StartOpening() {
        isOpening = true;
        openTime = DateTime.UtcNow.AddSeconds(info.openDurationInSec);
    }

}

public enum ChestType {
    Common = 1,
    Epic = 2,
    Legendary = 3
}