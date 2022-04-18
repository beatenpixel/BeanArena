using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MServices", menuName = "MicroCrew/Services/MServices")]
public class MServices : SingletonScriptableObject<MServices> {

    public override void Init() {
        AdManager.InitIfNeeded(null);
    }

}
