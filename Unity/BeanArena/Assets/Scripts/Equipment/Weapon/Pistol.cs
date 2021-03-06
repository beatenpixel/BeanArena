using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon {

    public Vector2 shootForce;
    public Transform shootPoint;

    protected override void OnUse(EquipmentUseArgs useArgs) {
        string bulletType = null;

        Bullet bullet = MPool.Get<Bullet>(bulletType);
        int teamLayer = Game.TeamIDToLayer(hero.info.teamID);
        bullet.gameObject.SetLayerRecursively(teamLayer);

        bullet.t.position = shootPoint.position;
        bullet.SetShotCharge(useArgs.charge);

        //float damage = itemData.GetStatValueWithRareness(StatType.Damage).intValue;
        float damage = hero.info.statsSummary.damage;

        bullet.SetDamage(damage);
        bullet.Shoot(shootPoint.right * Mathf.Lerp(shootForce.x, shootForce.y, useArgs.charge));

        if (hero.info.role != HeroRole.Player) {
            MSound.Play("pistol_shot", new SoundConfig().SetPitch(1,0.1f).SetVolume(0.2f,0.1f), t.position);
        } else {
            MSound.Play("pistol_shot", SoundConfig.randVolumePitch01, t.position);
        }

        if (hero.info.role == HeroRole.Player) {
            MCamera.inst.Shake(0.2f);
        }
    }

}
