using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroLimb : MonoBehaviour {

	public LimbType limbType;
	public Vector2 centerOfMass;

	public Rigidbody2D rb;
	public Collider2D[] colliders;
	public SpriteRenderer baseSpriteRend;

	public LimbMotion motion;

	public bool isGrounded { get; private set; }
	public Vector2 groundNormal { get; private set; }

	private float ungroundFixedTime;

	protected virtual void Awake() {
		rb.centerOfMass = centerOfMass;
	}

	public virtual void Init() {
		
	}

	protected virtual void FixedUpdate() {
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
			rb.MoveRotation(angle);
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

public enum LimbType {
	None,
	Body,
	RArm,
	LArm
}

[System.Serializable]
public class LimbMotion {
	public float defaultRotOffset;
	public float targetRot;
	public bool isGlobal;
	public float rotSpeed;

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