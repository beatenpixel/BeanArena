using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThinPlatform : MapElement {

    public EdgeCollider2D coll;

    public Bounds bounds;
    public Vector2 point;
    public Transform testPoint;

    private Dictionary<HeroBase, HeroEntry> heroes = new Dictionary<HeroBase, HeroEntry>();
    private Timer updateTimer;

    private Vector2[] edgeWorldPoints;

    public override void Init() {
        updateTimer = new Timer(0.2f);
        edgeWorldPoints = new Vector2[coll.pointCount];
    }

    private void Update() {

        point = testPoint.position;

        if (updateTimer) {
            updateTimer.AddFromNow();

            FindHeroes();
            SolveHeroes();
        }        
    }

    private void FindHeroes() {
        for (int i = 0; i < coll.pointCount; i++) {
            edgeWorldPoints[i] = transform.TransformPoint(coll.points[i] + coll.offset);
        }

        List<HeroBase> heroesToRemove = new List<HeroBase>();

        foreach (var hero in heroes.Keys) {
            if (!Game.inst.map.heroes.Contains(hero)) {
                heroesToRemove.Add(hero);
            }
        }

        foreach (var hero in heroesToRemove) {
            heroes.Remove(hero);
        }

        foreach (var hero in Game.inst.map.heroes) {
            if (!heroes.ContainsKey(hero)) {
                HeroEntry heroEntry = new HeroEntry();
                heroEntry.hero = hero;
                heroes.Add(hero, heroEntry);
            }
        }
    }

    private void SolveHeroes() {
        foreach (var heroEntry in heroes.Values) {
            Bounds b = heroEntry.hero.body.GetBounds();
            b.FillPoints2D(heroEntry.boundsPoints);

            int aboveCount = 0;

            for (int x = 0; x < 4; x++) {
                bool isAbove = true;

                for (int i = 0; i < edgeWorldPoints.Length - 1; i++) {
                    Vector2 line = edgeWorldPoints[i + 1] - edgeWorldPoints[i];
                    line = new Vector2(-line.y, line.x);

                    float cross = Vector2.Dot(line, (heroEntry.boundsPoints[x] - edgeWorldPoints[i]));

                    if (cross < 0) {
                        isAbove = false;
                        break;
                    }

                    //IMDraw.TextMesh(edgeWorldPoints[i], Quaternion.identity, 5f, Color.green, "cross: " + cross.ToString("F2"));
                }

                if (isAbove) {
                    aboveCount++;
                }

                //IMDraw.Sphere3D(heroEntry.boundsPoints[x], 0.1f, isAbove ? Color.red : Color.green);
            }

            float fallThroughTimeThreshold = 0.3f;

            heroEntry.isAbove = aboveCount >= 3;

            if (heroEntry.hero.GetHeroInput().arm.y < -0.3f && heroEntry.hero.GetHeroInput().move.magnitude > 0.05f) {
                heroEntry.isAbove = false;
                heroEntry.notAboveTime = fallThroughTimeThreshold + 0.1f;
            }

            if (!heroEntry.isAbove) {
                heroEntry.notAboveTime += (Time.time - heroEntry.lastCheck);
            } else {
                heroEntry.notAboveTime = 0f;
            }

            //IMDraw.Bounds(b, heroEntry.isAbove?Color.green : Color.red);

            for (int i = 0; i < heroEntry.hero.limbs.Count; i++) {
                if (heroEntry.hero.limbs[i]) {
                    if (heroEntry.isAbove) {
                        MPhysUtils.EnableCollision(heroEntry.hero.limbs[i].colliders, coll, true);
                    } else {
                        if (heroEntry.notAboveTime > fallThroughTimeThreshold) {
                            MPhysUtils.EnableCollision(heroEntry.hero.limbs[i].colliders, coll, false);
                        } else {
                            MPhysUtils.EnableCollision(heroEntry.hero.limbs[i].colliders, coll, true);
                        }
                    }
                }
            }

            heroEntry.lastCheck = Time.time;
        }
    }

    private void OnDrawGizmos() {
        for (int i = 0; i < coll.pointCount; i++) {
            Gizmos.DrawSphere(transform.TransformPoint(coll.points[i] + coll.offset), 0.2f);
        }
    }

    public class HeroEntry {
        public HeroBase hero;
        public bool isAbove;
        public Vector2[] boundsPoints;
        public float notAboveTime;
        public float lastCheck;

        public HeroEntry() {
            boundsPoints = new Vector2[4];
        }
    }

}
