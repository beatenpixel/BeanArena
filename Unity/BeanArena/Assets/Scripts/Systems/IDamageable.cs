using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    DamageResponse TakeDamage(DamageInfo damage);
}

public abstract class DamageInfo {
    public DamageCause causeType;

    public abstract float GetDamage();

    public DamageInfo(DamageCause _causeType) {
        causeType = _causeType;
    }
}

public abstract class HeroDamage : DamageInfo {

    public HeroBase hero;
    public HeroLimb limb;

    protected HeroDamage(HeroBase _hero, HeroLimb _limb, DamageCause _causeType) : base(_causeType) {
        hero = _hero;
        limb = _limb;
    }

}

public class DevDamage : DamageInfo {

    public DevDamage() : base(DamageCause.DEV) {
        
    }

    public override float GetDamage() {
        return 1000000;
    }
}

public class PhysicalDamage : HeroDamage {
    public float damage;

    public PhysicalDamage(float damage, HeroBase _hero, HeroLimb _limb) : base(_hero, _limb, DamageCause.PHYSICS) {
        this.damage = damage;
    }

    public override float GetDamage() {
        return damage;
        //return relVelocity.magnitude * Mathf.Sqrt(collision.rigidbody.mass + collision.otherRigidbody.mass);
    }
}

public class DamageResponse {
    public bool success = true;
}

public enum DamageCause {
    NONE = 0,
    PHYSICS = 1 << 0,
    DEV = 1 << 1
}