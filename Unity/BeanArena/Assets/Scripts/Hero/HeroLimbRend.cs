using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLimbRend : MonoBehaviour {

    public Transform rendT;

    public SpriteRenderer baseRendSprite;
    public SpriteRenderer baseRendT;

    [Header("Config")]
    public Axis2D flipOrientatinoAxis;

    private HeroLimb limb;

    private Vector3 startScale;

    private void Awake() {
        startScale = rendT.localScale;
    }

    public void Init(HeroLimb limb) {
        this.limb = limb;
        baseRendSprite.sortingOrder = limbsOrderLayers[limb.limbType];
    }

    public void SetOrientation(Orientation orient) {
        Vector3 newScale = startScale;

        if ((flipOrientatinoAxis & Axis2D.X) != 0) {
            newScale = newScale.SetX(startScale.x * (int)orient);
        }

        if ((flipOrientatinoAxis & Axis2D.Y) != 0) {
            newScale = newScale.SetY(startScale.y * (int)orient);
        }

        rendT.localScale = newScale;
    }

    public void SetBaseColor(Color color) {
        if(limb.limbType == LimbType.RArm) {
            baseRendSprite.color = (color * 0.9f).SetA(1f);
        } else if (limb.limbType == LimbType.LArm) {
            baseRendSprite.color = (color * 1.1f).SetA(1f);
        } else {
            baseRendSprite.color = (color * 1f).SetA(1f);
        }
    }

    public static Dictionary<LimbType, int> limbsOrderLayers = new Dictionary<LimbType, int> {
        {LimbType.Body, 50 },
        {LimbType.LArm, 30 },
        {LimbType.RArm, 70 },
    };

}
