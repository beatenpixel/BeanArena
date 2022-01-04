using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentFactory : MonoBehaviour {

    public void Init() {

    }

    public Equipment Create(EquipmentConfig config, Vector2 position) {
        if(config.category == EquipmentCategory.Weapon) {
            WeaponConfig weaponConfig = (WeaponConfig)config;

            Weapon weapon = MPool.Get<Weapon>(weaponConfig.weaponType.ToString());
            weapon.t.position = position;

            return weapon;
        }

        return null;
    }

}

public abstract class EquipmentConfig {
    public EquipmentCategory category;

    public EquipmentConfig(EquipmentCategory cat) {
        category = cat;
    }
}

public class WeaponConfig : EquipmentConfig {
    public WeaponType weaponType;

    public WeaponConfig(WeaponType _weaponType) : base(EquipmentCategory.Weapon) {
        weaponType = _weaponType;
    }
}
