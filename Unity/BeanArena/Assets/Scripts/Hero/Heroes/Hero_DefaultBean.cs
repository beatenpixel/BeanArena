using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_DefaultBean : HeroBase {

    private HeroFaceRend faceRend;

    protected override void InitOnAwake() {
        base.InitOnAwake();

        faceRend = GetComponentInChildren<HeroFaceRend>();
        faceRend.Init();
    }

    public override void InitInFactory(HeroConfig config) {
        base.InitInFactory(config);

        Color beanColor = MAssets.colors["bean_team_" + config.teamID];

        for (int i = 0; i < limbs.Count; i++) {
            limbs[i].rend.SetBaseColor(beanColor);
        }
    }

    public override void InternalUpdate() {
        base.InternalUpdate();
    }

    public override void Revive() {
        base.Revive();

        faceRend.SetFace(HeroFace.Normal);
    }

    protected override void Die() {
        base.Die();

        faceRend.SetFace(HeroFace.Dead);
    }

    public override void DestroyHero() {
        base.DestroyHero();
    }

}
