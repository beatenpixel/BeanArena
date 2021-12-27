using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAppUI : Singleton<MAppUI> {

    public GameUI gameUI;
    public UICanvas ignManagerCanvas;

    public override void Init() {
        gameUI.Init();
    }

    protected override void Shutdown() {
        
    }

}
