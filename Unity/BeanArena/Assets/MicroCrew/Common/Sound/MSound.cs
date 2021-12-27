using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSound : SingletonFromResources<MSound> {

    public MSoundGlobalConfig globalConfig;

    private List<MSoundGroup> soundGroups;
    private List<MSoundClip> sounds;

    private Dictionary<string, MSoundClip> soundsDict;
    private Dictionary<string, MSoundGroup> soundGroupsDict;

    public List<MAudioSource> spawnedAudioSources = new List<MAudioSource>();

    public override void Init() {
        Debug.Log("Init MSound");
        LoadSoundsFromResources();

        soundGroupsDict = new Dictionary<string, MSoundGroup>();
        for (int i = 0; i < soundGroups.Count; i++) {
            soundGroups[i].groupName = soundGroups[i].name;
            soundGroupsDict[soundGroups[i].groupName] = soundGroups[i];
        }

        soundsDict = new Dictionary<string, MSoundClip>();
        for (int i = 0; i < sounds.Count; i++) {
            sounds[i].soundName = sounds[i].name;

            soundsDict[sounds[i].soundName] = sounds[i];
            sounds[i].lastPlayTime = 0f;

            Debug.Log("add sound " + sounds[i].name);
        }
    }

    protected override void Shutdown() {

    }

    private void LoadSoundsFromResources() {
        soundGroups = new List<MSoundGroup>(Resources.LoadAll<MSoundGroup>("MSound/Groups"));
        sounds = new List<MSoundClip>(Resources.LoadAll<MSoundClip>("MSound/Clips"));
    }

    private MAudioSource Internal_PlaySound(string sound, SoundConfig config, Vector3 pos, bool isSoundGroup, int inGroupSoundID = -1) {
        MSoundClip soundToPlay;

        if (isSoundGroup) {
            MSoundGroup group = soundGroupsDict[sound];

            if (inGroupSoundID == -1) {
                inGroupSoundID = MRandom.Range(0, group.sounds.Length);
            }

            soundToPlay = group.sounds[inGroupSoundID];
        } else {
            if (!soundsDict.ContainsKey(sound)) {
                Debug.LogError("Sound not found: " + sound);
            }

            soundToPlay = soundsDict[sound];
        }

        if (Time.time <= soundToPlay.lastPlayTime + soundToPlay.playTimeThreshold) {
            return null;
        }

        soundToPlay.lastPlayTime = Time.time;

        MAudioSource audioSource = MPool.Get<MAudioSource>();
        audioSource.SetPosition(pos);

        audioSource.Play(soundToPlay, config);

        audioSource.Mute(!Game.data.soundOn);

        if (!spawnedAudioSources.Contains(audioSource)) {
            spawnedAudioSources.Add(audioSource);
        }

        return audioSource;
    }

    public bool CheckPlaySound(string sound, bool isSoundGroup, out int groupRandomSoundID) {
        MSoundClip soundToPlay;

        if (isSoundGroup) {
            MSoundGroup group = soundGroupsDict[sound];
            groupRandomSoundID = MRandom.Range(0, group.sounds.Length);

            soundToPlay = group.sounds[groupRandomSoundID];
        } else {
            if (!soundsDict.ContainsKey(sound)) {
                Debug.LogError("Sound not found: " + sound);
            }

            soundToPlay = soundsDict[sound];
            groupRandomSoundID = 0;
        }

        if (Time.time <= soundToPlay.lastPlayTime + soundToPlay.playTimeThreshold) {
            return false;
        }

        return true;
    }

    public static void Mute(bool mute) {
        for (int i = 0; i < inst.spawnedAudioSources.Count; i++) {
            if (inst.spawnedAudioSources[i].go.activeInHierarchy) {
                inst.spawnedAudioSources[i].Mute(mute);
            }
        }
    }

    public static MAudioSource Play(string sound, SoundConfig config = null, Vector3? pos = null) {
        InitIfNeeded(null);

        if (config == null) {
            config = SoundConfig.def;
        }

        return inst.Internal_PlaySound(sound, config, pos ?? Vector3.zero, false);
    }

    public static MAudioSource PlayFromGroup(string soundGroup, int soundID = -1, SoundConfig config = null, Vector3? pos = null) {
        InitIfNeeded(null);

        if (config == null) {
            config = SoundConfig.def;
        }

        return inst.Internal_PlaySound(soundGroup, config, pos ?? Vector3.zero, true, soundID);
    }

}

[System.Serializable]
public class MSoundGlobalConfig {
    public float globalVolume = 1f;
}
