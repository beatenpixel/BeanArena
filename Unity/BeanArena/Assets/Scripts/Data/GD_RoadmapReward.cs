using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class GD_RoadmapReward : GD {

    public int rewardUID;
    public bool isClaimed;

    public GD_RoadmapReward() : base(GDType.RoadmapReward, GDLoadOrder.Default) {
        SetDefaults(default);
    }

    public GD_RoadmapReward(SerializationInfo info, StreamingContext sc) : base(info, sc) {
        rewardUID = info.GetInt32("rewardUID");
        isClaimed = info.GetBoolean("isClaimed");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("rewardUID", rewardUID);
        info.AddValue("isClaimed", isClaimed);
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds) {
        rewardUID = -1;
        isClaimed = false;
    }

}