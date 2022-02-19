using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class MExt {

    public static void FillPoints2D(this Bounds bounds, Vector2[] points) {
        points[0] = bounds.min;
        points[1] = bounds.min.SetY(bounds.max.y);
        points[2] = bounds.max;
        points[3] = bounds.max.SetY(bounds.min.y);
    }

    public static void SetAnchor(this RectTransform rectT, Vector2 min, Vector2 max) {
        rectT.anchorMin = min;
        rectT.anchorMax = max;
    }

    public static void SetOffset(this RectTransform rectT, Vector2 offsetMin, Vector2 offsetMax) {
        rectT.offsetMin = offsetMin;
        rectT.offsetMax = offsetMax;
    }

    public static void Log<T>(this IList<T> list, Func<T,string> func) {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Collection log:");
        for (int i = 0; i < list.Count; i++) {
            sb.Append(i); sb.Append(": ");
            sb.AppendLine(func(list[i]));
        }

        Debug.Log(sb.ToString());
    }

    public static void SetLayerRecursively(this GameObject obj, int layer) {
        obj.layer = layer;

        foreach (Transform child in obj.transform) {
            child.gameObject.SetLayerRecursively(layer);
        }
    }

    public static void ShiftFromStart<T>(this T[] arr, T newItem) {
        for (int i = arr.Length - 1; i > 0; i--) {
            arr[i] = arr[i - 1];
        }
        arr[0] = newItem;
    }

    public static void ShiftFromEnd<T>(this T[] arr, T newItem) {
        for (int i = 0; i < arr.Length - 2; i++) {
            arr[i] = arr[i + 1];
        }
        arr[arr.Length - 1] = newItem;
    }

    //========= COLORS ===========

    public static Color SetA(this Color c, float a) {
        c.a = a;
        return c;
    }

    //========= VECTOR_3 ===========

    public static Vector3 AddX(this Vector3 v, float x) {
        v.x += x;
        return v;
    }

    public static Vector3 AddY(this Vector3 v, float y) {
        v.y += y;
        return v;
    }

    public static Vector3 AddZ(this Vector3 v, float z) {
        v.z += z;
        return v;
    }

    public static Vector3 SetX(this Vector3 v, float x) {
        v.x = x;
        return v;
    }

    public static Vector3 SetY(this Vector3 v, float y) {
        v.y = y;
        return v;
    }

    public static Vector3 SetZ(this Vector3 v, float z) {
        v.z = z;
        return v;
    }

    public static Vector3 MulX(this Vector3 v, float x) {
        v.x *= x;
        return v;
    }

    public static Vector3 MulY(this Vector3 v, float y) {
        v.y *= y;
        return v;
    }

    public static Vector3 MulZ(this Vector3 v, float z) {
        v.z *= z;
        return v;
    }

    public static Vector3 InvXYZ(this Vector3 a) {
        a.x = 1 / a.x;
        a.y = 1 / a.y;
        a.z = 1 / a.z;
        return a;
    }

    public static Vector3 MulEach(this Vector3 a, Vector3 b) {
        a.x *= b.x;
        a.y *= b.y;
        a.z *= b.z;
        return a;
    }

    public static Vector3 DivEach(this Vector3 a, Vector3 b) {
        if (b.x != 0) {
            a.x /= b.x;
        } else {
            Debug.LogError("DivisionByZero");
        }
        if (b.y != 0) {
            a.y /= b.y;
        } else {
            Debug.LogError("DivisionByZero");
        }
        if (b.z != 0) {
            a.z /= b.z;
        } else {
            Debug.LogError("DivisionByZero");
        }
        return a;
    }

    //==============================


    //========= VECTOR_4 ===========

    public static Vector2 Vec2XY(this Vector4 a) {
        return new Vector2(a.x, a.y);
    }

    public static Vector2 Vec2ZW(this Vector4 a) {
        return new Vector2(a.z, a.w);
    }

    public static Vector3 Vec3XY(this Vector4 a) {
        return new Vector3(a.x, a.y, 0);
    }

    public static Vector3 Vec3ZW(this Vector4 a) {
        return new Vector3(a.z, a.w, 0);
    }

    //========= VECTOR_2 ===========

    public static bool IsZero(this Vector2 a) {
        return (Mathf.Abs(a.x) < Mathf.Epsilon && Mathf.Abs(a.y) < Mathf.Epsilon);
    }

    public static Vector2 AddX(this Vector2 v, float x) {
        v.x += x;
        return v;
    }

    public static Vector2 AddY(this Vector2 v, float y) {
        v.y += y;
        return v;
    }

    public static Vector2 SetX(this Vector2 a, float x) {
        a.x = x;
        return a;
    }

    public static Vector2 SetY(this Vector2 a, float y) {
        a.y = y;
        return a;
    }

    public static Vector2 MulXY(this Vector2 a, Vector2 b) {
        a.x *= b.x;
        a.y *= b.y;
        return a;
    }

    public static Vector2 DivXY(this Vector2 a, Vector2 b) {
        a.x /= b.x;
        a.y /= b.y;
        return a;
    }

    public static Vector2 MulX(this Vector2 a, float f) {
        a.x *= f;
        return a;
    }

    public static Vector2 MulY(this Vector2 a, float f) {
        a.y *= f;
        return a;
    }

    public static Vector2 InvXY(this Vector2 a) {
        a.x = 1 / a.x;
        a.y = 1 / a.y;
        return a;
    }

    public static Vector2 FlipXY(this Vector2 a) {
        float y = a.y;
        a.y = a.x;
        a.x = y;
        return a;
    }

    //==============================

    //======== TRANSFORM ===========

    public static TransformData GetTransformData(this Transform t, bool local) {
        if (local) {
            return new TransformData() {
                isLocal = true,
                position = t.localPosition,
                rotation = t.localRotation.eulerAngles,
                scale = t.localScale
            };
        } else {
            return new TransformData() {
                isLocal = false,
                position = t.position,
                rotation = t.rotation.eulerAngles,
                scale = t.lossyScale
            };
        }
    }

    public static void ApplyTransformData(this Transform t, TransformData data) {
        if (data.isLocal) {
            t.localPosition = data.position;
            t.localRotation = Quaternion.Euler(data.rotation);
            t.localScale = data.scale;
        } else {
            t.position = data.position;
            t.rotation = Quaternion.Euler(data.rotation);

            if (t.parent != null) {
                t.localScale = data.scale.DivEach(t.parent.lossyScale);
            } else {
                t.localScale = data.scale;
            }
        }
    }

    //==============================    

}

[System.Serializable]
public struct TransformData {
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public bool isLocal;

    public TransformData(Vector3 position, Vector3 rotation, Vector3 scale, bool isLocal) {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        this.isLocal = isLocal;
    }

    public static TransformData identity = new TransformData(Vector3.zero, Vector3.zero, Vector3.one, true);

}

public static class EnumUtils {
    public static IEnumerable<T> GetValues<T>() {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    public static int EnumValuesCount<T>() {
        return Enum.GetValues(typeof(T)).Length;
    }

}