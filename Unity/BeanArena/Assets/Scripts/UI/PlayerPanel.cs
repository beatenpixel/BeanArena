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

	public Hero attachedHero;

	public void Init() {

    }

	public void SetHero(Hero hero) {
		attachedHero = hero;
	}
	
}
