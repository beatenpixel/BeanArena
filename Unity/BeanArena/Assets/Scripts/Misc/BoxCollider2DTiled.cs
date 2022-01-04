using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoxCollider2DTiled : MonoBehaviour {

    private SpriteRenderer spriteRend;
    private BoxCollider2D boxCollider;

    private void Update() {
        if (transform.hasChanged) {
            if (spriteRend == null) spriteRend = GetComponent<SpriteRenderer>();
            if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();

            if (boxCollider != null && spriteRend != null) {
                boxCollider.size = spriteRend.size;
            }
        }
    }

}