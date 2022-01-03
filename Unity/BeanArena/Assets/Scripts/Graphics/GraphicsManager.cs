using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsManager : MonoBehaviour {

    private static bool didSetDefaultResolution;
    public static Resolution defaultResolution;

    public void ApplyGraphicsPreset(GraphicsPreset preset) {
        QualitySettings.SetQualityLevel((int)preset, true);
        //MCamera.inst.ApplyGraphicsPreset(preset);

        if (!didSetDefaultResolution) {
            didSetDefaultResolution = true;
            defaultResolution = Screen.currentResolution;
        }

        switch (preset) {
            case GraphicsPreset.Fast:
                Screen.SetResolution(Mathf.RoundToInt(defaultResolution.width * 0.5f), Mathf.RoundToInt(defaultResolution.height * 0.5f), true, 30);
                Application.targetFrameRate = 30;
                break;
            case GraphicsPreset.Medium:
                Screen.SetResolution(defaultResolution.width, defaultResolution.height, true, 60);
                Application.targetFrameRate = 60;
                break;
            case GraphicsPreset.Fancy:
                Screen.SetResolution(defaultResolution.width, defaultResolution.height, true, 60);
                Application.targetFrameRate = 60;
                break;
        }
    }

}

public enum GraphicsPreset {
    Fast,
    Medium,
    Fancy,
    Count
}