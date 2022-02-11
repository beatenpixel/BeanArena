using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MicroCrew/UI/IconContent")]
public class SO_IconContent : ScriptableObject {
    public Sprite sprite;
    public Color color = Color.white;
    public Vector2 anchorMin = new Vector2(0.1f,0.1f);
    public Vector2 anchorMax = new Vector2(0.9f, 0.9f);
    public Vector2 offsetMin;
    public Vector2 offsetMax;
    public Vector2 pivot = new Vector2(0.5f, 0.5f);
}
