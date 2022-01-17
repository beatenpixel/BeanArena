using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MicroCrew/Variables/Float")]
public class FloatVariable : ScriptableObject {
	public float value;
}

[System.Serializable]
public class FloatReference {
	public bool useConstant = true;
	public float constantValue;
	public FloatVariable variable;

	public float value {
		get {
			return useConstant ? constantValue : variable.value;
        }
		set {
			if(useConstant) {
				constantValue = value;
            } else {
				variable.value = value;
            }
        }
    }
}
