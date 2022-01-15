using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MicroCrewEditorWindow : EditorWindow {

    public static MicroCrewEditorWindow inst;

    [MenuItem("MicroCrew/Windows/Main")]
    public static void ShowWindow() {
        inst = (MicroCrewEditorWindow)EditorWindow.GetWindow<MicroCrewEditorWindow>("MicroCrew");
    }

    [MenuItem("MicroCrew/FetchGame")]
    public static void FetchGame() {
        
    }

    private void OnEnable() {
        
    }

    void OnGUI() {

    }

}
