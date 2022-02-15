using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class ChestSlotUI : MonoBehaviour {

    public UISimpleButton openButton;
    public TextMeshProUGUI emptySlotText;
    public IconDrawer iconDrawer;
    public TextMeshProUGUI timeLeftText;

    public GD_Chest chest { get; private set; }

    private float animationTime;
    private ChestPanel panel;

    public void Init(ChestPanel panel) {
        this.panel = panel;
    }

    public void InternalUpdate() {
        
    }

    public void ButtonClick_Open() {
        panel.Click_OpenChest(this);
    }

    public void UpdateTime() {
        if (chest != null) {
            if(chest.isOpening) {
                if (chest.IsReadyToOpen()) {
                    openButton.Enable(true);
                    openButton.SetText(MLocalization.Get("OPEN"));
                    openButton.SetBackgroundColor(MAssets.colors[MAssets.COLOR_BUTTON_RED]);

                    timeLeftText.gameObject.SetActive(false);                    

                    if (Time.realtimeSinceStartup > animationTime + 2f) {
                        animationTime = Time.realtimeSinceStartup;

                        iconDrawer.t.DOPunchRotation(new Vector3(0, 0, MRandom.SignedRange(2, 6)), 0.4f, 12);
                    }
                } else {
                    openButton.Enable(true);
                    openButton.SetBackgroundColor(MAssets.colors[MAssets.COLOR_BUTTON_GREEN]);

                    TimeSpan timeLeft = chest.timeLeft;
                    timeLeftText.text = MFormat.GetTMProIcon(TMProIcon.Time) + MFormat.TimeSpan(timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
                    timeLeftText.gameObject.SetActive(true);

                    openButton.SetText(MFormat.GetTMProIcon(TMProIcon.Gem) + chest.gemSkipPrice);
                }
            } else {

            }            
        }
    }

    public void StartOpeningChest() {        
        //timeLeftText.text = chest.openTime.ToShortTimeString();
    }

	public void SetChest(GD_Chest chest) {
        if (chest == this.chest)
            return;

        this.chest = chest;

        emptySlotText.gameObject.SetActive(false);
        iconDrawer.gameObject.SetActive(true);
        iconDrawer.DrawChest(chest);

        if(chest.isOpening) {
            openButton.Enable(false);
        } else {
            openButton.Enable(true);
            openButton.SetBackgroundColor(MAssets.colors[MAssets.COLOR_BUTTON_GREEN]);
            openButton.SetText(MLocalization.Get("OPEN"));
        }        
    }

    public void SetEmpty() {
        chest = null;

        emptySlotText.gameObject.SetActive(true);
        iconDrawer.gameObject.SetActive(false);
        timeLeftText.gameObject.SetActive(false);

        openButton.Enable(false);
    }
	
}
