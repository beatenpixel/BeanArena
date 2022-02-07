using MicroCrew.Utils;
using System;
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

    private static Action<SceneLoadState> SceneLoadStateCallback;

    private List<GameScene> allScenes;

    public const string MENU_SCENE_NAME = "menu";
    public const string ARENA_SCENE_NAME = "game";

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
        //allScenes = new List<GameScene>(Resources.FindObjectsOfTypeAll<GameScene>());
        allScenes = new List<GameScene>(Resources.LoadAll<GameScene>("GameScenes"));
        //allScenes.Log(x => "sceneName: " + x.sceneName);
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

    private void Internal_LoadSceneAsync(string sceneName) {
        StartCoroutine(LoadSceneAsync_Coroutine(sceneName));
    }

    private IEnumerator LoadSceneAsync_Coroutine(string sceneName) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        SceneLoadStateCallback?.Invoke(SceneLoadState.Begin);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            SceneLoadStateCallback?.Invoke(SceneLoadState.Update);
            yield return new WaitForSecondsRealtime(1f / 60f);
        }

        SceneLoadStateCallback?.Invoke(SceneLoadState.Finish);
        SceneLoadStateCallback = null;
    }

    #region Wrappers

    public static void LoadScene(string name) {
        inst.Internal_LoadScene(name);
    }

    public static void LoadSceneAsync(string sceneName, Action<SceneLoadState> stateCallback) {
        SceneLoadStateCallback = stateCallback;
        inst.Internal_LoadSceneAsync(sceneName);
    }

    public static void ReloadScene() {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    public enum SceneLoadState {
        Begin,
        Update,
        Finish
    }

}

public struct SceneEvent {
    public GameScene prev;
    public GameScene next;
}