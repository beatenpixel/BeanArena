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

    public override void Init() {
        Scene startScene = SceneManager.GetActiveScene();
        currentScene = new GameScene() { scene = startScene, name = startScene.name };

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void Shutdown() {

    }

    private void Internal_LoadScene(string name) {
        GameScene nextScene = new GameScene() {
            name = name
        };

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

public struct GameScene {
    public string name;
    public Scene scene;
}