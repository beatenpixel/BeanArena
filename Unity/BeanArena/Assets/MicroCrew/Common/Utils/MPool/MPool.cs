using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace MicroCrew.Utils {

    [CreateAssetMenu(menuName = "MicroCrew/MPool", fileName = "MPool")]
    public class MPool : SingletonScriptableObject<MPool> {

        public List<GameObject> prefabs;
        public List<PoolObjectGroup> prefabGroups;

        [HideInInspector] public Dictionary<Type, Dictionary<string, List<IPoolObject>>> objects;
        [HideInInspector] public Dictionary<Type, Dictionary<string, IPoolObject>> objPrefabs;

        public override void Init() {
            MAssets.InitIfNeeded(null);

            objects = new Dictionary<Type, Dictionary<string, List<IPoolObject>>>();
            objPrefabs = new Dictionary<Type, Dictionary<string, IPoolObject>>();

            List<IPoolObject> poolPrefabs = new List<IPoolObject>();

            for (int i = 0; i < prefabGroups.Count; i++) {
                for (int x = 0; x < prefabGroups[i].prefabs.Count; x++) {
                    if(prefabGroups[i].prefabs[x] == null) {
                        Debug.LogError($"[MPool] Prefab in group '{prefabGroups[i].groupName}' at #{x} is null");
                        continue;
                    }

                    IPoolObject poolObject = prefabGroups[i].prefabs[x].GetComponent<IPoolObject>();

                    if(poolObject != null) {
                        poolPrefabs.Add(poolObject);
                    } else {
                        Debug.LogError($"[MPool] Prefab in group '{prefabGroups[i].groupName}' at #{x} has no IPoolObject interface");
                    }
                }
            }

            AddPrefabs(poolPrefabs);
        }

        public void AddPrefabs(List<IPoolObject> prefabsToAdd) {
            for (int i = 0; i < prefabsToAdd.Count; i++) {
                Type t = prefabsToAdd[i].GetPoolObjectType();
                string subType = prefabsToAdd[i].subType;

                if (!objPrefabs.ContainsKey(t)) {
                    objPrefabs.Add(t, new Dictionary<string, IPoolObject>());
                }

                if (!objPrefabs[t].ContainsKey(subType)) {
                    objPrefabs[t].Add(subType, prefabsToAdd[i]);
                }
            }
        }

        public IPoolObject CreateNew(IPoolObject prefab, Transform parent = null) {
            IPoolObject newInstance = MonoBehaviour.Instantiate(prefab.GetGameObject(), parent).GetComponent<IPoolObject>();
            return newInstance;
        }

        private T Internal_Get<T>(string subType, Transform parent = null) where T : IPoolObject {
            Type targetType = typeof(T);

            if (!objPrefabs.ContainsKey(targetType)) {
                targetType = targetType.BaseType;
            }

            if (!objects.ContainsKey(targetType)) {
                objects.Add(targetType, new Dictionary<string, List<IPoolObject>>());
            }

            if (!objects[targetType].ContainsKey(subType)) {
                objects[targetType].Add(subType, new List<IPoolObject>());
            }

            if (objects[targetType][subType].Count > 0) {
                T obj = (T)objects[targetType][subType][0];
                objects[targetType][subType].RemoveAt(0);
                obj.GetGameObject().transform.SetParent(parent);
                obj.OnPop();
                return obj;
            } else {
                if(!objPrefabs.ContainsKey(targetType) || !objPrefabs[targetType].ContainsKey(subType)) {
                    Debug.LogError($"[MPool] No prefab found: {targetType}:{subType}");
                }

                IPoolObject newObj = CreateNew(objPrefabs[targetType][subType], parent);
                newObj.OnCreate();
                newObj.OnPop();
                return (T)newObj;
            }
        }

        private void Internal_Push(IPoolObject obj) {
            Type objType = obj.GetPoolObjectType();

            if (!objects.ContainsKey(objType)) {
                objects.Add(objType, new Dictionary<string, List<IPoolObject>>());
            }

            if (!objects[objType].ContainsKey(obj.subType)) {
                objects[objType].Add(obj.subType, new List<IPoolObject>());
            }

            objects[objType][obj.subType].Add(obj);
            obj.OnPush();
        }

        public static T Get<T>(string subType = null, Transform parent = null) where T : IPoolObject {
            InitIfNeeded(null);

            if (subType == null) {
                subType = MPool.DEFAULT_SUB_TYPE;
            }
            return inst.Internal_Get<T>(subType, parent);
        }

        public static List<T> GetPrefabs<T>(bool collectChildTypes) where T : IPoolObject {
            Type t = typeof(T);
            List<T> result = new List<T>();

            if (inst.objPrefabs.ContainsKey(t)) {
                var prefabs = inst.objPrefabs[t];
                foreach (var item in prefabs) {
                    result.Add((T)item.Value);
                }
            }

            if (collectChildTypes) {
                foreach (var a in inst.objPrefabs) {
                    foreach (var b in a.Value) {
                        if (b.Value is T && !result.Contains((T)b.Value)) {
                            result.Add((T)b.Value);
                        }
                    }
                }
            }

            return result;
        }

        public static void AddSpawned(IPoolObject poolObj) {

        }

        public static void Push(IPoolObject obj) {
            inst.Internal_Push(obj);
        }

        public const string DEFAULT_SUB_TYPE = "default";

    }

    public interface IPoolObject {
        string subType { get; }
        Type GetPoolObjectType();
        GameObject GetGameObject();
        void OnCreate();
        void OnPop();
        void OnPush();
    }

    [System.Serializable]
    public class PoolObjectGroup {
        public string groupName;
        public List<GameObject> prefabs;
    }

}