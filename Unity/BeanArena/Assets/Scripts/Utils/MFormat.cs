using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MFormat {

    public static string GetCupsStr(int cups, Sign sign) {
        return $"<sprite name=\"cup\">{GetSignStr(sign)}{cups}";
    }

    public static string GetCoinsStr(int cups, Sign sign) {
        return $"<sprite name=\"cup\">{GetSignStr(sign)}{cups}";
    }

    public static string GetSignStr(Sign sign) {
        switch(sign) {
            case Sign.Minus: return "-";
            case Sign.Plus: return "+";
            default:
                return "";
        }
    }

    public static string GetLVLString(int level, int maxLevel) {
        if (level == maxLevel) {
            return MLocalization.Get("LVL_MAX_STR");
        } else {
            return MLocalization.Get("LVL_STR", LocalizationGroup.Main, level);
        }
    }

    public static string GetItemCategoryNameKey(ItemCategory cat) {
        return "ITEM_CATEGORY_" + cat.ToString().ToUpper();
    }

    public enum Sign {
        Minus = -1,
        None = 0,
        Plus = 1
    }
	
}
