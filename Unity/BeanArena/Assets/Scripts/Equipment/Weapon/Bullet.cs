using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile {

    protected override void Interact(Collision2D collision) {
        base.Interact(collision);

        Debug.Log(collision.collider.gameObject.name);

        Rigidbody2D otherRb = collision.collider.GetComponentInParent<Rigidbody2D>();
        Debug.Log("rb is null: " + (otherRb == null));
        if(otherRb != null) {
            otherRb.AddForce(transform.right * 500);
        }

        HeroLimb limb = collision.collider.gameObject.GetComponentInParent<HeroLimb>();
        Hero hero = collision.collider.gameObject.GetComponentInParent<Hero>();

        if (hero != null && limb != null) {
            hero.TakeDamage(new PhysicalDamage(10, hero, limb));
        }

        Push();
    }

    public override Type GetPoolObjectType() {
        return typeof(Bullet);
    }

}
