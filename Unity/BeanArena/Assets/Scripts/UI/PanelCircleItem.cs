using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelCircleItem : MonoBehaviour {

    public Image fillImage;
    [SerializeField] private IconDrawer iconDrawer;

    private Tween animTween;

    public void Init() {

    }

    public void SetWeaponItem(GD_Item item) {
        fillImage.fillAmount = 0f;
        iconDrawer.DrawItemPanelCircle(item, this);
    }

    public void RunReloadAnimation(float duration) {
        if(animTween != null) {
            animTween.Kill(true);
        }

        fillImage.fillAmount = 0f;
        animTween = fillImage.DOFillAmount(1f, duration).OnComplete(() => {
            fillImage.fillAmount = 0f;
        });
    }

}
