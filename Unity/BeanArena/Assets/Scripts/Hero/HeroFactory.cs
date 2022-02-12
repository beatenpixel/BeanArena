using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroFactory : MonoBehaviour { 

    public void Init() {

    }

    public Hero Create(HeroConfig config, Vector2 position) {
        Hero hero = MPool.Get<Hero>();

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
