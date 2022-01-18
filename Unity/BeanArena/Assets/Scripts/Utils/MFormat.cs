using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Polyglot;

public class MFormat {

    public static string GetLVLString(int level, int maxLevel) {
        if (level == maxLevel) {
            return Localization.Get("LVL_MAX_STR");
        } else {
            return Localization.GetFormat("LVL_STR", level);
        }
    }
	
}
