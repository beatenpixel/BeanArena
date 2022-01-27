using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectListSpawner<T> where T : MonoBehaviour {

	public List<T> objects { get; private set; }

	private Func<int,T> spawnFunc;
	private Action<T, int, bool> updateFunc;
	private Action<T, int> disableFunc;

	public int activeObjectsCount { get; private set; }

	public ObjectListSpawner(Func<int, T> spawnFunc, Action<T,int,bool> updateFunc, Action<T, int> disableFunc) {
		objects = new List<T>();
		this.spawnFunc = spawnFunc;
		this.updateFunc = updateFunc;
		this.disableFunc = disableFunc;
	}

	public void Update(int count) {
		int n = Mathf.Max(objects.Count, count);
        for (int i = 0; i < n; i++) {
			if(i < count) {
				if(i >= objects.Count) {
					T newObj = spawnFunc(i);
					objects.Add(newObj);

					updateFunc(objects[i], i, true);
				} else {
					updateFunc(objects[i], i, false);
                }
            } else {
				disableFunc(objects[i], i);
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
