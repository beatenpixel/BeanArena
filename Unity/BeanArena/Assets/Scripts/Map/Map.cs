using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Map : MonoBehaviour {

	public Transform groundBaseLineY;

	public List<Hero> heroes;

	[SerializeField] private List<MapArea> areas;

	protected virtual void Awake() {
		areas = new List<MapArea>(FindObjectsOfType<MapArea>());
    }

	public virtual void Init() {
		
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

	public void AddHero(Hero hero) {
		heroes.Add(hero);
    }

	private void UpdateShader() {
		Shader.SetGlobalFloat("_MapBaseLineWorldY", groundBaseLineY.position.y);
		Vector2 screenSpacePos = MCamera.inst.cam.WorldToScreenPoint(groundBaseLineY.position);
		float uvY = screenSpacePos.y / Screen.height;
		Shader.SetGlobalFloat("_MapBaseLineCameraSpaceY", uvY);
	}

	public MapArea GetArea(string name) {
		return areas.Find(x => x.areaName == name);
    }

	public List<MapArea> GetAreas(Func<MapArea,bool> func) {
		List<MapArea> result = new List<MapArea>();
        for (int i = 0; i < areas.Count; i++) {
			if(func(areas[i])) {
				result.Add(areas[i]);
            }
        }

		return result;
    }

	protected virtual void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(groundBaseLineY.position + Vector3.right * -50, groundBaseLineY.position + Vector3.right * 50);
    }
	
}
