using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : PoolObject {

	public MoveConfig moveConfig;

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

	public void InternalUpdate() {
		
	}
	
	public void InternalFixedUpdate() {
		if (input.move.magnitude > 0.2f) {
			body.rb.AddForce(input.move * moveConfig.moveForce * Time.deltaTime);

			//body.motion.SetR()

			if (body.isGrounded) {
				body.SetGrounded(false);

				body.rb.AddForce(input.move * moveConfig.jumpForce);
			}
		}		
	}

	public void MoveInput(Vector2 inp) {
		input.move = inp;
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

[System.Serializable]
public class MoveConfig {
	public float moveForce = 200f;
	public float jumpForce = 800f;
}
