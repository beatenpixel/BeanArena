using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class WUI_SpriteHealthbar : PoolObject {

	public Transform frameT;
	public SpriteRenderer frameRend;
	public Transform fillT;
	public SpriteRenderer fillRend;
	public Gradient gradientColor;
	public Color blinkColor = Color.white;

	public float value {
		get; private set;
	}
	private float valueLerp;

	private Vector2 startFillSize;
	private Tween blinkTween;
	private Tween rectTween;
	private bool isBlinking;
	private float blinkAmount;

	private Transform target;
	private Vector3 offset;

	public void Init(float startValue, Transform _target, Vector2 _offset, float width = 4f) {
		value = startValue;
		valueLerp = startValue;
		target = _target;
		offset = _offset;
		fillRend.color = gradientColor.Evaluate(startValue);
		frameRend.size = frameRend.size.SetX(width);
		fillRend.size = fillRend.size.SetX(width * 0.9f);
		startFillSize = fillRend.size;
	}

	public void InternalUpdate() {
		if (target != null) {
			frameT.position = target.position + offset;
		} else {
			WorldUI.inst.RemoveRemoveSpriteHealthbar(this);
		}
	}

	public void SetValue(float v, bool instant) {
		v = Mathf.Clamp(v, 0, 1);
		if (v == value)
			return;

		if (instant) {
			KillTweens();
			value = v;

			SetFillPercent(v);
		} else {
			KillTweens();

			if (v < value) {
				isBlinking = true;
				fillRend.color = blinkColor;
				blinkAmount = 1f;
				blinkTween = DOTween.To(() => 1f, (x) => {
					blinkAmount = x;
					fillRend.color = Color.Lerp(gradientColor.Evaluate(valueLerp), blinkColor, blinkAmount);
				}, 0f, 0.15f).SetDelay(0.15f).OnComplete(() => isBlinking = false);
			}

			valueLerp = v;
			value = v;

			rectTween = DOTween.To(() => valueLerp, x => valueLerp = x, v, 0.2f).OnUpdate(() => {
				SetFillPercent(valueLerp);
			});
		}
	}

	private void SetFillPercent(float p) {
		fillRend.size = new Vector2(startFillSize.x * p, startFillSize.y);
		fillT.localPosition = new Vector3(-(startFillSize.x - fillRend.size.x) * 0.5f, 0f, 0f);
		if (!isBlinking) {
			fillRend.color = gradientColor.Evaluate(p);
		}
	}

	private void KillTweens() {
		if (blinkTween != null) {
			blinkTween.Kill();
		}

		if (rectTween != null) {
			rectTween.Kill();
		}
	}

	public override Type GetPoolObjectType() {
		return typeof(WUI_SpriteHealthbar);
	}

}