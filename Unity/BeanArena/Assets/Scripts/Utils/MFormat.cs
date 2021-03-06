using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MFormat {

    public static string CurrencyToStr(CurrencyType currency, int amount) {
        switch(currency) {
            case CurrencyType.Coin:
                return GetTMProIcon(TMProIcon.Coin) + amount;
            case CurrencyType.Gem:
                return GetTMProIcon(TMProIcon.Gem) + amount;
            default:
                return "noSuchCurrency";
        }
    }

    public static string TextColorTag(Color color) {
        return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">";
    }

    public static string TextColorTagEnd => "</color>";

    private static Dictionary<StatType, TMProIcon> statIcons = new Dictionary<StatType, TMProIcon> {
        {StatType.Health, TMProIcon.Heart },
        {StatType.Damage, TMProIcon.Damage },
        {StatType.Speed, TMProIcon.Heart },
        {StatType.JumpHeight, TMProIcon.Jump },
        {StatType.Duration, TMProIcon.Time },
        {StatType.FusePoints, TMProIcon.Fuse },
        {StatType.Armor, TMProIcon.Armor }
    };

    public static string GetTMProIcon(StatType statType) {
        return GetTMProIcon(statIcons[statType]);
    }

    public static string GetTMProIcon(TMProIcon icon) {
        string statIconStr = "<sprite name=\"";

        switch (icon) {
            case TMProIcon.Coin:
                statIconStr += "coin\">";
            break;
            case TMProIcon.Damage:
                statIconStr += "damage\">";
                break;
            case TMProIcon.Heart:
                statIconStr += "heart\">";
                break;
            case TMProIcon.Time:
                statIconStr += "time\">";
                break;
            case TMProIcon.Jump:
                statIconStr += "jump\">";
                break;
            case TMProIcon.Fuse:
                statIconStr += "coin\">";
                break;
            case TMProIcon.Gem:
                statIconStr += "gem\">";
                break;
            case TMProIcon.Upgrade:
                statIconStr += "upgrade\">";
                break;
            case TMProIcon.Card:
                statIconStr += "cards\">";
                break;
            case TMProIcon.Armor:
                statIconStr += "armor\">";
                break;
            case TMProIcon.Cup:
                statIconStr += "cup\">";
                break;
        }

        return statIconStr;
    }

    public static string TimeSpan(int h, int m, int s) {
        string hStr = ToTwoDigits(h);
        string mStr = ToTwoDigits(m);
        string sStr = ToTwoDigits(s);

        if (h > 0) {
            return MLocalization.Get("TIME_HMS", LocalizationGroup.Main, hStr, mStr, sStr);
            //return MLocalization.Get("TIME_HM", LocalizationGroup.Main, hStr, mStr);
        } else if(m > 0) {
            return MLocalization.Get("TIME_MS", LocalizationGroup.Main, mStr, sStr);
        } else {
            return MLocalization.Get("TIME_S", LocalizationGroup.Main, sStr);
        }
    }

    public static string ToTwoDigits(int d) {
        if(d < 10) {
            return "0" + d;
        } else {
            return d.ToString("");
        }
    }

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

    public static string GetLVLString(int levelID, int maxLevelsCount) {
        if (levelID == maxLevelsCount - 1) {
            return MLocalization.Get("LVL_MAX_STR");
        } else {
            return MLocalization.Get("LVL_STR", LocalizationGroup.Main, (levelID + 1));
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

public enum TMProIcon {
    None,
    Coin,
    Time,
    Cup,
    Fuse,
    Damage,
    Heart,
    Jump,
    Gem,
    Upgrade,
    Card,
    Armor
}
