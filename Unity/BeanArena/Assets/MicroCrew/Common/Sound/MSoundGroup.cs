using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundGroup", menuName = "MicroCrew/Sound/SoundGroup")]
public class MSoundGroup : ScriptableObject {
    [HideInInspector] public string groupName;
    public MSoundClip[] sounds;
}
