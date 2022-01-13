using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroLimb : MonoBehaviour {

	public LimbType limbType;
	public Vector2 centerOfMass;

	public Rigidbody2D rb;
	public Collider2D[] colliders;

	public HeroLimbRend rend;
	public LimbMotion motion;

	public bool isGrounded { get; private set; }
	public Vector2 groundNormal { get; private set; }

	[Header("Equipment")]
	public int maxEquipmentSlots = 1;

	private float ungroundFixedTime;
	private Vector3 startLocalScale;

	public List<Equipment> equipment = new List<Equipment>();

	protected virtual void Awake() {
		startLocalScale = transform.localScale;
		rb.centerOfMass = centerOfMass;
	}

	public virtual void Init() {
		rend.Init(this);
	}

	public virtual void InternalFixedUpdate() {
		if (isGrounded && Time.fixedTime > ungroundFixedTime) {
			isGrounded = false;
			groundNormal = Vector2.up;
		}

		if(motion.rotSpeed > 0.01f) {
			float rot = motion.targetRot;
			if(!motion.isGlobal) {
				rot = transform.parent.rotation.eulerAngles.z + rot + motion.defaultRotOffset;
            }

			float angle = Mathf.LerpAngle(rb.rotation, rot, Time.deltaTime * motion.rotSpeed);

			if (motion.doTransformRotation) {
				rb.MoveRotation(rot);
				//transform.rotation = Quaternion.Euler(0,0,angle);
			} else {
				rb.MoveRotation(angle);
			}
        }
    }

	public bool CanEquip(Equipment equip) {
		Debug.Log(equip.gameObject.name + " " + gameObject.name + (equipment.Count < maxEquipmentSlots) + " " + equipment.Count);
		return equipment.Count < maxEquipmentSlots;
	}

	public bool AddEquipment(Equipment equip) {
		if (equipment.Count < maxEquipmentSlots) {
			equipment.Add(equip);
			return true;
		} else {
			return false;
        }
    }

	public void SetOrientation(Orientation orientation) {
		if (limbType == LimbType.Body) {
			rend.SetOrientation(orientation);
		} else {
			transform.localScale = startLocalScale.MulY((int)orientation);
		}
    }

	public void SetGrounded(bool isGrounded) {
		this.isGrounded = isGrounded;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        
    }

    protected virtual void OnCollisionStay2D(Collision2D collision) {
		int colCount = collision.contactCount;
		bool _isGrounded = false;

		for (int i = 0; i < colCount; i++) {
			ContactPoint2D contact = collision.GetContact(i);

			if(Vector2.Angle(Vector2.up, contact.normal) < 45) {
				_isGrounded = true;
				groundNormal = contact.normal;
				break;
            }
        }

		if(_isGrounded) {
			isGrounded = true;
			ungroundFixedTime = Time.fixedTime + Time.fixedDeltaTime * 2f;
        }
    }

    private void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.TransformPoint(centerOfMass), 0.1f);
    }

}

[Flags]
public enum LimbType {
	None = 0,
	Body = 1 << 0,
	RArm = 1 << 1,
	LArm = 1 << 2
}

[System.Serializable]
public class LimbMotion {
	public float defaultRotOffset;
	public float targetRot;
	public bool isGlobal;
	public float rotSpeed;
	public bool doTransformRotation;

	public LimbMotion SetR(float rot, bool isGlobal) {
		targetRot = rot;
		this.isGlobal = isGlobal;
		return this;
    }

	public LimbMotion SetS(float speed) {
		rotSpeed = speed;
		return this;
	}
}