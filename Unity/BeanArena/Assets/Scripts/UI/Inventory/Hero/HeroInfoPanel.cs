using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoPanel : MonoBehaviour {

    public TextMeshProUGUI heroNameText;
    public TextMeshProUGUI heroDescrText;

    public IconDrawer iconDrawer;
    
    public void DrawHero(GD_HeroItem item) {
        heroNameText.text = MLocalization.Get("BEAN_NAME_" + item.heroType.ToString().ToUpper(), LocalizationGroup.Items);
        heroDescrText.text = "\"" + MLocalization.Get("BEAN_DESCR_" + item.heroType.ToString().ToUpper(), LocalizationGroup.Items) + "\"";
        iconDrawer.DrawHero(item);
    }

}
