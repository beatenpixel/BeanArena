using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBullet : Projectile {

    private float shotCharge;
    private float damage;

    protected override void Interact(Collision2D collision) {
        base.Interact(collision);

        Rigidbody2D otherRb = collision.collider.GetComponentInParent<Rigidbody2D>();
        
        if (otherRb != null) {
            otherRb.AddForceAtPosition(transform.right * (300 + shotCharge * 400), transform.position);
        }

        HeroLimb limb = collision.collider.gameObject.GetComponentInParent<HeroLimb>();
        HeroBase hero = collision.collider.gameObject.GetComponentInParent<HeroBase>();

        if (hero != null && limb != null) {
            MSound.Play("bullet_bodyhit", SoundConfig.randVolumePitch01, t.position);
            hero.TakeDamage(new PhysicalDamage(damage, hero, limb));
        }

        Projectile projectile = collision.collider.GetComponentInParent<Projectile>();

        if (projectile != null) {
            MSound.Play("ricochet", SoundConfig.randVolumePitch01, t.position);
        }

        FX.inst.Explosion(collision.GetContact(0).point, Vector3.up);

        Push();
    }

    public void SetDamage(float damage) {
        this.damage = damage;
    }

    public void SetShotCharge(float charge) {
        shotCharge = charge;
    }

    public override Type GetPoolObjectType() {
        return typeof(RocketBullet);
    }

}
