using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragInputUI : MonoBehaviour {

    public bool inputEnabled = true;
    public bool ignoreOverUIElements = true;

    public DragInputConfig config;

    public UICanvas canvas;

    public GameObject group;

    public RectTransform pointA;
    public RectTransform pointB;

    public RectTransform dotPrefab;
    public List<RectTransform> dots;

    private Vector2 startDragPos;
    private bool startedDrag;

    private Vector3 pointAStartScale;
    private Vector3 pointBStartScale;
    private Vector3 dotStartScale;

    public Action<DragInput> OnInput;

    private Vector2 finalInput;

    public struct DragInput {
        public DragInputState state;
        public Vector2 input;
    }

    public enum DragInputState {
        Start,
        Update,
        Input,
        Cancel,
    }

    private void Awake() {
        dotStartScale = dotPrefab.localScale;
        pointAStartScale = pointA.localScale;
        pointBStartScale = pointB.localScale;
    }

    private void Update() {
        if (inputEnabled) {

            if (!MInput.IsPointerOverUIObject()) {

                if (Input.GetMouseButtonDown(0)) {
                    startDragPos = Input.mousePosition;
                    startedDrag = true;

                    pointA.position = startDragPos;

                    group.SetActive(true);

                    OnInput?.Invoke(new DragInput() {
                        state = DragInputState.Start
                    });
                }

                if (startedDrag) {
                    if (Input.GetMouseButton(0)) {
                        pointB.position = Input.mousePosition;

                        Vector2 uiPosA = canvas.WorldToCanvasPos(pointA.position);
                        Vector2 uiPosB = canvas.WorldToCanvasPos(pointB.position);

                        //Vector2 start = uiPosA + (uiPosB - uiPosA).normalized * (pointA.localScale.x * pointA.rect.width * 0.5f + dotPrefab.rect.width * 1f);
                        //Vector2 end = uiPosB + (uiPosA - uiPosB).normalized * (pointB.localScale.x * pointB.rect.width * 0.5f + dotPrefab.rect.width * 1f);

                        Vector2 start = uiPosA + (uiPosB - uiPosA).normalized * (pointA.localScale.x * pointA.rect.width * 0.5f - dotPrefab.rect.width * 0.3f);
                        Vector2 end = uiPosB + (uiPosA - uiPosB).normalized * (pointB.localScale.x * pointB.rect.width * 0.5f - dotPrefab.rect.width * 0.3f);

                        float maxDst = canvas.canvasT.sizeDelta.y * config.maxDstPercent;

                        Vector2 uiDD = (uiPosB - uiPosA);
                        float dst = uiDD.magnitude;
                        float dst2;

                        if (dst >= maxDst) {
                            dst = maxDst;
                            pointB.anchoredPosition = uiPosA + uiDD.normalized * maxDst;

                            uiPosB = canvas.WorldToCanvasPos(pointB.position);
                            //end = uiPosB + (uiPosA - uiPosB).normalized * (pointB.rect.width * 0.5f + dotPrefab.rect.width * 1f);
                            end = uiPosB + (uiPosA - uiPosB).normalized * (pointB.rect.width * 0.5f - dotPrefab.rect.width * 0.3f);
                        }

                        finalInput = uiDD.normalized * (dst / maxDst);

                        dst2 = Mathf.Clamp(dst - (pointA.rect.width * 0.5f + pointB.rect.width * 0.5f + dotPrefab.rect.width * 1f ), 0, 10000);

                        float len = dotPrefab.rect.width * 2f;

                        float scalePercent = Mathf.Clamp01(dst / maxDst);
                        pointA.localScale = pointAStartScale * (1 - scalePercent * config.pointAShrinkPercent);
                        pointB.localScale = pointBStartScale * (1 + scalePercent * config.pointBExpandPercent);

                        int count = Mathf.CeilToInt(dst2 / len * config.dotsCountMul);
                        count = Mathf.Clamp(count, 0, dots.Count);

                        bool debug = false;

                        if (debug) {
                            MDraw.TextLeft($"{dst2}, count: {count}", new Vector2(10, 10));
                            MDraw.TextLeft($"width {len}", new Vector2(10, 30));
                        }

                        for (int i = 0; i < dots.Count; i++) {
                            if (i < count) {
                                dots[i].gameObject.SetActive(true);
                                float perc = 0f;
                                if (count > 1) {
                                    perc = (i + 1) / (float)(count + 1);
                                } else {
                                    perc = 0.5f;
                                }

                                Vector2 canvasPos = Vector2.Lerp(start, end, perc);

                                dots[i].localScale = dotStartScale * (1f - scalePercent * config.dotsShrinkPercent);
                                dots[i].anchoredPosition = canvasPos;
                            } else {
                                dots[i].gameObject.SetActive(false);
                            }
                        }

                        OnInput?.Invoke(new DragInput() {
                            state = DragInputState.Update,
                            input = finalInput
                        });
                    }

                    if (Input.GetMouseButtonUp(0)) {
                        startedDrag = false;
                        group.SetActive(false);

                        OnInput?.Invoke(new DragInput() {
                            state = (finalInput.magnitude < config.cancelInputPercent) ? DragInputState.Cancel : DragInputState.Input,
                            input = finalInput
                        });

                        finalInput = Vector2.zero;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class DragInputConfig {
        [Range(0f,1f)] public float maxDstPercent = 0.2f;
        [Range(0f,1f)] public float cancelInputPercent = 0.2f;
        public float pointAShrinkPercent = 0.35f;
        public float pointBExpandPercent = 0.2f;
        public float dotsShrinkPercent = 0.3f;
        public float dotsCountMul = 1.3f;
    }

}
