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

    private Timer wiggleTimer;
    private Timer shineTimer;

    private void Start() {
        wiggleTimer = new Timer(MRandom.Range(2f, 4f));
        shineTimer = new Timer(2f);
    }

    private void Update() {
        if(wiggleTimer) {
            wiggleTimer.AddFromNow(MRandom.Range(2f, 4f));

            Wiggle();
        }

        if(shineTimer) {
            shineTimer.AddFromNow();

            Shine();
        }
    }

    private void Wiggle() {
        rootT.DOKill(true);
        rootT.DOPunchScale(new Vector3(-0.1f, 0.15f, 0f), 0.2f, 13);
    }

    private void Shine() {

    }

}
