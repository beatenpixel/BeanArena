using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class HeroBase : PoolObject, IDamageable, ITarget {

	public HeroInfo info { get; private set; }

	[Header("Config")]
	public MoveConfig moveConfig;
	[Header("Links")]
	public HeroEquipment heroEquipment;

	[HideInInspector] public HeroBody body;
	private List<HeroArm> arms;
	public List<HeroLimb> limbs { get; private set; }

	private HeroInput input = new HeroInput();
    public HeroInput GetHeroInput() => input;

	private float jumpTimer;

	private WUI_Text nicknameText;

	public Orientation orientation { get; private set; }
	private HeroControllType controllType = HeroControllType.OneJoystick;

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
    private HeroConfig initConfig;
    private Vector2 spawnPos;

    [HideInInspector] public SO_HeroInfo heroInfo;
    [HideInInspector] public GD_HeroItem heroData;

    protected override void Awake() {
        base.Awake();

        InitOnAwake();
	}

    protected virtual void InitOnAwake() {
        body = GetComponentInChildren<HeroBody>();
        arms = new List<HeroArm>(GetComponentsInChildren<HeroArm>());

        limbs = new List<HeroLimb>();
        limbs.Add(body);
        limbs.AddRange(arms);

        for (int i = 0; i < limbs.Count; i++) {
            limbs[i].Init();
        }

        heroEquipment.InitComponent(this);
        heroEquipment.Init();

        m_TargetAimPoints.Add(new TargetAimPoint() {
            worldPos = body.transform.position
        });
    }

	public virtual void InitInFactory(HeroConfig config) {
        initConfig = config;

        heroInfo = MAssets.heroesInfo.GetAsset(config.heroType);
        heroData = Game.data.inventory.heroes.Find(x => x.heroType == heroInfo.heroType);

        info = new HeroInfo();
        info.maxHealth = heroData.GetStatValue(StatType.Health).intValue;
		info.health = info.maxHealth;

		info.teamID = config.teamID;
		info.state = HeroState.Alive;
		info.role = config.role;
        info.nickname = config.nickname;

        if (config.role == HeroRole.Enemy) {
			WUI_TextStyle style = WUI_TextStyle.beanNickname;
			style.textColor = Color.white.SetA(0.3f);
			//nicknameText = WorldUI.inst.AddText(config.nickname, body.transform, Vector2.up * 1.5f, style);

			t.localScale = Vector3.one * 1.1f;
		}

        for (int i = 0; i < limbs.Count; i++) {
            limbs[i].rend.SetBaseColor(Color.white);
            limbs[i].gameObject.layer = Game.TeamIDToBeanLayer(config.teamID);
		}

		gameObject.layer = Game.TeamIDToBeanLayer(config.teamID);

		SetOrientation(config.orientation);
	}


    public void Init() {
		
	}

    public void SetSpawnPosition(Vector2 pos) {
        spawnPos = pos;
        t.position = spawnPos;
    }

    public void ReturnToSpawnPosition() {        
        body.transform.position = spawnPos;

        foreach (var limb in limbs) {
            if (limb.rb != null) {
                limb.rb.velocity = Vector2.zero;
                limb.rb.angularVelocity = 0f;
            }
        }
    }

	public virtual void InternalUpdate() {
		targetAimPoints[0].worldPos = body.transform.position;
	}

	private Vector2 GetMoveInputByAim(Vector2 aimInput) {
		float angle = Vector2.SignedAngle(Vector2.up, aimInput);
		return (Quaternion.Euler(0, 0, angle * 0.4f) * Vector3.up).normalized;
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
				//ArmInput(body.transform.right * MMath.CeilAwayFrom0Int(enemyDD.x));
				Vector2 armInput = enemyDD.normalized;
				ArmInput(armInput);

				Vector2 moveInput = GetMoveInputByAim(armInput);

				if (enemyDD.magnitude > 15) {					
					MoveInput(moveInput);
				} else {
					MoveInput(moveInput.MulX(0.1f));
				}
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
					MSound.Play("swoosh", new SoundConfig().SetPitch(1f, 0.3f).SetVolume(0.4f, 0.2f), t.position);
				}
			}

			if (!hadMoveInputLastFrame) {
				SetGravityLevelForBodyParts(1f);
			}

			hadMoveInputLastFrame = true;
		} else {
			body.motion.SetR(0f, true);

			if (hadMoveInputLastFrame) {
				SetGravityLevelForBodyParts(1f);
			}

			hadMoveInputLastFrame = false;
		}
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

		heroEquipment.OnButtonInput(inp);
    }

	public bool FindClosestTarget() {
		ITarget enemyTarget = Game.inst.map.GetClosestTarget(this.GetPosition(), (t) => t != (ITarget)this, out TargetAimPoint aimPoint);

		if (enemyTarget != null) {
			SetTarget(enemyTarget, aimPoint);
			return true;
		}

		return false;
	}

	protected virtual void Die() {
		info.state = HeroState.Dead;
		HeroDieEvent.Invoke(new HeroDieEvent(this));
    }

    public virtual void Revive() {
        info.health = info.maxHealth;
        info.state = HeroState.Alive;

        input.move = Vector2.zero;
        input.arm = Vector2.zero;

        ReturnToSpawnPosition();
        SetOrientation(initConfig.orientation);        

        HeroDamageEvent.Invoke(new HeroDamageEvent(this));
    }

	public Vector2 GetPosition() {
		return body.transform.position;
    }

	public IEnumerable<HeroLimb> this[LimbType type] {
		get {
			return limbs.Where(x => x.limbType == type);
        }
    }

	// IDamageable
    public virtual DamageResponse TakeDamage(DamageInfo damage) {

		switch(damage.causeType) {
			case DamageCause.PHYSICS: {
					PhysicalDamage physDmg = (PhysicalDamage)damage;

					float dmg = physDmg.GetDamage();
					info.health -= dmg;

					physDmg.limb.rend.TakeDamage(Mathf.Clamp01(dmg / info.maxHealth * 4));					
				}break;
            case DamageCause.DEV:
                DevDamage devDmg = (DevDamage)damage;

                info.health -= devDmg.GetDamage();
                break;
		}

        if(info.role == HeroRole.Player) {
            MCamera.inst.VignetteColor_OnHit();
            MCamera.inst.Shake(0.4f);
        }

		if (info.state != HeroState.Alive) {
			return new DamageResponse() { success = false };
		}

		HeroDamageEvent.Invoke(new HeroDamageEvent(this));

		if(info.health <= 0) {
			info.health = 0f;
			MSound.Play("death", null, t.position);
			Die();			
        } else {
			MSound.PlayFromGroup("hero_ouch", -1, new SoundConfig().SetPitch(1.6f, 0.2f), t.position);
		}		

		return new DamageResponse();
    }

	// IDamageable

	public virtual void DestroyHero() {
        heroEquipment.DestroyEquipment();
        //WorldUI.inst.RemoveText(nicknameText);

        Destroy(gameObject);
        //Push();
    }

}

public class HeroComponent : MonoBehaviour {

	protected HeroBase hero;

	public void InitComponent(HeroBase _hero) {
		hero = _hero;
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
	public float moveForce = 400;
	public float jumpForce = 1200f;
}

[System.Serializable]
public class HeroInfo {
	public HeroState state;
	public HeroRole role;
	public float health;
	public float maxHealth;
	public int teamID;

    public string nickname;
    public int mmr;
}

public enum HeroControllType {
	OneJoystick,
	TwoJoysticks
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

	public HeroBase hero;

	public HeroDamageEvent(HeroBase hero) {
		this.hero = hero;
	}
}

public class HeroDieEvent : MGameEvent<HeroDieEvent> {

	public HeroBase hero;

	public HeroDieEvent(HeroBase hero) {
		this.hero = hero;
	}
}