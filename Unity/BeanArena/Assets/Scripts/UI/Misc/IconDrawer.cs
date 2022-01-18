using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class IconDrawer : MonoBehaviour {

	public RectTransform rectT;
	public Image image;	

	[SerializeField] private SO_IconContent config;

	public void SetIcon(SO_IconContent _config ) {
        config = _config;
    }

    public void DrawIcon() {
        if (config != null) {
            rectT.anchorMin = config.anchorMin;
            rectT.anchorMax = config.anchorMax;
            rectT.offsetMin = config.offsetMin;
            rectT.offsetMax = -config.offsetMax;

            image.sprite = config.sprite;
            image.color = config.color;
        } else {
            Debug.LogError("IconDrawer: No icon to draw");
        }
    }

#if UNITY_EDITOR
    private void Update() {
        if(!Application.isPlaying) {
            if (config != null) {
                if (transform.hasChanged) {
                    DrawIcon();
                }
            }
        }
    }
#endif

}
