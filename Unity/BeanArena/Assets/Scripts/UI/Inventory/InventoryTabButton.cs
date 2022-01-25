using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTabButton : PoolObject {    

	public NotificationDot notificationDot;
	public TextMeshProUGUI tabName;
    public Image tabBackgroundImage;

    public Color activeColor;
    public Color unactiveColor;

    private Action<InventoryTabButton, object> OnClickEvent;
    private object onClickArg;

    private Color startColor;

    protected override void Awake() {
        base.Awake();

        startColor = tabBackgroundImage.color;
    }

    public void SetState(InventoryTabButtonState state) {
        switch(state) {
            case InventoryTabButtonState.Unlocked:

                break;
            case InventoryTabButtonState.Locked:

                break;
            case InventoryTabButtonState.Pressed:

                break;
        }
    }

    public void InvokeOnClick() {
        OnClickEvent?.Invoke(this, onClickArg);
    }

    public void SetOnClick(Action<InventoryTabButton, object> callback, object arg) {
        onClickArg = arg;
        OnClickEvent = callback;
    }

    public void SetActive(bool active) {
        if(active) {
            tabBackgroundImage.color = activeColor;
        } else {
            tabBackgroundImage.color = unactiveColor;
        }
    }

    public override Type GetPoolObjectType() {
        return typeof(InventoryTabButton);   
    }
}

public enum InventoryTabButtonState {
    Unlocked,
    Locked,
    Pressed
}