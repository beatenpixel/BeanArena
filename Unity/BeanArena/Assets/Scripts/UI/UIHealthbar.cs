using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIHealthbar : MonoBehaviour {

	public float defaultValue;

	public RectTransform holderRectT;
	public Image fillImg;
	public RectTransform fillRectT;
	public Color startColor = Color.red;
	public Color blinkColor = Color.white;

	public float value {
		get; private set;
	}
	[SerializeField] private float valueLerp;

	[SerializeField] private Vector2 startFillSize;
	private Tween colorTween;
	private Tween rectTween;

    private bool isInitialized;

	private void Awake() {
        InternalInit();
        SetValue(defaultValue, true);
	}

    private void InternalInit() {
        if (isInitialized) {
            return;
        }

        startFillSize = fillRectT.rect.size;
        fillRectT.anchorMin = new Vector2(0, 0.5f);
        fillRectT.anchorMax = new Vector2(0, 0.5f);

        isInitialized = true;
    }

	public void SetValue(float v, bool instant) {
        InternalInit();

        v = Mathf.Clamp(v, 0, 1);

        if (v == value) {
            if(instant) {
                fillRectT.sizeDelta = startFillSize.MulX(value);
            }

            return;
        }

		if (instant) {
			KillTweens();
			value = v;

			fillRectT.sizeDelta = startFillSize.MulX(value);
        } else {
			KillTweens();

			if (v < value) {
				fillImg.color = blinkColor;
				colorTween = fillImg.DOColor(startColor, 0.2f).SetUpdate(true).SetDelay(0.15f);
			} else {
				colorTween = fillImg.DOColor(startColor, 0.2f).SetUpdate(true);
			}

			valueLerp = value;
			value = v;

			rectTween = DOTween.To(() => valueLerp, x => valueLerp = x, v, 0.2f).SetUpdate(true).OnUpdate(() => {
				fillRectT.sizeDelta = startFillSize.MulX(valueLerp);
			});
        }
	}

	private void KillTweens() {
		if (colorTween != null) {
			colorTween.Kill();
		}

		if (rectTween != null) {
			rectTween.Kill();
		}
	}

}
