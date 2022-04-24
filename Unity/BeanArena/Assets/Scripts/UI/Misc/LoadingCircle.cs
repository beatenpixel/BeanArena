using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCircle : MonoBehaviour
{
    private RectTransform rectT;

    private void Awake() {
        rectT = GetComponent<RectTransform>();
    }

    private void Update() {
        rectT.rotation *= Quaternion.Euler(0, 0, -180f * Time.unscaledDeltaTime);
    }

}
