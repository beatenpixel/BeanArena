using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentFactory : MonoBehaviour {

    public void Init() {

    }

    public Equipment Create(GD_Item itemData, Vector2 position) {

        if (itemData.info.category == ItemCategory.Weapon) {
            Weapon weaponPrefab = itemData.info.prefab.GetComponent<Weapon>();

            Weapon weapon = MPool.Get<Weapon>(weaponPrefab.subType);
            weapon.SetItemData(itemData);
            weapon.t.position = position;

            return weapon;
        } else if (itemData.info.category == ItemCategory.Head) {
            Gadget gadgetPrefab = itemData.info.prefab.GetComponent<Gadget>();

            Gadget gadget = MPool.Get<Gadget>(gadgetPrefab.subType);
            gadget.SetItemData(itemData);
            gadget.t.position = position;

            return gadget;
        }

        return null;
    }

}
