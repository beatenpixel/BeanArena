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
		winCounter.SetWinsCount(MRandom.Range(0, 3));
    }

	public void SetHero(Hero hero) {
		attachedHero = hero;
	}
	
}
