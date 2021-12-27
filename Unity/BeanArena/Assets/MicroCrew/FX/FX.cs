using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX : Singleton<FX> {

    public override void Init() {

    }

    protected override void Shutdown() {

    }

    public void Explosion(Vector3 p, Vector3 normal, float scale = 1f) {
        MParticle explosion = MPool.Get<MParticle>("explosion");
        explosion.transform.position = p;
        explosion.transform.localScale = Vector3.one * scale;
        explosion.transform.up = normal;
        explosion.Play();
    }

}
