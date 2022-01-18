using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemButton : UIButtonBase {

    public IconDrawer iconDrawer;

    [SerializeField] private ItemButtonConfig config;
    public override UIButtonConfig baseConfig => config;

    private Vector2 pointerPressPos;
    private bool isDragging;
    private float pressStartTime;

    private void Update() {
        if(isPressed) {
            Vector2 newPointerPos = Input.mousePosition;
            Vector2 dd = (newPointerPos - pointerPressPos);
            
            if(dd.x > Screen.width * 0.05f || (Time.time > pressStartTime + 0.5f && dd.magnitude < Screen.width * 0.05f)) {
                StartDrag();
            }
        }

        if(isDragging) {
            subRectT.position = Input.mousePosition;

            if(Input.GetMouseButtonUp(0)) {
                StopDrag();
            }
        }
    }

    private void StartDrag() {
        if (isDragging)
            return;

        subRectT.SetParent(UIWindowManager.inst.uiCanvas.canvasT);
        subRectT.SetAsLastSibling();

        isDragging = true;
    }

    private void StopDrag() {
        subRectT.SetParent(rectT);
        subRectT.anchoredPosition = startAnchoredPosition;
        isDragging = false;
    }

    protected override void OnBecomePressed(PointerEventData eventData) {
        base.OnBecomePressed(eventData);

        pressStartTime = Time.time;
        pointerPressPos = eventData.position;
        isDragging = false;

        //subRectT.DOKill(true);
        //subRectT.DOScale(startScale * config.pressScale, config.pressDuration).SetUpdate(true).SetEase(Ease.OutBack);
    }

    protected override void OnBecomeUnpressed(PointerEventData eventData) {
        base.OnBecomeUnpressed(eventData);

        //subRectT.DOKill(true);
        //subRectT.DOScale(startScale, config.pressDuration).SetUpdate(true).SetEase(Ease.OutBack);
    }

    protected override void OnClick(PointerEventData eventData) {
        base.OnClick(eventData);

        MSound.Play("click", SoundConfig.randVolumePitch01);
    }

}

[System.Serializable]
public class ItemButtonConfig : UIButtonConfig {
    public float pressScale = 0.9f;
    public float pressDuration;
}
