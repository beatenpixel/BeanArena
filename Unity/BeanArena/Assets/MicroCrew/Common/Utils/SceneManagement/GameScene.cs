using Polyglot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "MicroCrew/GameScene")]
public class GameScene : ScriptableObject {
    public string sceneName;
    public bool isMenu;
}
