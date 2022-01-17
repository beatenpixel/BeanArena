using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MicroCrew/CameraConfig")]
public class SO_CameraConfig : ScriptableObject {
    public float followTime = 0.2f;
    public float sizeChangeTime = 0.3f;

    public bool useLimits;
    public Vector2 limitsCenter;
    public Vector2 limitsSize;

    public Vector2 followMargin;
    public Vector2 minMaxSize;
}
