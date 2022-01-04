using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon {

    public Vector2 shootForce;

    public Transform shootPoint;

    protected override void OnUse() {
        Bullet bullet = MPool.Get<Bullet>();
        int teamLayer = Game.TeamIDToLayer(hero.info.teamID);
        bullet.gameObject.SetLayerRecursively(teamLayer);

        bullet.t.position = shootPoint.position;
        bullet.Shoot(shootPoint.right * MRandom.Range(shootForce.x, shootForce.y));
    }

}
