using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Map : MonoBehaviour {

	public Transform groundBaseLineY;

	public List<HeroBase> heroes = new List<HeroBase>();
	public List<ITarget> targets = new List<ITarget>();

    [SerializeField] private List<MapElement> mapElements;
	[SerializeField] private List<MapArea> areas;

	protected virtual void Awake() {
		areas = new List<MapArea>(FindObjectsOfType<MapArea>());
        mapElements = new List<MapElement>(FindObjectsOfType<MapElement>());
    }

	public virtual void Init() {
        for (int i = 0; i < mapElements.Count; i++) {
            mapElements[i].Init();
        }
    }

	protected virtual void OnDestroy() {
        
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

	public void AddHero(HeroBase hero) {
		heroes.Add(hero);
		targets.Add(hero);
	}

    public void RemoveHero(HeroBase hero) {
        hero.DestroyHero();
        heroes.Remove(hero);
    }

    public void ResetMap() {
        for (int i = 0; i < heroes.Count; i++) {
            heroes[i].Revive();
        }
    }

    public void OnGameExit() {
        for (int i = heroes.Count - 1; i >= 0; i--) {
            RemoveHero(heroes[i]);
        }
    }

	public ITarget GetClosestTarget(Vector2 worldPos, Func<ITarget, bool> targetFilter, out TargetAimPoint aimPoint) {
		float minDst = 1000000;
		ITarget closestTarget = null;
		aimPoint = null;

		for (int i = 0; i < targets.Count; i++) {
			if (!targetFilter(targets[i]))
				continue;

			int pc = targets[i].targetAimPoints.Count;

			for (int x = 0; x < pc; x++) {
				Vector2 dd = worldPos - targets[i].targetAimPoints[x].worldPos;
				if(dd.magnitude < minDst) {
					minDst = dd.magnitude;

					closestTarget = targets[i];
					aimPoint = targets[i].targetAimPoints[x];
				}
			}
        }

		return closestTarget;
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

public enum MapType {
    None = -1,
    Arena_ConcreteAndBushes = 100,

}
