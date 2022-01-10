using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon {

    public Vector2 shootForce;
    public Transform shootPoint;

    protected override void OnUse(EquipmentUseArgs useArgs) {
        Debug.Log("CHARGE: " + useArgs.charge);

        string bulletType = null;
        if(weaponSO.weaponType == WeaponType.WaterPistol) {
            bulletType = "WaterBullet";
        }

        Bullet bullet = MPool.Get<Bullet>(bulletType);
        int teamLayer = Game.TeamIDToLayer(hero.info.teamID);
        bullet.gameObject.SetLayerRecursively(teamLayer);

        bullet.t.position = shootPoint.position;
        bullet.SetShotCharge(useArgs.charge);
        bullet.Shoot(shootPoint.right * Mathf.Lerp(shootForce.x, shootForce.y, useArgs.charge));
    }

}
