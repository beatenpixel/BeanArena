using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment : PoolObject {

    [SerializeField] protected Rigidbody2D rb;

    [SerializeField] protected EquipmentAttachType attachType;
    public LimbType canAttachToLimbs;
    public TransformData attachTData;
    public float useDelay = 0.5f;

    protected float nextUseTime;

    protected HeroLimb limb;
    protected Hero hero;

    public virtual void AttachToHero(Hero hero, HeroLimb limb) {
        this.hero = hero;
        this.limb = limb;
    }

    public virtual void Use() {
        if(Time.time > nextUseTime) {
            nextUseTime = Time.time + useDelay;

            OnUse();
        }
    }

    protected virtual void OnDrawGizmosSelected() {

    }

    protected abstract void OnUse();

    public override Type GetPoolObjectType() {
        return typeof(Equipment);
    }

}

public enum EquipmentAttachType {
    Transform,
    Rigidbody
}

public enum EquipmentCategory {
    None,
    Weapon,
    Helmet,
    Boots
}