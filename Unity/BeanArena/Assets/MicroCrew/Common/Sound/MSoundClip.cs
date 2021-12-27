using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundClip", menuName = "MicroCrew/Sound/SoundClip")]
public class MSoundClip : ScriptableObject {
    public SoundClipCategory category = SoundClipCategory.DEFAULT;
    [HideInInspector] public string soundName;
    public AudioClip clip;
    public float volume = 1f;
    public float pitch = 1f;
    public float playTimeThreshold = 0.1f;
    [HideInInspector] public float lastPlayTime;
}

public enum SoundClipCategory {
    NONE,
    DEFAULT,
    MUSIC
}