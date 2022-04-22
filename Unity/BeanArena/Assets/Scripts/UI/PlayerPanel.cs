using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour {

	public UIHealthbar healthbar;
	public Image icon;
	public TextMeshProUGUI playerNameText;
	public TextMeshProUGUI cupsText;
	public WinCounter winCounter;

    public PanelCircleItem panelCircleItem;

	[HideInInspector] public HeroBase attachedHero;

	public void Init() {
		winCounter.Init(GM_ArenaBot.ROUNDS_TO_WIN);
		winCounter.SetWinsCount(0);
        healthbar.SetValue(1f, true);
    }

	public void SetHero(HeroBase hero) {        
		attachedHero = hero;
        winCounter.SetWinsCount(0);
        healthbar.SetValue(1f, true);

        cupsText.text = MFormat.GetCupsStr(hero.info.mmr, MFormat.Sign.None);
        playerNameText.text = hero.info.nickname;
    }
	
}
