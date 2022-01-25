using Coffee.UIEffects;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationDot : MonoBehaviour {

    [SerializeField] private RectTransform rootT;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private UIShiny uiShiny;

    private Coroutine coroutine;

    private void OnEnable() {
        coroutine = StartCoroutine(Animate());
    }

    private void OnDisable() {
        if (coroutine != null) {
            StopCoroutine(coroutine);
        }
    }

    public void Enable(bool enable, int? number = null) {
        if(number != null) {
            text.text = ((int)number).ToString();
        }

        gameObject.SetActive(enable);
    }

    private IEnumerator Animate() {
        while(true) {
            yield return new WaitForSecondsRealtime(3f);
            Wiggle();
            yield return new WaitForSecondsRealtime(0.5f);
            Shine();
        }
    }

    private void Wiggle() {
        rootT.DOKill(true);
        rootT.DOPunchScale(new Vector3(-0.1f, 0.15f, 0f), 0.2f, 13);
    }

    private void Shine() {
        uiShiny.Play(true);
    }

}
