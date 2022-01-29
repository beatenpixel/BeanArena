using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentFactory : MonoBehaviour {

    public void Init() {

    }

    public Equipment Create(SO_ItemInfo itemInfo, Vector2 position) {

        if (itemInfo.category == ItemCategory.Weapon) {
            Weapon weaponPrefab = itemInfo.prefab.GetComponent<Weapon>();

            Weapon weapon = MPool.Get<Weapon>(weaponPrefab.subType);
            weapon.itemInfo = itemInfo;
            weapon.t.position = position;

            return weapon;
        }

        return null;
    }

}
