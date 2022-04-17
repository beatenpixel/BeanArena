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

    public void PopulateStatsSummaryWithAllEquipment(HeroStatsSummary summ) {
        foreach (var stat in itemInfo.stats) {
            if (summ.stats.ContainsKey(stat.statType)) {
                summ.stats[stat.statType] = summ.stats[stat.statType] + stat.GetValue(itemData.levelID, StatConfig.FromItem(itemData));
            } else {
                summ.stats.Add(stat.statType, new StatValue(stat.GetValue(itemData.levelID, StatConfig.FromItem(itemData))));
            }
        }
    }

    public void SetItemData(GD_Item _itemData) {
        itemData = _itemData;

        if (itemData.HasStat(StatType.Duration)) {
            useDelay = itemData.GetStatValueWithRareness(StatType.Duration).floatValue;
        }
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

            if(hero.hasUIPanel) {
                hero.uiPanel.panelCircleItem.RunReloadAnimation(useDelay);
            }
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