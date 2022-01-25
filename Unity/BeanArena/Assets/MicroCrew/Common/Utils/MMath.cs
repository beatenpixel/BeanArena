using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MMath {

    public static float SinAnimation(float startTime, float time, float speed, float minValue, float maxValue) {
        float p = Mathf.Sin((time - startTime) * speed - Mathf.PI * 0.5f) * 0.5f + 0.5f;
        return Mathf.Lerp(minValue, maxValue, p);
    }

    public static float RoundToAccuracy(float value, float acc, bool upOrDown) {
        return value - (value % acc) + (upOrDown?acc:0f);
    }

    public static int RoundToAccuracy(int value, int acc, bool upOrDown) {
        return value - (value % acc) + (upOrDown ? acc : 0);
    }

    public static double RoundFirstSignificantDigit(this double input) {
        int precision = 0;
        var val = input;
        while (Math.Abs(val) < 1) {
            val *= 10;
            precision++;
        }
        return Math.Round(input, precision);
    }

    public static float Loop(float v, float min, float max) {
        if (v > max) {
            return min + (v - max);
        } else if (v < min) {
            return max + (v - min);
        } else {
            return v;
        }
    }

    public static int Loop(int v, int min, int max) {
        Debug.Log($"v{v} min{min} max{max}");

        if (v > max) {
            return min + (v - max) - 1;
        } else if (v < min) {
            return max + (v - min) + 1;
        } else {
            return v;
        }
    }

    public static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt) {
        lineDir.Normalize();//this needs to be a unit vector
        var v = pnt - linePnt;
        var d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;
    }

    public static Vector2 Project(Vector2 line1, Vector2 line2, Vector2 toProject) {
        float m = (float)(line2.y - line1.y) / (line2.x - line1.x);
        float b = (float)line1.y - (m * line1.x);

        float x = (m * toProject.y + toProject.x - m * b) / (m * m + 1);
        float y = (m * m * toProject.y + m * toProject.x + b) / (m * m + 1);

        return new Vector2(x, y);
    }

    public static int CeilAwayFrom0Int(float a) {
        return SignInt(a) * Mathf.CeilToInt(Mathf.Abs(a));
    }

    public static int SignInt(float a) {
        if (a == 0) {
            return 0;
        } else if (a > 0) {
            return 1;
        } else {
            return -1;
        }
    }

    public static Vector2 PerpendicularCW(this Vector2 vector2) {
        return new Vector2(vector2.y, -vector2.x).normalized;
    }

    public static Vector2 PerpendicularCCW(this Vector2 vector2) {
        return new Vector2(-vector2.y, vector2.x).normalized;
    }

}
