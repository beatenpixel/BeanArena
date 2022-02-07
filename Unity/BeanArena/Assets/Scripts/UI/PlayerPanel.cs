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

	[HideInInspector] public Hero attachedHero;

	public void Init() {
		winCounter.Init(2);
		winCounter.SetWinsCount(0);
        healthbar.SetValue(1f, true);
    }

	public void SetHero(Hero hero) {        
		attachedHero = hero;
        winCounter.SetWinsCount(0);
        healthbar.SetValue(1f, true);

        cupsText.text = MFormat.GetCupsStr(hero.info.mmr, MFormat.Sign.None);
        playerNameText.text = hero.info.nickname;
    }
	
}
