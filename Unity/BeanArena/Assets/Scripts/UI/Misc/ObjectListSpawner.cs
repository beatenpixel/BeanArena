using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectListSpawner<T> where T : MonoBehaviour {

	private List<T> objs = new List<T>();

	private Func<int,T> spawnFunc;
	private Action<T, int, bool> updateFunc;
	private Action<T, int> disableFunc;

	public int activeObjectsCount { get; private set; }

	public ObjectListSpawner(Func<int, T> spawnFunc, Action<T,int,bool> updateFunc, Action<T, int> disableFunc) {
		this.spawnFunc = spawnFunc;
		this.updateFunc = updateFunc;
		this.disableFunc = disableFunc;
	}

	public void Spawn(int count) {
		int n = Mathf.Max(objs.Count, count);
        for (int i = 0; i < n; i++) {
			if(i < count) {
				if(i >= objs.Count) {
					T newObj = spawnFunc(i);
					objs.Add(newObj);

					updateFunc(objs[i], i, true);
				} else {
					updateFunc(objs[i], i, false);
                }
            } else {
				disableFunc(objs[i], i);
            }
        }

		activeObjectsCount = count;
	}

	public T this[int ind] {
		get {
			return objs[ind];
        }
    }
	
}
