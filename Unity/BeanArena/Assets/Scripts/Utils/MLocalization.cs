using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class MLocalization : Singleton<MLocalization> {
    public override void Init() {

    }

    protected override void Shutdown() {

    }



    public static string Get(string key, LocalizationGroup group = LocalizationGroup.Main) {
        return LocalizationSettings.StringDatabase.GetLocalizedString(group.ToString(), key);
    }

    public static string Get(string key, LocalizationGroup group = LocalizationGroup.Main, params object[] args) {
        return LocalizationSettings.StringDatabase.GetLocalizedString(group.ToString(), key, args);
    }

    public static string OK => Get("OK");
    public static string THANKS_NO => Get("THANKS_NO");
    public static string YES => Get("YES");
    public static string NO => Get("NO");
    public static string RETRY => Get("RETRY");
    public static string ARE_YOU_SURE => Get("ARE_YOU_SURE");
    public static string THANKS => Get("THANKS");

}

public enum LocalizationGroup {
    Main,
    Items
}
