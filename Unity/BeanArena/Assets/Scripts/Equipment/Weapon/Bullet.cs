using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile {

    private float shotCharge;

    protected override void Interact(Collision2D collision) {
        base.Interact(collision);

        Debug.Log(collision.collider.gameObject.name);

        Rigidbody2D otherRb = collision.collider.GetComponentInParent<Rigidbody2D>();
        Debug.Log("rb is null: " + (otherRb == null));
        if(otherRb != null) {
            otherRb.AddForceAtPosition(transform.right * (300 + shotCharge * 400), transform.position);
        }

        HeroLimb limb = collision.collider.gameObject.GetComponentInParent<HeroLimb>();
        HeroBase hero = collision.collider.gameObject.GetComponentInParent<HeroBase>();

        if (hero != null && limb != null) {
            MSound.Play("bullet_bodyhit", SoundConfig.randVolumePitch01, t.position);
            hero.TakeDamage(new PhysicalDamage(10 * (1 + shotCharge), hero, limb));
        }

        Projectile projectile = collision.collider.GetComponentInParent<Projectile>();

        if(projectile != null) {
            MSound.Play("ricochet", SoundConfig.randVolumePitch01, t.position);
        }

        Push();
    }

    public void SetShotCharge(float charge) {
        shotCharge = charge;
    }

    public override Type GetPoolObjectType() {
        return typeof(Bullet);
    }

}
