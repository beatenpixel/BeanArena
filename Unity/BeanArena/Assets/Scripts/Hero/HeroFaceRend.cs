using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroFaceRend : MonoBehaviour {

	public SpriteRenderer faceSpriteRend;

	public void Init() {
		faceSpriteRend.sortingOrder = HeroLimbRend.limbsOrderLayers[LimbType.Body] + 1;
    }

}

public enum HeroFace {
	None,
	Normal,
	Angry,
	Laugh
}
