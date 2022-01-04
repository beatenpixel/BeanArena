using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PoolObject {

    public LayerMask interactMask;
    public Rigidbody2D rb;

    public float additionalGravity = 5f;
    public Vector2 refRotationDireciton;
    public TrailRenderer trailRend;

    private bool doClearTrail;

    public override void OnPop() {
        base.OnPop();

        rb.velocity = Vector2.zero;
        if (trailRend != null) {
            doClearTrail = true;
        }
    }

    public override void OnPush() {
        base.OnPush();

        if (trailRend != null) {
            doClearTrail = true;
        }
    }

    private void Update() {
        transform.rotation = Quaternion.FromToRotation(refRotationDireciton, rb.velocity);
    }

    protected virtual void LateUpdate() {
        if (doClearTrail) {
            trailRend.Clear();
            doClearTrail = false;
        }
    }

    private void FixedUpdate() {
        rb.AddForce(Vector2.up * -additionalGravity * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (MUtils.LayerInMask(interactMask, collision.gameObject.layer)) {
            Interact(collision);
        }
    }

    protected virtual void Interact(Collision2D collsion) {
        
    }

    public virtual void Shoot(Vector2 force) {
        rb.AddForce(force);
    }

    public override Type GetPoolObjectType() {
        return typeof(Projectile);
    }
}
