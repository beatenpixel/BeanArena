using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BeanArena/ChestInfo")]
public class SO_ChestInfo : ScriptableObject, ITypeKey<ChestType> {

    public ChestType chestType;
    public string chestName_LKey;
    public string chestDescr_LKey;
    public SO_IconContent icon;
    public GameObject prefab;

    public ChestType GetKey() {
        return chestType;
    }
}
