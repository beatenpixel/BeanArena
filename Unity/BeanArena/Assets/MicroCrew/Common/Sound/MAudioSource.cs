using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAudioSource : PoolObject {

    [SerializeField] private AudioSource audioSource;

    private SoundConfig sett;

    public override System.Type GetPoolObjectType() {
        return typeof(MAudioSource);
    }

    public override void OnPop() {
        base.OnPop();
    }

    public override void OnPush() {
        base.OnPush();
    }

    public void SetPosition(Vector3 pos) {
        t.position = pos;
    }

    public void Mute(bool mute) {
        audioSource.mute = mute;
    }

    public MAudioSource Play(MSoundClip sound, SoundConfig _sett) {
        sett = _sett;

        float pitch;
        float volume;

        if (sett.threeDimensional) {
            audioSource.spatialBlend = 1f;
        } else {
            audioSource.spatialBlend = 0f;
        }

        audioSource.clip = sound.clip;
        audioSource.loop = sett.loop;

        if (sett.pitchRandomVariation != null) {
            Vector2 rand = sett.pitchRandomVariation ?? Vector2.zero;
            pitch = sett.pitch * sound.pitch * Random.Range(1 + rand.x, 1 + rand.y);
        } else {
            pitch = sett.pitch * sound.pitch;
        }

        if (sett.volumeRandomVariation != null) {
            Vector2 rand = sett.volumeRandomVariation ?? Vector2.zero;
            volume = MSound.inst.globalConfig.globalVolume * sett.volume * sound.volume * Random.Range(1 + rand.x, 1 + rand.y);
        } else {
            volume = MSound.inst.globalConfig.globalVolume * sett.volume * sound.volume;
        }

        switch (sound.category) {
            case SoundClipCategory.DEFAULT:
                //volume *= GameData.gameData.settings.volume;
                break;
            case SoundClipCategory.MUSIC:
                //volume *= GameData.gameData.settings.musicVolume;
                break;
        }

        audioSource.volume = volume;
        audioSource.pitch = pitch;

        if (sett.loop) {
            audioSource.Play();
        } else {
            audioSource.PlayOneShot(sound.clip);
            this.Wait(() => {
                Push();
            }, sound.clip.length / sett.pitch + 0.1f);
        }

        return this;
    }

}

public class SoundConfig {
    public float pitch = 1f;
    public Vector2? pitchRandomVariation;
    public float volume = 1f;
    public Vector2? volumeRandomVariation;

    public bool loop = false;
    public bool threeDimensional = true;

    public Transform targetT;
    public Vector3 targetTPosOffset;

    public SoundConfig SetLoop(bool _loop) {
        loop = _loop;
        return this;
    }

    public SoundConfig SetVolume(float _volume) {
        volume = _volume;
        volumeRandomVariation = null;
        return this;
    }

    public SoundConfig SetVolume(float _volume, Vector2? _volumeRandomVariation) {
        volume = _volume;
        volumeRandomVariation = _volumeRandomVariation;
        return this;
    }

    public SoundConfig SetVolume(float _volume, float _volumeRandomVariation) {
        volume = _volume;
        volumeRandomVariation = new Vector2(-_volumeRandomVariation, _volumeRandomVariation);
        return this;
    }

    public SoundConfig SetPitch(float _pitch) {
        pitch = _pitch;
        pitchRandomVariation = null;
        return this;
    }

    public SoundConfig SetPitch(float _pitch, Vector2? _pitchRandomVariation) {
        pitch = _pitch;
        pitchRandomVariation = _pitchRandomVariation;
        return this;
    }

    public SoundConfig SetPitch(float _pitch, float _pitchRandomVariation) {
        pitch = _pitch;
        pitchRandomVariation = new Vector2(-_pitchRandomVariation, _pitchRandomVariation);
        return this;
    }

    public SoundConfig SetTargetT(Transform target, Vector3 posOffset) {
        targetT = target;
        targetTPosOffset = posOffset;
        return this;
    }

    public static SoundConfig[] configs = new SoundConfig[] {
        def,
        randVolume01,
        randPitch01,
        randVolumePitch01
    };

    public static SoundConfig def = new SoundConfig();

    public static SoundConfig randVolume01 = new SoundConfig().SetVolume(1f, new Vector2(-0.1f, 0.1f));
    public static SoundConfig randPitch01 = new SoundConfig().SetPitch(1f, new Vector2(-0.1f, 0.1f));
    public static SoundConfig randVolumePitch01 = new SoundConfig().SetVolume(1f, new Vector2(-0.1f, 0.1f)).SetPitch(1f, new Vector2(-0.1f, 0.1f));

}

public enum SoundConfigType : byte {
    Default = 0,
    Volume01 = 1,
    Pitch01 = 2,
    VolumePitch01 = 3
}
