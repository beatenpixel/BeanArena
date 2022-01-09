using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile {

    protected override void Interact(Collision2D collision) {
        base.Interact(collision);

        HeroLimb limb = collision.gameObject.GetComponentInParent<HeroLimb>();
        if(limb != null) {
            
        }

        Hero hero = collision.gameObject.GetComponentInParent<Hero>();
        if(hero != null) {
            hero.TakeDamage(new PhysicalDamage(hero, limb));
        }

        Push();
    }

    public override Type GetPoolObjectType() {
        return typeof(Bullet);
    }

}
