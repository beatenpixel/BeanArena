using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveUIGroup : UIGroupBase {

	public Vector2 targetAnchoredPos;

	private Vector2 startAnchoredPos;
	private Vector3 startLocalScale;
	private Vector2 startSizeDelta;

	protected override void Awake() {
		base.Awake();

		startAnchoredPos = rectT.anchoredPosition;
		startLocalScale = rectT.localScale;
		startSizeDelta = rectT.sizeDelta;
	}

	protected override void Show() {
		rectT.DOAnchorPos(targetAnchoredPos, 0.3f).SetUpdate(true).OnComplete(stateSwitcher.FinishSwitch);
	}

	protected override void Hide() {
		rectT.DOAnchorPos(startAnchoredPos, 0.3f).SetUpdate(true).OnComplete(() => {
			go.SetActive(false);
			stateSwitcher.FinishSwitch();
		});
	}

    protected override void Interrupt() {
		rectT.DOKill(true);
    }

}
