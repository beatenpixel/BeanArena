using DG.Tweening;
using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCamera : Singleton<MCamera> {

    public Camera cam;
    public Transform camT;

    [Header("Config")]
    public Vector2 minMaxZoomFOV;
    public Vector2 offsetInPlaneDefault;
    public Vector2 offsetInPlaneWhenAiming;

    private RaycastHitCache hitCache;

    [SerializeField] private Transform target;
    private Vector3 targetOffset;

    private Vector3 camPosDamp;
    private Vector3 targetPos;

    private float targetFov;
    private float rotX, rotY, rotZ;
    public float camDst = 10;

    private Vector2 currerntOffsetInPlane;
    private Vector2 targetOffsetInPlane;

    private Vector3 shakeOffset;
    private Tween shakeTween;

    private Vector3 startPos;

    private void Awake() {
        startPos = camT.position;
    }

    public override void Init() {
        hitCache = new RaycastHitCache(5);

        targetFov = minMaxZoomFOV.y;
        targetOffsetInPlane = offsetInPlaneDefault;
        currerntOffsetInPlane = offsetInPlaneDefault;

        MGameLoop.Update.Register(1000, InternalUpdate);
        MGameLoop.LateUpdate.Register(1000, InternalLateUpdate);
    }

    protected override void Shutdown() {
        MGameLoop.Update.Unregister(InternalUpdate);
        MGameLoop.LateUpdate.Unregister(InternalLateUpdate);
    }

    private void InternalUpdate() {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * 10f);
        currerntOffsetInPlane = Vector2.Lerp(currerntOffsetInPlane, targetOffsetInPlane, Time.deltaTime * 10f);
    }

    private void InternalLateUpdate() {

    }

    private void LateUpdate() {
        if (target != null) {
            Follow();
        } else {
            camT.position = Vector3.SmoothDamp(camT.position, startPos, ref camPosDamp, 0.1f) + shakeOffset;
        }
    }

    private void Follow() {
        Quaternion rot = Quaternion.Euler(0, rotY, 0) * Quaternion.Euler(rotX, 0, 0) * Quaternion.Euler(0, 0, rotZ);
        targetPos = target.position + targetOffset + rot * Vector3.forward * camDst;

        camT.rotation = rot;
        camT.forward = -camT.forward;

        targetPos += camT.right * currerntOffsetInPlane.x + camT.up * currerntOffsetInPlane.y;

        camT.position = Vector3.SmoothDamp(camT.position, targetPos, ref camPosDamp, 0.1f) + shakeOffset;
    }

    public void SetSizeAccordingToScreen(Vector2 safeAreaSize, ScreenMatchType matchType) {
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

        cam.orthographicSize = size;
    }

    public void SetTarget(Transform _target, Vector3 targetOffset, Vector3 axisRotations, float dst, Vector2 _offsetInPlane) {
        target = _target;
        this.targetOffset = targetOffset;
        rotX = axisRotations.x; rotY = axisRotations.y; rotZ = axisRotations.z;
        camDst = dst;
        targetOffsetInPlane = _offsetInPlane;
    }

    public void SetZoomPercent(float p, bool instant = false) {
        targetFov = Mathf.Lerp(minMaxZoomFOV.x, minMaxZoomFOV.y, 1 - p);

        if (instant) {
            cam.fieldOfView = targetFov;
        }
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

    public void Shake(float force = 0.15f, float duration = 0.4f) {
        if (shakeTween != null) {
            shakeTween.Kill(false);
            shakeOffset = Vector3.zero;
        }

        shakeTween = DOTween.Shake(() => shakeOffset, x => shakeOffset = x, duration, force, 12, 90, true, true);
    }

}

public enum ScreenMatchType {
    Horizontal,
    Vertical,
    Both
}
