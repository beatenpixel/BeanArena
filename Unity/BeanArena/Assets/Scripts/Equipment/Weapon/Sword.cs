using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon {

    public Vector2 forcePoint;

    public float force = 300;
    public float pushForce = 50f;
    public float massOnUse = 1f;
    public float massOnIdle = 0.1f;

    protected override void Awake() {
        base.Awake();
        rb.mass = massOnIdle;
    }

    protected override void OnUse(EquipmentUseArgs useArgs) {
        float torque = force * -(int)hero.orientation;

        //rb.mass = massOnUse;
        limb.rb.AddTorque(torque, ForceMode2D.Impulse);
        //rb.AddForceAtPosition(transform.up * torque, t.TransformPoint(forcePoint), ForceMode2D.Impulse);
        this.Wait(() => {
            //rb.mass = massOnIdle;
        }, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        Hero hero = collider.GetComponentInParent<Hero>();

        if (hero != null) {

        }

        Rigidbody2D otherRb = collider.GetComponentInParent<Rigidbody2D>();

        if(otherRb != null) {
            Vector2 pointVel = rb.GetRelativePointVelocity(forcePoint);            
            pointVel = Vector2.ClampMagnitude(pointVel, 5);
            pointVel = pointVel.normalized * Mathf.Clamp(Mathf.Abs(rb.angularVelocity) * 0.01f, 0, 3);
            limb.rb.angularVelocity = limb.rb.angularVelocity * 0.3f;
            rb.angularVelocity = rb.angularVelocity * 0.3f;

            otherRb.AddForce(pointVel * pushForce, ForceMode2D.Impulse);
            IMDraw.Line3D(t.TransformPoint(forcePoint), t.TransformPoint(forcePoint) + (Vector3)pointVel, Color.red, 5f);
        }
    }

    protected override void OnDrawGizmosSelected() {
        base.OnDrawGizmosSelected();

        Gizmos.DrawLine(transform.TransformPoint(forcePoint), transform.TransformPoint(forcePoint) + transform.up * 1f);
    }

}
