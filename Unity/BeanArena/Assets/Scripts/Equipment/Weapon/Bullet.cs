using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile {

    protected override void Interact(Collision2D collsion) {
        base.Interact(collsion);

        Push();
    }

    public override Type GetPoolObjectType() {
        return typeof(Bullet);
    }

}
