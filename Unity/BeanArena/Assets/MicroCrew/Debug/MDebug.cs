using IngameDebugConsole;
using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using Tayx.Graphy;
using UnityEngine;

public class MDebug : Singleton<MDebug> {

    public GraphyManager graphyManager;
    public DebugLogManager debugLogManager;

    public override void Init() {
        
    }

    protected override void Shutdown() {
        
    }

    [ConsoleMethod("fps", "Show GraphyConsole")]
    public static void CreateCubeAt() {
        inst.graphyManager.ToggleActive();
    }

}
