using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroFactory : MonoBehaviour { 

    public void Init() {

    }

    public HeroBase Create(HeroConfig config, Vector2 position) {
        //Hero_DefaultBean hero = MPool.Get<Hero_DefaultBean>();
        HeroBase hero = null;

        switch(config.heroType) {
            case HeroType.Shark: hero = MPool.Get<Hero_Shbark>(); break;
            default: hero = MPool.Get<Hero_DefaultBean>(); break;
        } 

        hero.InitInFactory(config);
        hero.SetSpawnPosition(position);

        return hero;
    }

}

public class HeroConfig {
    public string nickname;
    public int teamID;
    public Orientation orientation;
    public HeroRole role;
    public HeroType heroType;
}
