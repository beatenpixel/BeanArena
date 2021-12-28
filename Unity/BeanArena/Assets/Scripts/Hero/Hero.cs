using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : PoolObject {

	public MoveConfig moveConfig;

	private HeroBody body;
	private List<HeroArm> arms;

	private HeroInput input = new HeroInput();

	private float jumpTimer;

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

			float angle = Vector2.SignedAngle(Vector2.up, input.move.normalized);
			body.motion.SetR(angle, true);

			if (input.move.y > 0.3f && Time.time > jumpTimer) {
				if (body.isGrounded) {
					jumpTimer = Time.time + 0.3f;
					body.SetGrounded(false);

					body.rb.AddForce(input.move * moveConfig.jumpForce);
				}
			}
		} else {
			body.motion.SetR(0f, true);
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
