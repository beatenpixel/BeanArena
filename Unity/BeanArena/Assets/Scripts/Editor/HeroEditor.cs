using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(HeroBase))]
public class HeroEditor : Editor {

    private HeroBase inst;
    private SO_HeroInfo heroInfo;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        inst = (HeroBase)target;

        if (string.IsNullOrEmpty(inst.gameObject.scene.name)) {
            return;
        }

        GUILayout.Space(50);
        GUILayout.Label("=== HeroEditor ===");
        SO_HeroInfo newHeroInfo = (SO_HeroInfo)EditorGUILayout.ObjectField("SO_HeroInfo", heroInfo, typeof(SO_HeroInfo), false);

        if(newHeroInfo != heroInfo) {
            heroInfo = newHeroInfo;
            SwapHeroInfo();
        }
    }

    private void SwapHeroInfo() {
        inst.editorPreviewHeroInfo = heroInfo;
    }

}
