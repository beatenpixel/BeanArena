using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MAppConfig", menuName = "MicroCrew/MAppConfig")]
public class MAppConfig : ScriptableObject {

    public bool useDisplayFps = true;
    public int targetFPS = 60;
    public bool isUnityRemote;
    public bool unityLoggerEnabled;

}
