using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Map : MonoBehaviour {

	public Transform groundBaseLineY;

	public List<Hero> heroes;

	public virtual void Init() {
		for (int i = 0; i < heroes.Count; i++) {
			heroes[i].Init();
		}
	}
	
	public virtual void InternalUpdate() {
		UpdateShader();

        for (int i = 0; i < heroes.Count; i++) {
			heroes[i].InternalUpdate();
        }
	}

	public virtual void InternalFixedUpdate() {
		for (int i = 0; i < heroes.Count; i++) {
			heroes[i].InternalFixedUpdate();
		}
	}

	private void UpdateShader() {
		Shader.SetGlobalFloat("_MapBaseLineWorldY", groundBaseLineY.position.y);
		Vector2 screenSpacePos = MCamera.inst.cam.WorldToScreenPoint(groundBaseLineY.position);
		float uvY = screenSpacePos.y / Screen.height;
		Shader.SetGlobalFloat("_MapBaseLineCameraSpaceY", uvY);
	}

	protected virtual void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(groundBaseLineY.position + Vector3.right * -50, groundBaseLineY.position + Vector3.right * 50);
    }
	
}
