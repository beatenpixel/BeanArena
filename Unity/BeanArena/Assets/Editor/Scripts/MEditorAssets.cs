using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MicroCrew/Editor/MEditorAssets")]
public class MEditorAssets : SingletonScriptableObject<MEditorAssets> {

    private bool isSetup;

    private void OnEnable() {
        isSetup = false;
        SetupIfNeeded();
    }

    public override void Init() {
        isSetup = false;
        SetupIfNeeded();
    }

    private void SetupIfNeeded() {
        if(isSetup) {
            return;
        }

        isSetup = true;
    }

    public Texture GetTexture(string name) {
        SetupIfNeeded();
        return Resources.Load<Texture>("MEditorAssets/Textures/" + name);
    }

}
