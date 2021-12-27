using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MDraw : MonoBehaviour {

    public static void RectFill(Vector2 pos, Vector2 size, bool center = true, Color? color = null, float duration = 0f) {
        Vector2 offset = center ? -size * 0.5f : Vector2.zero;
        IMDraw.RectangleFilled2D(new Rect(pos.x + offset.x, Screen.height - pos.y + offset.y, size.x, size.y), color ?? Color.red, duration);
    }

    public static void Rect(Vector2 pos, Vector2 size, bool center = true, Color ? color = null, float duration = 0f) {
        Vector2 offset = center ? -size * 0.5f : Vector2.zero;
        IMDraw.RectangleOutline2D(new Rect(pos.x + offset.x, Screen.height - pos.y + offset.y, size.x, size.y), color ?? Color.red, duration);
    }

    public static void Text(string text, Vector2 pos, int size = 15, Color? color = null, float duration = 0f) {
        IMDraw.Label(pos.x, Screen.height - pos.y, color ?? Color.white, size, LabelPivot.MIDDLE_CENTER, LabelAlignment.CENTER, text, duration);
    }

    public static void TextLeft(string text, Vector2 pos, int size = 15, Color? color = null, float duration = 0f) {
        IMDraw.Label(pos.x, Screen.height - pos.y, color ?? Color.white, size, LabelPivot.LOWER_LEFT, LabelAlignment.LEFT, text, duration);
    }

    public static void TextRight(string text, Vector2 pos, int size = 15, Color? color = null, float duration = 0f) {
        IMDraw.Label(pos.x, Screen.height - pos.y, color ?? Color.white, size, LabelPivot.UPPER_RIGHT, LabelAlignment.RIGHT, text, duration);
    }

    public static void TextShadowed(string text, Vector2 pos, int size = 15, Color? color = null, float duration = 0f) {
        IMDraw.LabelShadowed(pos.x, Screen.height - pos.y, color ?? Color.white, size, LabelPivot.MIDDLE_CENTER, LabelAlignment.CENTER, text, duration);
    }

}
