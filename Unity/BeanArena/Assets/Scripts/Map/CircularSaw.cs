using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularSaw : MonoBehaviour {

    public float rotSpeed = 180f;

    private void Update() {
        transform.rotation *= Quaternion.Euler(0, 0, rotSpeed * Time.deltaTime);
    }

}
