using MicroCrew.Economy;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuTopPanel : MonoBehaviour {

    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI gemsText;

    public void Init() {
        GE_OnCurrencyChanged.Register((x) => {
            if (x.info.HasChanged(CurrencyType.Coin)) {
                (Currency a, Currency b) = x.info.Get(CurrencyType.Coin);
                coinsText.text = b.amount.ToString();
            }

            if (x.info.HasChanged(CurrencyType.Gem)) {
                (Currency a, Currency b) = x.info.Get(CurrencyType.Gem);
                gemsText.text = b.amount.ToString();
            }
        });
    } 

    public void Draw() {
        coinsText.text = Economy.inst.playerInventory.all[CurrencyType.Coin].currency.amount.ToString();
        gemsText.text = Economy.inst.playerInventory.all[CurrencyType.Gem].currency.amount.ToString();
    }

}
