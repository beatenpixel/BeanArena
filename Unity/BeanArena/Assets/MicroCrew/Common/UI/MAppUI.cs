using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAppUI : Singleton<MAppUI> {

    public GameUI gameUI;
    public MenuUI menuUI;
    public OverlayUI overlayUI;
    public UICanvas ignManagerCanvas;

    public override void Init() {
        gameUI.Init();
        menuUI.Init();
        overlayUI.Init();
    }

    protected override void Shutdown() {
        
    }

}
