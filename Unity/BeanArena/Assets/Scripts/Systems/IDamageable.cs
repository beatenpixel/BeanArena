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

public class PhysicalDamage : DamageInfo {
    public Collision2D collision;
    public Vector2 relVelocity;

    public PhysicalDamage() : base(DamageCause.PHYSICS) {

    }

    public override float GetDamage() {
        return relVelocity.magnitude * Mathf.Sqrt(collision.rigidbody.mass + collision.otherRigidbody.mass);
    }
}

public class DamageResponse {
    public bool success = true;
}

public enum DamageCause {
    NONE = 0,
    PHYSICS = 1 << 0
}