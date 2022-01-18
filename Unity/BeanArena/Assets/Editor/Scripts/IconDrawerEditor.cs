using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IconDrawer))]
public class IconDrawerEditor : MCustomEditor<IconDrawer> {

    protected override void DrawUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Update")) {
            inst.DrawIcon();
        }
    }

    protected override void OnGUIChanged() {
        Debug.Log("OnGUIChanged");
    }
}

public abstract class MCustomEditor<T> : Editor where T : UnityEngine.Object {

    protected T inst;

    public override void OnInspectorGUI() {     
        inst = (T)target;    
        
        if(GUI.changed) {
            OnGUIChanged();
        }

        DrawUI();
    }

    protected abstract void DrawUI();

    protected virtual void OnGUIChanged() {

    }

}
