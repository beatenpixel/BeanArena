using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour {

    public string areaName;
    public Vector2 size;

    public Vector2 GetRandomPosition() {
        return (Vector2)transform.position + MRandom.InsideRect(size);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan.SetA(0.5f);
        Gizmos.DrawWireCube(transform.position, size);
    }

}
