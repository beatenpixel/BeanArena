using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinCounter : MonoBehaviour {

	public WinCounterDot[] dots;

	public int wins { get; private set; }
	private int maxWins;

	public void Init(int maxWins) {
		this.maxWins = maxWins;
        for (int i = 0; i < dots.Length; i++) {
            if(i < maxWins) {
                dots[i].root.SetActive(true);
            } else {
                dots[i].root.SetActive(false);
            }
        }
    }

	public void SetWinsCount(int _wins) {
		this.wins = _wins;

        for (int i = 0; i < dots.Length; i++) {
			if (i < wins) {
				dots[i].dot.color = MAssets.colors["win_dot"];
			} else {
				dots[i].dot.color = MAssets.colors["neutral_dot"];
			}
        }
    }

    [System.Serializable]
	public class WinCounterDot {
        public GameObject root;
        public Image dot;
    }

}
