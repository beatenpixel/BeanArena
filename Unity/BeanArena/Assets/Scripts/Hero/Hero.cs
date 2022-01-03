using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : PoolObject {

	public MoveConfig moveConfig;

	private HeroBody body;
	private List<HeroArm> arms;
	private List<HeroLimb> limbs;

	private HeroInput input = new HeroInput();

	private float jumpTimer;

	private WUI_Text nicknameText;

	public Orientation orientation { get; private set; }

	protected override void Awake() {
        base.Awake();

		body = GetComponentInChildren<HeroBody>();
		arms = new List<HeroArm>(GetComponentsInChildren<HeroArm>());

		limbs = new List<HeroLimb>();
		limbs.Add(body);
		limbs.AddRange(arms);

        for (int i = 0; i < limbs.Count; i++) {
			limbs[i].Init();
        }
	}

	public virtual void InitInFactory(HeroConfig config) {
		if(config.role == HeroRole.Enemy) {
			WUI_TextStyle style = WUI_TextStyle.beanNickname;
			style.textColor = Color.white.SetA(0.3f);
			nicknameText = WorldUI.inst.AddText(config.nickname, body.transform, Vector2.up * 1.5f, style);

			t.localScale = Vector3.one * 1.1f;
		}

		Color beanColor = MAssets.colors["bean_team_" + config.teamID];
		
        for (int i = 0; i < limbs.Count; i++) {
			limbs[i].rend.SetBaseColor(beanColor);
			limbs[i].gameObject.layer = LayerMask.NameToLayer("team" + config.teamID);
		}

		SetOrientation(config.orientation);
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

		if(input.arm.magnitude > 0.2f) {
			float armAngle = Vector2.SignedAngle(Vector2.right, input.arm.normalized);

			for (int i = 0; i < arms.Count; i++) {
				arms[i].motion.SetR(armAngle, true).SetS(20f);
			}
		} else {
			for (int i = 0; i < arms.Count; i++) {
				arms[i].motion.SetR(0f, true).SetS(0f);
			}
		}
	}

	public void SetOrientation(Orientation o) {
		orientation = o;

        for (int i = 0; i < limbs.Count; i++) {
			limbs[i].rend.SetOrientation(orientation);
        }
    }

	public void MoveInput(Vector2 inp) {
		input.move = inp;
	}

	public void ArmInput(Vector2 inp) {
		int s = MMath.SignInt(inp.x);
		if ((int)orientation != s && s != 0) {
			SetOrientation((Orientation)s);
		}

		input.arm = inp;
	}

	public override Type GetPoolObjectType() {
		return typeof(Hero);
    }

}

public class HeroInput {
	public Vector2 move;
	public Vector2 arm;
	public bool shoot;
	public bool ult;
}

[System.Serializable]
public class MoveConfig {
	public float moveForce = 200f;
	public float jumpForce = 800f;
}

public enum HeroRole {
	Player,
	Enemy,
	NPC
}

[Flags]
public enum Axis2D {
	X = 1,
	Y = 2,
}

public enum Orientation {
	Left = -1,
	Right = 1,
}