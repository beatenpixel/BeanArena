using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MUtils : Singleton<MUtils> {

    public override void Init() {
        
    }

    protected override void Shutdown() {
        
    }


	/*
	1  2
	0  3
	*/
	public static bool PointIsInsideCorners(Vector2 point, Vector3[] corners) {
		return point.x >= corners[0].x && point.x <= corners[2].x && point.y >= corners[0].y && point.y <= corners[2].y;
	}

	public static bool PointIsInsideCorners(Vector2 point, Vector2[] corners) {
		return point.x >= corners[0].x && point.x <= corners[2].x && point.y >= corners[0].y && point.y <= corners[2].y;
    }

	public static bool LayerInMask(int mask, int layer) {
		return mask == (mask | (1 << layer));
	}

	public static bool EnumAnyInMask(int mask, int enumValues) {
		return (mask & enumValues) > 0;
	}

	public static bool EnumAllInMask(int mask, int enumValues) {
		return mask == (mask | enumValues);
	}

	public static int Ind(int x, int y, int w) {
		return y * w + x;
	}

	public static void IndTo2D(int ind, int w, out int x, out int y) {
		y = ind / w;
		x = ind % w;
	}

}

public interface IIndexed2D {
	int Ind(int x, int y);
	bool IndTo2D(int ind, out int x, out int y);
	bool IsValid(int x, int y);
}

public class BinaryStateSwitcher {

	private bool state;
	private bool tempState;
	private bool isChangingState;

	public BinaryStateSwitcher(bool defaultState) {
		state = defaultState;
		tempState = defaultState;
	}

	public bool Switch(bool _state) {
		if (isChangingState)
			return false;

		if (state) {
			if (_state)
				return false;

			isChangingState = true;
			tempState = _state;

			return true;
		} else {
			if (!_state)
				return false;

			isChangingState = true;
			tempState = _state;

			return true;
		}
	}

	public void FinishSwitch() {
		state = tempState;
		isChangingState = false;
	}

	public void SetStateForced(bool _state) {
		state = _state;
		tempState = _state;
		isChangingState = false;
    }

}

public class RefreshableObjectsList<T> where T : MonoBehaviour {

    private Func<int, T> spawnNew;
    private Action<int, T> refresh;
    private Action<int, T, bool> enable;

    private List<T> objects;

    public RefreshableObjectsList(Func<int, T> _spawnNew, Action<int, T, bool> _enable, Action<int, T> _refresh) {
        spawnNew = _spawnNew;
        refresh = _refresh;
        enable = _enable;

        objects = new List<T>();
    }

    public void Refresh(int targetObjectsCount) {
        for (int i = 0; i < Mathf.Max(targetObjectsCount, objects.Count); i++) {
            if (i < targetObjectsCount) {
                T targetObject;

                if (i >= objects.Count) {
                    targetObject = spawnNew(i);
                    objects.Add(targetObject);
                } else {
                    targetObject = objects[i];
                }

                enable(i, targetObject, true);
                refresh(i, targetObject);
            } else {
                enable(i, objects[i], false);
            }
        }
    }

    public T GetObject(int ind) {
        return objects[ind];
    }

}

public class ChangeCheck<T> where T : IEquatable<T> {

	public T value { get; private set; }
	private bool hasValue;
	private bool m_IsDirty;

	public ChangeCheck() {
		hasValue = false;
    }

	public ChangeCheck(T initialValue) {
		value = initialValue;
		hasValue = true;
    }

	public bool CheckIsDirtyAndClear() {
		if (m_IsDirty) {
			m_IsDirty = false;
			return true;
		} else {
			return false;
		}
	}

	public void Set(T newValue) {
		if (!hasValue) {
			m_IsDirty = true;
			hasValue = true;
			value = newValue;
			return;
		} else {
			bool equals = newValue.Equals(value);
			if (!equals) {
				m_IsDirty = true;
			}

			value = newValue;
		}
    }

}

/*

[MCSceneManager]

[MCFunc]
FuncLoop
Wait

[MCInput]

*/
