using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UICanvas : MonoBehaviour {

	public Canvas canvas;
	public RectTransform canvasT;
	public CanvasScaler canvasScaler;

	public Vector2 referenceCanvasSize {
		get {
			return canvasScaler.referenceResolution;
        }
    }

	public Vector2 realCanvasSize {
		get {
			return canvasT.sizeDelta;
        }
    }

	public Vector2 canvasWorldSize {
		get {
			return new Vector2(canvasT.position.x, canvasT.position.y) * 2f;
        }
    }

	public Vector2 WorldToCanvasPos(Vector2 worldPos) {
		return new Vector2(worldPos.x / canvasT.localScale.x, worldPos.y / canvasT.localScale.y);
	}

	public Vector2 ScreenToCanvaspos(Vector2 screenPos) {
		Vector2 worldPos = MCamera.GetWorldPos2D(screenPos);
		return WorldToCanvasPos(worldPos);
	}

	public Vector2 CanvasToScreenPos(Vector2 canvasPos) {
		return new Vector2(canvasPos.x / canvasT.sizeDelta.x * Screen.width, canvasPos.y / canvasT.sizeDelta.y * Screen.height);
	}

	public float DstByX(Vector2 dd) {
		return dd.magnitude / Screen.width;
	}

}