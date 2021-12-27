using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIW_Opener {

	protected UIWindow target;
	protected UIW_SessionInfo sessionInfo;

	public virtual void Init(UIWindow _target, UIW_SessionInfo _sessionInfo) {
		target = _target;
		sessionInfo = _sessionInfo;
	}

	public abstract void Open(bool open);

}

public class IGNWindowSimpleOpener : UIW_Opener {

	private Vector3 startScale;
	private bool isOpened;
	private bool isChangingOpenState;

	public override void Init(UIWindow _target, UIW_SessionInfo _sessionInfo) {
		base.Init(_target, _sessionInfo);

		startScale = target.rectT.localScale;
	}

	public override void Open(bool open) {
		if (isChangingOpenState)
			return;

		if (open) {
			if (isOpened)
				return;

			target.OnStartOpening();
			sessionInfo.OnWindowStateChanged?.Invoke(UIWindowState.OPENING);

			isChangingOpenState = true;
			target.rectT.localScale = startScale * 0.8f;
			target.rectT.DOScale(startScale, 0.2f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(OnOpened);
		} else {
			if (!isOpened)
				return;

			target.OnStartClosing();
			sessionInfo.OnWindowStateChanged?.Invoke(UIWindowState.CLOSING);

			isChangingOpenState = true;
			OnClosed();
			//target.rectT.DOScale(startScale*0.8f, 0.1f).SetEase(Ease.OutBack).OnComplete(OnClosed);
		}
	}

	private void OnOpened() {
		isChangingOpenState = false;
		isOpened = true;
		target.OnOpened();
		sessionInfo.OnWindowStateChanged?.Invoke(UIWindowState.OPENED);
	}

	private void OnClosed() {
		isChangingOpenState = false;
		isOpened = false;
		target.OnClosed();
		sessionInfo.OnWindowStateChanged?.Invoke(UIWindowState.CLOSED);
		target.Push();
	}

}