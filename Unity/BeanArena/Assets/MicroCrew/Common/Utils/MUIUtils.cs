using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MUIUtils {
    
	public static void SetMargin(this RectTransform rectT, Vector2 bottomLeft, Vector2 topRight) {
        rectT.offsetMin = new Vector2(bottomLeft.x, bottomLeft.y);
        rectT.offsetMax = new Vector2(-topRight.x, -topRight.y);
    }

}

public enum UIEventType {
    None,
    Click,
    PointerDown,
    PointerUp,
    DragStart,
    DragUpdate,
    DragEnd,
}
