using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon {

    public Vector2 forcePoint;

    public float force = 300;
    public float massOnUse = 1f;
    public float massOnIdle = 0.1f;

    protected override void Awake() {
        base.Awake();
        rb.mass = massOnIdle;
    }

    protected override void OnUse() {
        float torque = force * -(int)hero.orientation;

        //rb.mass = massOnUse;
        //limb.rb.AddTorque(force, ForceMode2D.Impulse);
        rb.AddForceAtPosition(transform.up * torque, t.TransformPoint(forcePoint), ForceMode2D.Impulse);
        this.Wait(() => {
            //rb.mass = massOnIdle;
        }, 0.5f);
    }

    protected override void OnDrawGizmosSelected() {
        base.OnDrawGizmosSelected();

        Gizmos.DrawLine(transform.TransformPoint(forcePoint), transform.TransformPoint(forcePoint) + transform.up * 1f);
    }

}
