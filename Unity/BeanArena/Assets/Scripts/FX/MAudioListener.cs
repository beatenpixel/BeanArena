using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAudioListener : MonoBehaviour {

	public AudioListener audioListener;
	public AudioLowPassFilter lowPassFilter;

	public void Init() {
		
	}

	public void EnableDeathScreenEffect(bool enable) {
		lowPassFilter.enabled = enable;
    }
	
}
