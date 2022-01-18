using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgressionBase<T> {

	public ProgressionBehaviour behaviour;
	
}

public enum ProgressionBehaviour {
	Manual,
	Interpolate,
	Extrapolate
}