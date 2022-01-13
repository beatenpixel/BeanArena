using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 1 - OnActiveSceneChanged
// 2 - OnSceneLoaded

public class MSceneManager : Singleton<MSceneManager> {

    public static PrioritizedEvent<SceneEvent> OnSceneChangeStart = new PrioritizedEvent<SceneEvent>();
    public static PrioritizedEvent<SceneEvent> OnSceneChangeEnd = new PrioritizedEvent<SceneEvent>();

    public static GameScene currentScene { get; private set; }

    private static SceneEvent currentEvent;

    private List<GameScene> allScenes;

    public override void Init() {
        LoadSceneAssets();

        Scene startScene = SceneManager.GetActiveScene();
        currentScene = allScenes.Find(x => x.sceneName == startScene.name);
        currentEvent = new SceneEvent() {
            prev = currentScene,
            next = currentScene
        };

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void Shutdown() {
        
    }

    private void LoadSceneAssets() {
        allScenes = new List<GameScene>(Resources.FindObjectsOfTypeAll<GameScene>());
        allScenes.Log(x => "sceneName: " + x.sceneName);
    }

    private void Internal_LoadScene(string name) {
        GameScene nextScene = allScenes.Find(x => x.sceneName == name);

        currentEvent = new SceneEvent() {
            prev = currentScene,
            next = nextScene
        };

        OnSceneChangeStart?.Invoke(currentEvent);
        SceneManager.LoadScene(name, LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        OnSceneChangeEnd?.Invoke(currentEvent);
    }

    #region Wrappers

    public static void LoadScene(string name) {
        inst.Internal_LoadScene(name);
    }

    public static void ReloadScene() {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

}

public struct SceneEvent {
    public GameScene prev;
    public GameScene next;
}