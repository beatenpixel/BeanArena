using DG.Tweening;
using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCamera : Singleton<MCamera> {

    public CameraFollowMode followMode;

    public Camera cam;
    public Transform camT;

    public Camera shadowCam;
    public Transform shadowCamT;

    public GradientBackground gradientBackground;
    public FastPostProcessing fastPostProcessing;
    public MAudioListener audioListener;

    [Header("Config")]
    public SO_CameraConfig config;

    private RaycastHitCache hitCache;

    [SerializeField] private List<CameraTarget> targets;
    private Vector2 shakeValue;

    private Vector3 startCamPos;
    private float startCamSize;

    private Vector2 camPosDamp;
    private Vector2 targetPos;
    private Vector2 pos;

    private float targetSize;
    private float targetSizeDamp;

    private Bounds targetsBounds;

    private Tween shakeTween;

    public static float SCREEN_WH_RATIO => Screen.width / (float)Screen.height;

    private void Awake() {
        startCamPos = camT.position;
        startCamSize = cam.orthographicSize;
    }

    public override void Init() {
        hitCache = new RaycastHitCache(5);

        audioListener.Init();

        MGameLoop.Update.Register(1000, InternalUpdate);
        MGameLoop.LateUpdate.Register(1000, InternalLateUpdate);
    }

    protected override void Shutdown() {
        MGameLoop.Update.Unregister(InternalUpdate);
        MGameLoop.LateUpdate.Unregister(InternalLateUpdate);
    }

    private void InternalUpdate() {

    }

    private void InternalLateUpdate() {
        
        if(followMode == CameraFollowMode.TrackTargets) {
            UpdateTrackTargetsMode();
        } else if(followMode == CameraFollowMode.Fixed) {
            UpdateFixedMode();
        }

        Vector3 newPos = Vector2.SmoothDamp(camT.position, targetPos, ref camPosDamp, config.followTime) + shakeValue;
        newPos = ClampCameraPosInsideArea(newPos, config.limitsCenter, config.limitsSize, cam);
        camT.position = newPos.SetZ(startCamPos.z);

        float newSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref targetSizeDamp, config.sizeChangeTime);
        cam.orthographicSize = newSize;

        shadowCamT.position = camT.position;
        shadowCam.orthographicSize = cam.orthographicSize;

        gradientBackground.AlignSizeToCamera(cam);
    }

    private void UpdateFixedMode() {

    }

    private void UpdateTrackTargetsMode() {
        int enabledTargetsCount = 0;
        for (int i = 0; i < targets.Count; i++) {
            enabledTargetsCount += (targets[i].t != null && targets[i].doFollow) ? 1 : 0;
        }

        if (enabledTargetsCount > 0) {
            targetsBounds = new Bounds();

            for (int i = targets.Count - 1; i >= 0; i--) {
                if (targets[i].t != null && targets[i].doFollow) {
                    Bounds b = new Bounds(targets[i].targetPosition, targets[i].bounds);
                    targetsBounds.Encapsulate(b);
                }
            }

            targetsBounds.Expand(config.followMargin);

            targetPos = targetsBounds.center;

            float minSize, maxSize;

            if (config.useLimits) {
                minSize = Mathf.Min(config.limitsSize.y * 0.5f, config.minMaxSize.x);
                maxSize = Mathf.Min(config.limitsSize.y * 0.5f, config.minMaxSize.y);
            } else {
                minSize = config.minMaxSize.x;
                maxSize = config.minMaxSize.y;
            }

            //MDraw.TextLeft($"maxSize: " + maxSize.ToString("F1"), new Vector2(30, 100));

            if (targetsBounds.size.y > config.minMaxSize.y) {
                targetSize = Mathf.Clamp(targetsBounds.size.y * 0.5f, minSize, maxSize);
            } else {
                targetSize = Mathf.Clamp(targetsBounds.size.x / SCREEN_WH_RATIO * 0.5f, minSize, maxSize);
            }
        } else {
            targetPos = startCamPos;
            targetSize = startCamSize;
        }

        if (config.useLimits) {
            targetPos = ClampCameraPosInsideArea(targetPos, config.limitsCenter, config.limitsSize, cam);
        }       
    }

    private void OnDrawGizmosSelected() {
        if (config.useLimits) {
            Gizmos.color = Color.red.SetA(0.1f);
            Gizmos.DrawCube(config.limitsCenter, config.limitsSize);
        }

        for (int i = 0; i < targets.Count; i++) {
            Gizmos.color = Color.green.SetA(0.1f);
            Gizmos.DrawCube(targets[i].targetPosition, targets[i].bounds);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetPos, 0.3f);
    }

    /*
    public void ApplyGraphicsPreset(GraphicsPreset preset) {
        
        postFXVolume.sharedProfile.TryGet(out Bloom bloom);
        postFXVolume.sharedProfile.TryGet(out Vignette vignette);

        switch (preset) {
            case GraphicsPreset.Fast:
                bloom.active = false;
                vignette.active = false;
                break;
            case GraphicsPreset.Medium:
                bloom.active = false;
                vignette.active = true;
                break;
            case GraphicsPreset.Fancy:
                bloom.active = true;
                vignette.active = true;
                break;
        }
    }
    */

    public float GetSizeAccordingToScreen(Vector2 safeAreaSize, ScreenMatchType matchType) {
        float wDivH = Screen.width / (float)Screen.height;
        IMDraw.Label(50, 50, Color.white, 20, LabelPivot.UPPER_LEFT, LabelAlignment.RIGHT, "screen: " + Screen.width + " " + Screen.height);

        float minSizeToSafeX = safeAreaSize.x / wDivH * 0.5f;
        float minSizeToSafeY = safeAreaSize.y * 0.5f;

        float size;

        if (matchType == ScreenMatchType.Horizontal) {
            size = minSizeToSafeX;
        } else if (matchType == ScreenMatchType.Vertical) {
            size = minSizeToSafeY;
        } else {
            size = Mathf.Max(minSizeToSafeX, minSizeToSafeY);
        }

        return size;
    }

    public void ClearTargets() {
        targets.Clear();
    }

    public CameraTarget AddTarget(CameraTarget target) {
        SetFollowMode(CameraFollowMode.TrackTargets);
        targets.Add(target);
        return target;
    }

    public void SetFollowMode(CameraFollowMode _followMode) {
        followMode = _followMode;
    }

    public void SetFixedArea(Vector2 center, Vector2 size, ScreenMatchType matchType, bool instant) {
        ClearTargets();
        SetFollowMode(CameraFollowMode.Fixed);

        targetSize = GetSizeAccordingToScreen(size, matchType);
        targetPos = center;

        if(instant) {
            camT.position = center;
            cam.orthographicSize = targetSize;
        }
    }

    public void Shake() {
        if (shakeTween != null) {
            shakeTween.Kill(true);
        }
        shakeValue = Vector3.zero;

        shakeTween = DOTween.Shake(() => shakeValue, (x) => shakeValue = x, 0.3f, 0.4f, 12).OnComplete(() => {
            shakeTween = null;
        });
    }

    public static Vector2 GetWorldPos2D(Vector2 screenPos) {
        InitIfNeeded(null);
        return inst.cam.ScreenToWorldPoint(screenPos).SetZ(0);
    }

    public static bool GetWorldPos3D(Vector2 screenPos, out Vector3 pos) {
        InitIfNeeded(null);

        Ray ray = inst.cam.ScreenPointToRay(screenPos);
        int hitsCount = Physics.RaycastNonAlloc(ray, inst.hitCache, 100);

        if (hitsCount > 0) {
            pos = inst.hitCache[0].point;
            return true;
        } else {
            pos = Vector3.zero;
            return false;
        }
    }

    public static bool GetWorldHit3D(Vector2 screenPos, out RaycastHit hit) {
        InitIfNeeded(null);

        Ray ray = inst.cam.ScreenPointToRay(screenPos);
        int hitsCount = Physics.RaycastNonAlloc(ray, inst.hitCache, 100);

        if (hitsCount > 0) {
            hit = inst.hitCache[0];
            return true;
        } else {
            hit = default;
            return false;
        }
    }

    public static Vector2 ClampCameraPosInsideArea(Vector2 camPos, Vector2 areaCenter, Vector2 areaSize, Camera _cam) {
        Vector2 mapBottomLeft = areaCenter - areaSize * 0.5f;
        Vector2 mapTopRight = areaCenter + areaSize * 0.5f;
        Vector2 camSize = GetCameraWorldSize(_cam);

        Vector2 minCamPos = mapBottomLeft + camSize * 0.5f;
        Vector2 maxCamPos = mapTopRight - camSize * 0.5f;

        return new Vector2(Mathf.Clamp(camPos.x, minCamPos.x, maxCamPos.x), Mathf.Clamp(camPos.y, minCamPos.y, maxCamPos.y));
    }

    public static Vector2 GetCameraWorldSize(Camera _cam) {
        float k = Screen.width / (float)Screen.height;
        return new Vector2(_cam.orthographicSize * k, _cam.orthographicSize) * 2f;
    }

    public static Vector4 GetCameraCorners(Camera _cam, Transform _camT) {
        Vector2 camSize = GetCameraWorldSize(_cam);
        return new Vector4(_camT.position.x - camSize.x * 0.5f, _camT.position.y - camSize.y * 0.5f, _camT.position.x + camSize.x * 0.5f, _camT.position.y + camSize.y * 0.5f);
    }

}

[System.Serializable]
public class CameraTarget {
    public Transform t;
    public Vector2 offset;
    public Vector2 bounds;
    public float priority = 0.5f;
    public bool doFollow = true;

    public CameraTarget(Transform _t, Vector2 _offset, Vector2 _bounds, bool doFollow = true) {
        t = _t;
        offset = _offset;
        bounds = _bounds;
        this.doFollow = doFollow;
    }

    public CameraTarget SetPriority(float p) {
        priority = p;
        return this;
    }

    public Vector2 targetPosition => (Vector2)t.position + offset;

}

public enum CameraFollowMode {
    None,
    Fixed,
    TrackTargets
}

public enum ScreenMatchType {
    Horizontal,
    Vertical,
    Both
}