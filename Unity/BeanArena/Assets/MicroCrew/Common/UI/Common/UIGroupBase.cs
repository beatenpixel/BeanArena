using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIGroupBase : MonoBehaviour {

	[SerializeField] protected GameObject go;
	[SerializeField] protected RectTransform rectT;
	[SerializeField] protected CanvasGroup canvasGroup;
	[SerializeField] protected bool enabledInitially;
	[SerializeField] private bool canBeInterupted = true;

	protected BinaryStateSwitcher stateSwitcher;
	protected bool didAwake;

	protected virtual void Awake() {
		if (!didAwake) {
			stateSwitcher = new BinaryStateSwitcher(enabledInitially);
			didAwake = true;
		}
	}

	public bool Show(bool show) {
		if (!didAwake) {
			Awake();
		}

		if (stateSwitcher.Switch(show)) {
			if (show) {
				go.SetActive(true);
				Show();
			} else {
				Hide();
			}
			return true;
		} else {
			if (canBeInterupted) {
				stateSwitcher.FinishSwitch();
				Interrupt();
				stateSwitcher.SetStateForced(show);

				if (show) {
					go.SetActive(true);
					Show();
				} else {
					Hide();
				}
				return true;
			} else {
				return false;
			}
		}
	}

	protected abstract void Interrupt();
	protected abstract void Show();
	protected abstract void Hide();

}
