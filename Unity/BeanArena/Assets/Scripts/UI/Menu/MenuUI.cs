using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour {
    
	public void Init() {
		MGameLoop.Update.Register(InternalUpdate);
	}
	
	public void InternalUpdate() {
		
	}	
	
}
