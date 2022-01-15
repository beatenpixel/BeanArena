using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour {

	public static MenuUI inst;

	public GameObject rootGO;

	public void Init() {
		inst = this;

		MGameLoop.Update.Register(InternalUpdate);
	}
	
	public void InternalUpdate() {
		
	}

	public void Show(bool show) {
		rootGO.SetActive(show);

		if (show) {

		} else {

		}
	}


}
