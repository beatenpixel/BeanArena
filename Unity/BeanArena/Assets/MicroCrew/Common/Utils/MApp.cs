using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MApp : Singleton<MApp> {

    public MAppConfig config;

    public override void Init() {
        InitAppConfig();

        MSceneManager.InitIfNeeded(null);
        GameStateManager.InitIfNeeded(null);
    }

    protected override void Shutdown() {

    }

    private void InternalInit() {

    }

    private void InternalAwake() {
        InitIfNeeded(this);
    }

    private void InternalStart() {
        
    }

    private void InternalPreUpdate() {
        MGameLoop.inst.InternalPreUpdate();
    }

    private void InternalUpdate() {
        MGameLoop.inst.InternalUpdate();
    }

    private void InternalLateUpdate() {
        MGameLoop.inst.InternalLateUpdate();
    }

    private void InternalFixedUpdate() {
        MGameLoop.inst.InternalFixedUpdate();
    }

    private void InitAppConfig() {
        if (config.useDisplayFps) {
            config.targetFPS = Screen.currentResolution.refreshRate;
        }

        Application.targetFrameRate = config.targetFPS;

#if UNITY_EDITOR
        if (config.isUnityRemote) {
            MInput.inputType = MInput.InputType.Mobile;
        } else {
            MInput.inputType = MInput.InputType.PC;
        }
#else
        MInput.inputType = MInput.InputType.Mobile;
#endif

        Debug.unityLogger.logEnabled = config.unityLoggerEnabled;
    }

    #region UnityCallbacks

    private void Awake() {
        InternalInit();
        InternalAwake();
    }

    private void Start() {
        InternalStart();
    }

    private void Update() {
        InternalPreUpdate();
        InternalUpdate();
    }

    private void LateUpdate() {
        InternalLateUpdate();
    }

    private void FixedUpdate() {
        InternalFixedUpdate();
    }

    private void OnApplicationFocus(bool focus) {

    }

    private void OnApplicationQuit() {

    }

    #endregion

}
