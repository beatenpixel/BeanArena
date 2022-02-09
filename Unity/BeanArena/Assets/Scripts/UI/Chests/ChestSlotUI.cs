using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChestSlotUI : MonoBehaviour {

    public IconDrawer iconDrawer;
    public TextMeshProUGUI timeLeftText;

	public void SetChest(GD_Chest chest) {
        iconDrawer.DrawChest(chest, chest.info);
        timeLeftText.text = "time";
    }
	
}
