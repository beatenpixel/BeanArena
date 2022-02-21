using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Skeleton : HeroBase {

    protected override void InitOnAwake() {
        base.InitOnAwake();
    }

    public override void InitInFactory(HeroConfig config) {
        base.InitInFactory(config);
    }

    public override void InternalUpdate() {
        base.InternalUpdate();
    }

    public override void Revive() {
        base.Revive();
    }

    protected override void Die() {
        base.Die();
    }

    public override void DestroyHero() {
        base.DestroyHero();
    }

    public override Type GetPoolObjectType() {
        return typeof(Hero_Skeleton);
    }

}
