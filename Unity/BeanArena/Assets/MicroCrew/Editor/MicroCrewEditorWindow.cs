using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MicroCrewEditorWindow : EditorWindow {

    public static MicroCrewEditorWindow inst;

    [MenuItem("MicroCrew/PlayerData/DeleteAll")]
    public static void DeletePlayerData() {
        string path = Application.persistentDataPath + GameDataManager.PLAYER_DATA_LOCATION;

        if (File.Exists(path)) {
            File.Delete(path);
            Debug.Log("PlayerData deleted successfully!");
        } else {
            Debug.Log("No data found");
        }
    }

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
