using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MFormat {

    public static string GetLVLString(int level, int maxLevel) {
        if (level == maxLevel) {
            return MLocalization.Get("LVL_MAX_STR");
        } else {
            return MLocalization.Get("LVL_STR", LocalizationGroup.Main, level);
        }
    }
	
}
