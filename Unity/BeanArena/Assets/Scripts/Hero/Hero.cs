using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : PoolObject {

	private HeroBody body;
	private List<HeroArm> arms;

	private HeroInput input = new HeroInput();

	protected override void Awake() {
        base.Awake();

		body = GetComponentInChildren<HeroBody>();
		arms = new List<HeroArm>(GetComponentsInChildren<HeroArm>());
    }

    public void Init() {
		
	}
	
	public void InternalStart() {
		
	}
	
	public void InternalUpdate() {
		
	}
	
	public void InternalLateUpdate() {
		
	}
	
	public void InternalFixedUpdate() {
		
	}

    public override Type GetPoolObjectType() {
		return typeof(Hero);
    }

}

public class HeroInput {
	public Vector2 move;
	public bool shoot;
	public bool ult;
}
