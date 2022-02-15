using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_NakedMan : HeroBase {

    public Transform signT;
    private Vector3 signStartScale;

    protected override void InitOnAwake() {
        base.InitOnAwake();
        signStartScale = signT.localScale;
    }

    public override void InitInFactory(HeroConfig config) {
        base.InitInFactory(config);
    }

    public override void InternalUpdate() {
        base.InternalUpdate();        
    }

    private void LateUpdate() {
        signT.rotation = Quaternion.Euler(0, 0, 0);
        signT.localScale = signStartScale.MulX((int)orientation);
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
        return typeof(Hero_NakedMan);
    }

}
