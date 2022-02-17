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

    public void EnableDeathScreenEffect(bool enable) {
        if (enable) {
            MCamera.inst.fastPostProcessing.EnableGraysacle(true, false);
            MCamera.inst.audioListener.EnableDeathScreenEffect(true);
            Time.timeScale = 0.8f;
        } else {
            MCamera.inst.fastPostProcessing.EnableGraysacle(false, true);
            MCamera.inst.audioListener.EnableDeathScreenEffect(false);
            Time.timeScale = 1f;
        }
    }

    public MParticle SpawnParticle(string name, Vector3 p, Vector3 normal, float scale = 1f) {
        MParticle particle = MPool.Get<MParticle>(name);
        particle.transform.position = p;
        particle.transform.localScale = Vector3.one * scale;
        particle.transform.up = normal;
        particle.Play();
        return particle;
    }

    public void Explosion(Vector3 p, Vector3 normal, float force = 1f) {
        MParticle explosion = MPool.Get<MParticle>("explosion");
        explosion.transform.position = p;
        explosion.transform.localScale = Vector3.one * force;
        explosion.transform.up = normal;
        explosion.Play();
        MCamera.inst.Shake(1f * force);
    }



}
