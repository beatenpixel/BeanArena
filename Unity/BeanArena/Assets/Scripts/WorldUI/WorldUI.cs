using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUI : Singleton<WorldUI> {

    public Transform rootT;

    private List<WUI_Text> floatingLabels;
    private List<WUI_SpriteHealthbar> spriteHealthbars;

    public override void Init() {
        WUI_TextStyle.InitializeStyles();

        floatingLabels = new List<WUI_Text>();
        spriteHealthbars = new List<WUI_SpriteHealthbar>();

        MSceneManager.OnSceneChangeStart.Add(BeforeSceneTransition);

        MGameLoop.Update.Register(InternalUpdate);
    }

    protected override void Shutdown() {

    }

    internal void InternalUpdate() {
        for (int i = 0; i < floatingLabels.Count; i++) {
            floatingLabels[i].InternalUpdate();
        }

        for (int i = 0; i < spriteHealthbars.Count; i++) {
            spriteHealthbars[i].InternalUpdate();
        }
    }

    private void BeforeSceneTransition(SceneEvent transition) {
        for (int i = floatingLabels.Count - 1; i >= 0; i--) {
            floatingLabels[i].Push();
            floatingLabels.RemoveAt(i);
        }

        for (int i = spriteHealthbars.Count - 1; i >= 0; i--) {
            spriteHealthbars[i].Push();
            spriteHealthbars.RemoveAt(i);
        }
    }

    public WUI_Text AddText(string label, Transform _target, Vector2 _offset, WUI_TextStyle style) {
        WUI_Text newLabel = MPool.Get<WUI_Text>();
        newLabel.SetTarget(label, _target, _offset, style);
        floatingLabels.Add(newLabel);
        return newLabel;
    }

    public void RemoveText(WUI_Text label) {
        if (floatingLabels.Contains(label)) {
            floatingLabels.Remove(label);
            label.Push();
        }
    }

    public WUI_SpriteHealthbar AddSpriteHealthbar(float startValue, Transform _target, Vector2 _offset, float width = 3) {
        WUI_SpriteHealthbar newHealthbar = MPool.Get<WUI_SpriteHealthbar>();
        newHealthbar.Init(startValue, _target, _offset, width);
        spriteHealthbars.Add(newHealthbar);
        return newHealthbar;
    }

    public void RemoveRemoveSpriteHealthbar(WUI_SpriteHealthbar healthbar) {
        if (spriteHealthbars.Contains(healthbar)) {
            spriteHealthbars.Remove(healthbar);
            healthbar.Push();
        }
    }

    private Vector2 minMaxDmgSize = new Vector2(0.8f, 1.5f);
    private Vector2 minMaxDmgSizeValue = new Vector2(20, 100);

}