using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoPanel : MonoBehaviour {

    public TextMeshProUGUI heroNameText;
    public TextMeshProUGUI heroDescrText;
    public TextMeshProUGUI heroStatsText;

    public IconDrawer iconDrawer;

    public UISimpleButton upgradeButton;
    
    public void DrawHero(GD_HeroItem item, HeroInfoDrawConfig config) {
        upgradeButton.SetOnClick(config.OnUpgradeButtonClickCallback);

        iconDrawer.DrawHero(item);

        heroNameText.text = MLocalization.Get("BEAN_NAME_" + item.heroType.ToString().ToUpper(), LocalizationGroup.Items);
        heroDescrText.text = "\"" + MLocalization.Get("BEAN_DESCR_" + item.heroType.ToString().ToUpper(), LocalizationGroup.Items) + "\"";

        string statsStr = "";
        for (int i = 0; i < item.info.stats.Count; i++) {
            string lineStr = MFormat.GetTMProIcon(item.info.stats[i].statType);

            lineStr += item.info.stats[i].GetValueStr(item.levelID);

            statsStr += lineStr + "\n";
        }

        heroStatsText.text = statsStr;

        if(item.levelID == item.info.rarenessInfo.maxLevel - 1) {
            upgradeButton.SetText(MLocalization.Get("LVL_MAX_STR"));
            upgradeButton.SetBackgroundColor(MAssets.colors[MAssets.COLOR_BUTTON_MAGENTA]);
            iconDrawer.DrawText("");
        } else {
            HeroLevelInfo heroLevelInfo = item.info.rarenessInfo.levelsInfo[item.levelID];            

            upgradeButton.SetText(MLocalization.Get("UPGRADE_CARD_BUTTON", LocalizationGroup.Main, heroLevelInfo.coinsToLevel));

            if (item.cardsCollected >= heroLevelInfo.cardsToLevel) {
                iconDrawer.DrawText(item.cardsCollected + "/" + heroLevelInfo.cardsToLevel);
            } else {
                iconDrawer.DrawText(MFormat.TextColorTag(MAssets.colors[MAssets.COLOR_BUTTON_RED]) + item.cardsCollected + MFormat.TextColorTagEnd + "/" + heroLevelInfo.cardsToLevel);
            }

            if (item.cardsCollected >= heroLevelInfo.cardsToLevel && Economy.inst.HasCurrency(CurrencyType.Coin, heroLevelInfo.coinsToLevel)) {
                upgradeButton.SetBackgroundColor(MAssets.colors[MAssets.COLOR_BUTTON_GREEN]);
            } else {
                upgradeButton.SetBackgroundColor(MAssets.colors[MAssets.COLOR_BUTTON_RED]);
            }
        }        
    }

}
