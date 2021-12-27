using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class MPhysUtils {
    
    
	
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
