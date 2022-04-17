using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadmapLineSegment : PoolObject {

    public RectTransform rectT;

    public override Type GetPoolObjectType() {
        return typeof(RoadmapLineSegment);
    }
}
