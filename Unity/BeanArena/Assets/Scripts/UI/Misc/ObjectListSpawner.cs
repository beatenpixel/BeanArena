using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectListSpawner<T> where T : MonoBehaviour {

	public List<T> objects { get; private set; }

	private Func<int,T> spawnFunc;
	private Action<T, int, bool> enableFunc;
	private Action<T, int> updateFunc;
    private Action<T, int> destroyFunc;

	public int activeObjectsCount { get; private set; }

	public ObjectListSpawner(Func<int, T> spawnFunc, Action<T,int,bool> enableFunc, Action<T, int> updateFunc, Action<T,int> destroyFunc) {
		objects = new List<T>();
		this.spawnFunc = spawnFunc;
		this.enableFunc = enableFunc;
		this.updateFunc = updateFunc;
        this.destroyFunc = destroyFunc;
    }

    public void Remove(T obj) {
        if(objects.Contains(obj)) {
            int ind = objects.IndexOf(obj);
            destroyFunc(obj, ind);
            objects.Remove(obj);
            activeObjectsCount -= 1;
        }
    }

	public void Update(int count) {
        int prevCount = activeObjectsCount;

		int n = Mathf.Max(objects.Count, count);
        for (int i = 0; i < n; i++) {
			if(i < count) {
				if(i < objects.Count) {
                    
                } else {
                    T newObj = spawnFunc(i);
                    objects.Add(newObj);
                }

                enableFunc(objects[i], i, true);
                updateFunc(objects[i], i);
            } else {
				enableFunc(objects[i], i, false);
            }
        }

		activeObjectsCount = count;
	}

	public T this[int ind] {
		get {
			return objects[ind];
        }
    }
	
}
