using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour {

    public float normalAngle;
    public float bounceForce;
    public bool multiplyByMass;
    public LayerMask interactMask;
    public Vector2 normal => (Quaternion.Euler(0, 0, normalAngle) * transform.up).normalized;

    private void OnCollisionEnter2D(Collision2D collision) {
        ProcessCollision(collision);
        Debug.Log("coll: " + collision.gameObject.name);
    }

    private void OnCollisionStay2D(Collision2D collision) {
        ProcessCollision(collision);
    }

    private void ProcessCollision(Collision2D collision) {
        if (MUtils.LayerInMask(interactMask, collision.gameObject.layer)) {
            Rigidbody2D otherRb = collision.collider.GetComponentInParent<Rigidbody2D>();

            if (otherRb != null) {
                float force = bounceForce;
                if (multiplyByMass) {
                    if (otherRb.mass > 1f) {
                        force *= Mathf.Sqrt(otherRb.mass);
                    } else {
                        force *= otherRb.mass;
                    }
                }

                otherRb.AddForce(normal * force, ForceMode2D.Force);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.TransformPoint(Vector3.zero), transform.TransformPoint(Vector3.zero) + (Vector3)normal);
    }

}
