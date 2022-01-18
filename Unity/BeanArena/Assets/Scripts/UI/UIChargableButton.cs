using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIChargableButton : UIButtonBase {

    public UIChargableButtonConfig config;

    public Image chargeImage;
    
    public override UIButtonConfig baseConfig => config;

    private float charge;
    public float chargePercent => Mathf.Clamp01(charge / config.chargeTime);

    private Vector2 startAnchoredPos;

    private float newShakeTime;
    private Vector2 shakePos;
    private Vector2 targetShakePos;

    private Color startChargeImageColor;

    public Action<UIChargeableButtonOutput> OnOutput;

    public override Type GetPoolObjectType() {
        return typeof(UIChargableButton);
    }

    protected override void Awake() {
        base.Awake();
        startChargeImageColor = chargeImage.color;
        startAnchoredPos = subRectT.anchoredPosition;
        chargeImage.fillAmount = 0;
    }

    private void Update() {
        if(isPressed) {
            charge = Mathf.Clamp(charge + Time.deltaTime, 0, config.chargeTime);
            chargeImage.fillAmount = Mathf.Clamp01(charge / config.chargeTime);
            chargeImage.color = startChargeImageColor.SetA(chargePercent * startChargeImageColor.a);

            subRectT.localScale = startScale * Mathf.Lerp(1f, config.minScale, chargePercent);

            if (Time.time > newShakeTime) {
                newShakeTime = Time.time + config.shakeFrequency * MRandom.Range(0.7f, 1.3f) * (2 - chargePercent);
                targetShakePos = UnityEngine.Random.insideUnitCircle * config.shakeMagnitude * chargePercent;
            }

            shakePos = Vector2.Lerp(shakePos, targetShakePos, Time.deltaTime * config.shakeSmooth);
            subRectT.anchoredPosition = startAnchoredPos + shakePos;            
        }
    }

    protected override void OnBecomePressed(PointerEventData eventData) {
        base.OnBecomePressed(eventData);

        subRectT.DOKill(true);
        chargeImage.DOKill(true);
        chargeImage.fillAmount = 0f;
        chargeImage.color = startChargeImageColor.SetA(0);
        charge = 0f;
    }

    protected override void OnBecomeUnpressed(PointerEventData eventData) {
        base.OnBecomeUnpressed(eventData);
    }

    protected override void OnClick(PointerEventData eventData) {
        base.OnClick(eventData);

        subRectT.DOKill(true);
        subRectT.anchoredPosition = startAnchoredPos;
        subRectT.localScale = startScale;
        subRectT.DOPunchScale(startScale * 0.15f, 0.15f, 13);
        //subRectT.DOScale(startScale, 0.3f).SetEase(Ease.OutElastic);

        chargeImage.DOKill(true);
        chargeImage.DOFade(0f, 0.15f);

        OnOutput?.Invoke(new UIChargeableButtonOutput() {
            chargePercent = this.chargePercent
        });
    }
}

public struct UIChargeableButtonOutput {
    public float chargePercent;
}

[System.Serializable]
public class UIChargableButtonConfig : UIButtonConfig {
    public float minScale = 0.8f;
    public float chargeTime = 1.5f;
    public float shakeFrequency = 0.1f;
    public float shakeMagnitude = 30f;
    public float shakeSmooth = 30f;
}