using DG.Tweening;
using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneTransitionManager : Singleton<SceneTransitionManager> {

    public UICanvas canvas;

    public CanvasGroup levelLoadCanvasGroup;
    public TextMeshProUGUI loadingText;
    public RectTransform loadingWheelRectT;

    public bool useDotsAnimation = false;

    private float minLoadDuration = 0.3f;

    private string levelName;
    private bool isTransitioning;

    private float startLoadTime;
    private float dotAnimationTime = 0.5f;
    private float nextDotAnimationTime;
    private int dotCount;

    public override void Init() {
        MSceneManager.OnSceneChangeEnd.Add(OnSceneChangeEnd);
    }

    protected override void Shutdown() {

    }

    private void Update() {
        if (isTransitioning) {

            if (useDotsAnimation) {
                if (Time.realtimeSinceStartup > nextDotAnimationTime) {
                    nextDotAnimationTime = Time.realtimeSinceStartup + dotAnimationTime;

                    dotCount += 1;
                    if (dotCount > 3) {
                        dotCount = 0;
                    }

                    string dotsText = "";
                    for (int i = 0; i < dotCount; i++) {
                        dotsText += ".";
                    }

                    loadingText.text = MLocalization.Get("LEVEL_LOADING_PARAM", LocalizationGroup.Main, dotsText);
                }
            }

            loadingWheelRectT.rotation *= Quaternion.Euler(0, 0, -180f * Time.unscaledDeltaTime);
        }
    }

    public void LoadLevelAsync(string _levelName) {
        levelName = _levelName;
        isTransitioning = true;

        canvas.Show(true);

        levelLoadCanvasGroup.alpha = 0f;
        levelLoadCanvasGroup.DOFade(1f, 0.2f).SetUpdate(true).OnComplete(OnFadeInAsync);
    }

    public void LoadLevel(string _levelName) {
        levelName = _levelName;
        isTransitioning = true;

        canvas.Show(true);

        levelLoadCanvasGroup.alpha = 0f;
        levelLoadCanvasGroup.DOFade(1f, 0.2f).SetUpdate(true).OnComplete(OnFadeIn);
    }

    private void OnFadeIn() {
        startLoadTime = Time.realtimeSinceStartup;
        MSceneManager.LoadScene(levelName);
    }

    private void OnFadeInAsync() {
        startLoadTime = Time.realtimeSinceStartup;
        nextDotAnimationTime = startLoadTime;
        dotCount = 0;
        loadingWheelRectT.rotation = Quaternion.Euler(0, 0, 0);
        MSceneManager.LoadSceneAsync(levelName, OnSceneLoadState);
    }

    private void OnSceneLoadState(MSceneManager.SceneLoadState state) {
        switch (state) {
            case MSceneManager.SceneLoadState.Begin:

                break;
            case MSceneManager.SceneLoadState.Update:
                
                break;
            case MSceneManager.SceneLoadState.Finish:

                break;
        }
    }

    private void OnSceneChangeEnd(SceneEvent e) {
        if (isTransitioning) {
            if (Time.realtimeSinceStartup < startLoadTime + minLoadDuration) {
                this.WaitRealtime(TransitionOut, minLoadDuration - (Time.realtimeSinceStartup - startLoadTime));
            } else {
                TransitionOut();
            }
        }
    }

    private void TransitionOut() {
        levelLoadCanvasGroup.DOFade(0f, 0.2f).SetUpdate(true).OnComplete(OnFadeOut);
    }

    private void OnFadeOut() {
        isTransitioning = false;
        levelName = null;
        canvas.Show(false);
    }

}
