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

    protected GD_Item itemData { get; private set; }
    public SO_ItemInfo itemInfo;

    protected float nextUseTime;

    protected HeroLimb limb;
    protected HeroBase hero;

    public void SetItemData(GD_Item _itemData) {
        itemData = _itemData;
    }

    public virtual void AttachToHero(HeroBase hero, HeroLimb limb) {
        this.hero = hero;
        this.limb = limb;
    }

    public virtual void UnattachFromHero() {
        
    }

    public virtual void Use(EquipmentUseArgs useArgs) {
        if(Time.time > nextUseTime) {
            nextUseTime = Time.time + useDelay;

            OnUse(useArgs);
        }
    }

    protected virtual void OnDrawGizmosSelected() {

    }

    protected abstract void OnUse(EquipmentUseArgs useArgs);

    public override Type GetPoolObjectType() {
        return typeof(Equipment);
    }

}

public enum EquipmentAttachType {
    Transform,
    Rigidbody
}

public class EquipmentUseArgs {
    public float charge;
}