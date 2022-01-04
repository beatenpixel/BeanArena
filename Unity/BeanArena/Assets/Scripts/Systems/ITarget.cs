using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITarget {
    public TargetInfo targetInfo { get; }
    public List<TargetAimPoint> targetAimPoints { get; }
}

public class TargetInfo {
    public TargetType type;
}

public class TargetAimPoint {
    public Vector2 worldPos;
}

public class TargetAim {
    public ITarget target;
    public TargetAimPoint aimPoint;
}

public enum TargetAttitude {
    Neutral,
    Friendly,
    Hostile
}

public enum TargetType {
    MapElement,
    Hero
}
