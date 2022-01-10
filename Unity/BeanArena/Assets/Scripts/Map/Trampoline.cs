using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour {

    public Vector2 normal;
    public float bounceForce;
    public LayerMask interactMask;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (MUtils.LayerInMask(interactMask, collision.gameObject.layer)) {
            Rigidbody2D otherRb = collision.collider.GetComponentInParent<Rigidbody2D>();

            if(otherRb != null) {
                //otherRb.AddForce()
            }
        }
    }

}
