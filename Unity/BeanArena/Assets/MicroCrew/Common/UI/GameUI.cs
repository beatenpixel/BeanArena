using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {

    public static GameUI inst;
    public static UICanvas canvas => inst.m_Canvas;

    [SerializeField] private UICanvas m_Canvas;

	public void Init() {
        inst = this;

        MGameLoop.Update.Register(InternalUpdate);
    }

    public void InternalUpdate() {

    }

}
