using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MParticle : PoolObject {

    protected List<ParticleSystem> paritcles = new List<ParticleSystem>();
    protected ParticleSystem mainParticle;

    protected float pushTime;

    protected override void Awake() {
        base.Awake();

        GetComponentsInChildren<ParticleSystem>(true, paritcles);

        float maxPushTime = -1000;

        foreach (var p in paritcles) {
            if (p.gameObject == gameObject) {
                mainParticle = p;
            }

            var particleMain = p.main;
            particleMain.loop = false;

            float pTime = p.main.duration / p.main.simulationSpeed;

            if (pTime > maxPushTime) {
                maxPushTime = pTime;
            }
        }

        pushTime = maxPushTime;
    }

    public void Play() {
        foreach (var p in paritcles) {
            p.time = 0;
        }

        mainParticle.Play(true);

        this.Wait(() => {
            Push();
        }, pushTime);
    }

    public override Type GetPoolObjectType() {
        return typeof(MParticle);
    }

}
