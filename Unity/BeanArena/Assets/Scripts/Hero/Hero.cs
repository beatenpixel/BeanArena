using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hero : PoolObject, IDamageable, ITarget {

	public HeroInfo info { get; private set; }

	public MoveConfig moveConfig;

	private HeroFaceRend faceRend;
	[HideInInspector] public HeroBody body;
	private List<HeroArm> arms;
	private List<HeroLimb> limbs;

	private HeroInput input = new HeroInput();

	private float jumpTimer;

	private WUI_Text nicknameText;

	public Orientation orientation { get; private set; }

	private TargetAim targetAim;

	// ITarget
	private TargetInfo m_TargetInfo = new TargetInfo() {
		type = TargetType.Hero
	};

	private List<TargetAimPoint> m_TargetAimPoints = new List<TargetAimPoint>();

	public TargetInfo targetInfo => m_TargetInfo;
    public List<TargetAimPoint> targetAimPoints => m_TargetAimPoints;
	// ITarget

	private bool hadMoveInputLastFrame;

    protected override void Awake() {
        base.Awake();

		body = GetComponentInChildren<HeroBody>();
		arms = new List<HeroArm>(GetComponentsInChildren<HeroArm>());
		faceRend = GetComponentInChildren<HeroFaceRend>();

		limbs = new List<HeroLimb>();
		limbs.Add(body);
		limbs.AddRange(arms);

        for (int i = 0; i < limbs.Count; i++) {
			limbs[i].Init();
        }

		faceRend.Init();

		m_TargetAimPoints.Add(new TargetAimPoint() {
			worldPos = body.transform.position
		});
	}

	public virtual void InitInFactory(HeroConfig config) {
		info = new HeroInfo();
		info.maxHealth = 100;
		info.health = 100;
		info.teamID = config.teamID;
		info.state = HeroState.Alive;
		info.role = config.role;

		if (config.role == HeroRole.Enemy) {
			WUI_TextStyle style = WUI_TextStyle.beanNickname;
			style.textColor = Color.white.SetA(0.3f);
			nicknameText = WorldUI.inst.AddText(config.nickname, body.transform, Vector2.up * 1.5f, style);

			t.localScale = Vector3.one * 1.1f;
		}

		Color beanColor = MAssets.colors["bean_team_" + config.teamID];
		
        for (int i = 0; i < limbs.Count; i++) {
			limbs[i].rend.SetBaseColor(beanColor);
			limbs[i].gameObject.layer = Game.TeamIDToLayer(config.teamID);
		}

		gameObject.layer = Game.TeamIDToLayer(config.teamID);

		SetOrientation(config.orientation);
	}

    public void Init() {
		
	}

	public void InternalUpdate() {
		targetAimPoints[0].worldPos = body.transform.position;
	}
	
	public void InternalFixedUpdate() {
		if (info.state == HeroState.Dead) {
			return;
		}

        for (int i = 0; i < limbs.Count; i++) {
			limbs[i].InternalFixedUpdate();
        }

		if (targetAim != null) {
			Vector2 bodyDir = body.transform.right * (int)orientation;
			Vector2 enemyDD = targetAim.aimPoint.worldPos - (Vector2)arms[0].transform.position;

			if (info.role != HeroRole.Player) {
				ArmInput(body.transform.right * MMath.CeilAwayFrom0Int(enemyDD.x));
			}

			if (Mathf.Abs(enemyDD.x) > 1f) {
				SetOrientationFromVector2(enemyDD);
			}
		}

		float armAngle2 = Vector2.SignedAngle(Vector2.right, input.arm.normalized);

		for (int i = 0; i < arms.Count; i++) {
			arms[i].motion.SetR(armAngle2 + i * 10, true).SetS(500);
		}

		if (input.move.magnitude > 0.2f) {
			if (!body.isGrounded) {
				body.rb.AddForce(input.move * moveConfig.moveForce * Time.deltaTime);
			}

			float angle = Vector2.SignedAngle(Vector2.up, input.move.normalized);

			body.motion.SetR(angle, true);

			if (input.move.y > 0.3f && Time.time > jumpTimer) {
				if (body.isGrounded) {
					jumpTimer = Time.time + 0.5f;
					body.SetGrounded(false);

					body.rb.AddForce(input.move * moveConfig.jumpForce);
				}
			}

			if (!hadMoveInputLastFrame) {
				SetGravityLevelForBodyParts(0.5f);
			}

			hadMoveInputLastFrame = true;
		} else {
			body.motion.SetR(0f, true);

			if (hadMoveInputLastFrame) {
				SetGravityLevelForBodyParts(0.8f);
			}

			hadMoveInputLastFrame = false;
		}

		if (false) {

			if (targetAim != null /* && input.move.magnitude > 0.2f*/) {
				Vector2 p = targetAim.aimPoint.worldPos;
				Vector2 dd = p - (Vector2)arms[0].transform.position;

				if (dd.magnitude <= 30f) {
					ArmInput(dd.normalized);
				} else {
					ArmInput(Vector2.zero);
				}
			} else {
				ArmInput(Vector2.zero);
			}

			if (input.arm.magnitude > 0.2f) {
				float armAngle = Vector2.SignedAngle(Vector2.right, input.arm.normalized);

				for (int i = 0; i < arms.Count; i++) {
					arms[i].motion.SetR(armAngle + i * 15, true).SetS(15f);

					continue;

					if (arms[i].limbType != LimbType.RArm) {
						arms[i].motion.SetR(armAngle + i * 15, true).SetS(15f);
					} else {
						arms[i].motion.SetR(0f, true).SetS(0f);
					}
				}
			} else {
				for (int i = 0; i < arms.Count; i++) {
					arms[i].motion.SetR(0f, true).SetS(0f);
				}
			}

		} else {
			if (false) {
				if (targetAim != null) {
					Vector2 bodyDir = body.transform.right * (int)orientation;
					Vector2 enemyDD = targetAim.aimPoint.worldPos - (Vector2)arms[0].transform.position;

					ArmInput(body.transform.right * MMath.CeilAwayFrom0Int(enemyDD.x));

					if (Mathf.Abs(enemyDD.x) > 1f) {
						SetOrientationFromVector2(enemyDD);
					}

					float armAngle = Vector2.SignedAngle(Vector2.right, input.arm.normalized);

					for (int i = 0; i < arms.Count; i++) {
						arms[i].motion.SetR(armAngle + i * 15, true).SetS(500);
					}
				} else {
					ArmInput(body.transform.right * (int)orientation);

					float armAngle = Vector2.SignedAngle(Vector2.right, input.arm.normalized);

					for (int i = 0; i < arms.Count; i++) {
						arms[i].motion.SetR(armAngle + i * 15, true).SetS(500);
					}
				}
			}

			
		}

		/*
		bool autoAim = false;

		if (autoAim) {
			bool resetArms = false;

			if(targetAim != null) {
				Vector2 p = targetAim.aimPoint.worldPos;
				Vector2 dd = p - (Vector2)arms[0].transform.position;

				if(dd.magnitude <= 7f) {
					float armAngle = Vector2.SignedAngle(Vector2.right, dd.normalized);

					for (int i = 0; i < arms.Count; i++) {
						arms[i].motion.SetR(armAngle + i * 15f, true).SetS(15f);
					}
				} else {
					resetArms = true;
                }
            } else {
				resetArms = true;
            }

			if(resetArms) {
				for (int i = 0; i < arms.Count; i++) {
					arms[i].motion.SetR(0f, true).SetS(0f);
				}
			}
		} 
		*/

	}

	private void SetGravityLevelForBodyParts(float grav) {
		for (int i = 0; i < limbs.Count; i++) {
			if (limbs[i].rb != null) {
				limbs[i].rb.gravityScale = grav;
			}
		}
	}

	public void SetTarget(ITarget _target, TargetAimPoint _aimPoint = null) {
		if(_aimPoint == null) {
			_aimPoint = _target.targetAimPoints[0];
        }

		targetAim = new TargetAim() {
			target = _target,
			aimPoint = _aimPoint
		};
	}

	public void SetOrientation(Orientation o) {
		orientation = o;

        for (int i = 0; i < limbs.Count; i++) {
			limbs[i].SetOrientation(orientation);
        }
    }

	public void MoveInput(Vector2 inp) {
		if (info.state != HeroState.Alive) {
			return;
		}

		input.move = inp;
	}

	public void ArmInput(Vector2 inp) {
		if (info.state != HeroState.Alive) {
			return;
		}

		input.arm = inp;
	}

	public void SetOrientationFromVector2(Vector2 inp) {
		int s = MMath.SignInt(inp.x);
		if ((int)orientation != s && s != 0) {
			SetOrientation((Orientation)s);
		}
	}

	public void ButtonInput(ButtonInputEventData inp) {
		if (info.state != HeroState.Alive) {
			return;
		}

		HeroLimb limb;

		if(inp.buttonID == 0) {
			limb = this[LimbType.RArm].First();
		} else {
			limb = this[LimbType.LArm].First();
		}

		if(limb.equipment.Count > 0) {
			limb.equipment[0].Use(new EquipmentUseArgs() {
			  charge = inp.chargePercent
			});
        }
    }

	public bool AttachEquipment(Equipment equip) {
		var targetLimbs = limbs.Where(x => MUtils.EnumAnyInMask((int)equip.canAttachToLimbs, (int)x.limbType));
		
		if (targetLimbs.Count() > 0) {
			var freeLimbs = targetLimbs.Where(x => x.CanEquip(equip));

			if (freeLimbs.Count() > 0) {
				HeroLimb freeLimb = freeLimbs.First();

				equip.AttachToHero(this, freeLimb);
				freeLimb.AddEquipment(equip);

				return true;
			}
		}

		return false;
	}

	public bool FindClosestTarget() {
		ITarget enemyTarget = Game.inst.map.GetClosestTarget(this.GetPosition(), (t) => t != (ITarget)this, out TargetAimPoint aimPoint);

		if (enemyTarget != null) {
			SetTarget(enemyTarget, aimPoint);
			return true;
		}

		return false;
	}

	private void Die() {
		info.state = HeroState.Dead;
		faceRend.SetFace(HeroFace.Dead);
    }

	public Vector2 GetPosition() {
		return body.transform.position;
    }

	public override Type GetPoolObjectType() {
		return typeof(Hero);
    }

	public IEnumerable<HeroLimb> this[LimbType type] {
		get {
			return limbs.Where(x => x.limbType == type);
        }
    }

	// IDamageable
    public DamageResponse TakeDamage(DamageInfo damage) {

		switch(damage.causeType) {
			case DamageCause.PHYSICS: {
					PhysicalDamage physDmg = (PhysicalDamage)damage;

					float dmg = physDmg.GetDamage();
					info.health -= dmg;

					Debug.Log($"physDmg:{physDmg.limb == null} physDmg.limb.rend:{physDmg.limb.rend}");

					physDmg.limb.rend.TakeDamage(Mathf.Clamp01(dmg / info.maxHealth * 4));
				}break;
		}

		if (info.state != HeroState.Alive) {
			return new DamageResponse() { success = false };
		}

		HeroDamageEvent.Invoke(new HeroDamageEvent(this));

		if(info.health <= 0) {
			info.health = 0f;
			Die();
        }

		return new DamageResponse();
    }
	// IDamageable

	public void DestroyHero() {
		Destroy(gameObject);
		//Push();
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

[System.Serializable]
public class HeroInfo {
	public HeroState state;
	public HeroRole role;
	public float health;
	public float maxHealth;
	public int teamID;
}

public enum HeroState {
	None,
	Alive,
	Dead
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

public class HeroDamageEvent : MGameEvent<HeroDamageEvent> {

	public Hero hero;

	public HeroDamageEvent(Hero hero) {
		this.hero = hero;
	}
}