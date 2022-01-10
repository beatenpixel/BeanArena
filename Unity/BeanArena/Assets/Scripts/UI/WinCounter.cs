using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinCounter : MonoBehaviour {

	public Image[] dots;

	public int wins { get; private set; }
	private int maxWins = 2;

	public void Init(int maxWins) {
		this.maxWins = maxWins;
    }

	public void SetWinsCount(int _wins) {
		this.wins = _wins;

        for (int i = 0; i < dots.Length; i++) {
			if (i < wins) {
				dots[i].color = MAssets.colors["win_dot"];
			} else {
				dots[i].color = MAssets.colors["neutral_dot"];
			}
        }
    }
	
}
