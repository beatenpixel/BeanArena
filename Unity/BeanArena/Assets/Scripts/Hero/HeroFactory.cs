using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroFactory : MonoBehaviour { 

    public void Init() {

    }

    public HeroBase Create(HeroConfig config, Vector2 position) {
        //Hero_DefaultBean hero = MPool.Get<Hero_DefaultBean>();
        HeroBase hero = MPool.Get<HeroBase>(config.heroType.ToString());

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
    public SO_HeroInfo info;
}
