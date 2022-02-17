using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainFX : MonoBehaviour {

    private MAudioSource rainSound;
    public ParticleSystem rainParticle;

    private void Start() {
        rainSound = MSound.Play("rain_loop", new SoundConfig() {
            loop = true,
            threeDimensional = false
        });
    }

}
