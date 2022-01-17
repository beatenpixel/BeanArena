using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIGroupAppear : UIGroupBase {

	[SerializeField] private float hideCanvasGroupAlpha = 0f;
	[SerializeField] private Vector2 inOutTransitionTime = new Vector2(0.2f, 0.2f);
	private float startCanvasGroupAlpha;
	private Vector3 startScale;

	protected override void Awake() {
		base.Awake();

		startCanvasGroupAlpha = canvasGroup.alpha;
		startScale = rectT.localScale;
	}

	protected override void Show() {
		canvasGroup.alpha = hideCanvasGroupAlpha;
		canvasGroup.DOFade(startCanvasGroupAlpha, inOutTransitionTime.x).SetUpdate(true).OnComplete(stateSwitcher.FinishSwitch);
		rectT.localScale = startScale * 0.8f;
		rectT.DOScale(startScale, inOutTransitionTime.x).SetUpdate(true);
	}

	protected override void Hide() {
		canvasGroup.DOFade(hideCanvasGroupAlpha, inOutTransitionTime.y).SetUpdate(true).OnComplete(() => {
			go.SetActive(false);
			stateSwitcher.FinishSwitch();
		});
	}

    protected override void Interrupt() {
		canvasGroup.DOKill(true);
		rectT.DOKill(true);
    }

}
