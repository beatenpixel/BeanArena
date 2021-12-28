using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class MPhysUtils {
    
    public static void EnableCollision(Collider2D a, Collider2D b, bool enable) {
        Physics2D.IgnoreCollision(a, b, !enable);
    }

    public static void EnableCollision(IList<Collider2D> a, Collider2D b, bool enable) {
        int c = a.Count;
        for (int i = 0; i < c; i++) {
            Physics2D.IgnoreCollision(a[i], b, !enable);
        }
    }

    public static void EnableCollision(IList<Collider2D> a, IList<Collider2D> b, bool enable) {
        int aCount = a.Count;
        int bCount = b.Count;

        for (int i = 0; i < aCount; i++) {
            for (int j = 0; j < bCount; j++) {
                if (i != j) {
                    Physics2D.IgnoreCollision(a[i], b[j], !enable);
                }
            }
        }
    }

}

public class RaycastHitCache {

    private RaycastHit[] hits;
    private RaycastHit[] hitsSortedByDst;
    private int cacheSize;

    public RaycastHitCache(int _cacheSize) {
        cacheSize = _cacheSize;
        hits = new RaycastHit[cacheSize];
    }

    public void SortHitsByDistance() {
        hitsSortedByDst = hits.OrderBy(x => x.distance).ToArray();
    }

    public IEnumerator<RaycastHit> GetAllHits(int hitsCount, bool sortByDst) {
        if(sortByDst) {
            for (int i = 0; i < hits.Length; i++) {
                if (i < hitsCount) {
                    yield return hits[i];
                }
            }
        } else {
            for (int i = 0; i < hits.Length; i++) {
                if (i < hitsCount) {
                    yield return hits[i];
                }
            }
        }
    }

    public static implicit operator RaycastHit[](RaycastHitCache cache) {
        return cache.hits;
    }

    public RaycastHit this[int ind] {
        get {
            return hits[ind];
        }
    }

}
