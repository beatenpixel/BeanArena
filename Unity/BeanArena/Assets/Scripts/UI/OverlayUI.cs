using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayUI : MonoBehaviour {

    public static OverlayUI inst;

    public RectTransform noInternetIcon;
    private float noIntenretActivateTime;

    public void Init() {
        inst = this;

        MGameLoop.Update.Register(InternalUpdate);

        BeanNetwork.inst.OnInternetConnectionChanged += OnInternetConnectionChanged;
    }

    private void InternalUpdate() {
        if (noInternetIcon.gameObject.activeInHierarchy) {
            noInternetIcon.localScale = Vector3.one * (1 + Mathf.Sin((Time.time - noIntenretActivateTime) * 3f) * 0.05f);
        }
    }

    private void OnInternetConnectionChanged(bool isConnected) {
        if (isConnected) {
            noInternetIcon.gameObject.SetActive(false);
        } else {
            noIntenretActivateTime = Time.time;
            noInternetIcon.gameObject.SetActive(true);
        }
    }

}
