using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;

public class HelperFunctionEditorWindow : EditorWindow {

    private static HelperFunctionEditorWindow inst;

    private const string FUNC_PREFIX = "Func_";

    private Object[][] cachedObjects = new Object[20][];
    private Vector2 scrollPos;

    [MenuItem("MicroCrew/Windows/HelperFunctions")]
    public static void ShowWindow() {
        inst = (HelperFunctionEditorWindow)EditorWindow.GetWindow< HelperFunctionEditorWindow>("HelperFunctions");
    }

    private void OnEnable() {
        for (int i = 0; i < cachedObjects.Length; i++) {
            cachedObjects[i] = new Object[10];
        }
    }

    public void Func_SwapGameObjectsNames(GameObject gameObjectA, GameObject gameObjectB) {
        string name = gameObjectA.name;
        gameObjectA.name = gameObjectB.name;
        gameObjectB.name = name;
    }

    public void Func_ListAllChildren(GameObject go) {
        for (int i = 0; i < go.transform.childCount; i++) {
            Debug.Log($"Child #{i}: {go.transform.GetChild(i).gameObject.name}");
        }
    }

    public void Func_HelloWorld() {
        GameObject go = null;
        Debug.Log("Hello, world!");
    }

    public void Func_FindMissingScripts() {
        //Get the current scene and all top-level GameObjects in the scene hierarchy
        Scene currentScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = FindObjectsOfType<GameObject>(true);

        Debug.Log("Start checking " + rootObjects.Length + " gameObjects...");

        List<Object> objectsWithDeadLinks = new List<Object>();
        foreach (GameObject g in rootObjects) {
            //Get all components on the GameObject, then loop through them 
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++) {
                Component currentComponent = components[i];

                //If the component is null, that means it's a missing script!
                if (currentComponent == null) {
                    //Add the sinner to our naughty-list
                    objectsWithDeadLinks.Add(g);
                    Selection.activeGameObject = g;
                    Debug.Log(g + " has a missing script!");
                    break;
                }
            }
        }
        if (objectsWithDeadLinks.Count > 0) {
            //Set the selection in the editor
            Selection.objects = objectsWithDeadLinks.ToArray();
        } else {
            Debug.Log("No GameObjects in '" + currentScene.name + "' have missing scripts! Yay!");
        }
    }

    void OnGUI() {

        var methods = typeof(HelperFunctionEditorWindow).GetMethods();
        Color savedColor = GUI.color;
        int methodID = -1;

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (var method in methods) {
            methodID++;

            if (method.Name.StartsWith(FUNC_PREFIX)) {
                string funcName = method.Name.Substring(FUNC_PREFIX.Length);
                var parameters = method.GetParameters();

                bool wrongParams = false;

                foreach (var param in parameters) {
                    if (!param.ParameterType.IsSubclassOf(typeof(UnityEngine.Object))) {
                        wrongParams = true;
                    }
                }

                GUILayout.BeginHorizontal();

                GUILayout.Label($"{funcName}", EditorStyles.boldLabel);

                GUILayout.EndHorizontal();

                bool canExecute = true;

                if (wrongParams) {
                    canExecute = false;
                }

                Object[] targetBuffer = cachedObjects[methodID];
                for (int i = 0; i < parameters.Length; i++) {
                    if (!parameters[i].ParameterType.IsSubclassOf(typeof(UnityEngine.Object))) {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(parameters[i].Name);
                        GUI.color = Color.red;
                        GUILayout.Label($"type [{parameters[i].ParameterType}] is not supported", EditorStyles.boldLabel);
                        GUI.color = savedColor;
                        GUILayout.EndHorizontal();
                    } else {
                        targetBuffer[i] = EditorGUILayout.ObjectField(parameters[i].Name, targetBuffer[i], parameters[i].ParameterType, true);
                        if (targetBuffer[i] == null) {
                            canExecute = false;
                        }
                    }
                }

                EditorGUI.BeginDisabledGroup(!canExecute);

                if (GUILayout.Button("Run")) {
                    try {
                        object[] objs = new object[parameters.Length];
                        for (int i = 0; i < objs.Length; i++) {
                            objs[i] = targetBuffer[i];
                        }

                        method.Invoke(this, objs);
                    } catch (Exception e) {
                        Debug.LogError($"[{funcName}] " + e.Message);
                    }
                }

                EditorGUI.EndDisabledGroup();
            }
        }

        EditorGUILayout.EndScrollView();

        GUI.color = savedColor;
    }

}
