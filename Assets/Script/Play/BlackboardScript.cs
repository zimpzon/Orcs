﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script
{
    class BlackboardScript
    {
        public static PlayerScript PlayerScript;
        public static Transform PlayerTrans;
        public static List<ActorBase> DeadEnemies = new ();
        static Collider2D[] RawEnemyOverlap = new Collider2D[200];
        public static ActorBase[] EnemyOverlap = new ActorBase[200];

        public static void DestroyAllEnemies()
        {
            var allEnemies = GetAllEnemies();
            for (int i = 0; i < allEnemies.Length; ++i)
            {
                allEnemies[i].ReturnToCache();
            }
        }

        public static ActorBase[] GetAllEnemies()
            => GameObject.FindObjectsOfType<ActorBase>();

        public static void DestroyAllCorpses()
        {
            for (int i = 0; i < DeadEnemies.Count; ++i)
            {
                DeadEnemies[i].ReturnToCache();
            }

            DeadEnemies.Clear();
        }

        //public static bool CheckOverlap(Vector3 posA, Vector3 posB, float rad)
        //{
        //    float diffX = posA.x - posB.x;
        //    float diffY = posA.y - posB.y;
        //    float dist2 = diffX * diffX + diffY * diffY;
        //    return dist2 < rad * rad;
        //}

        //public static bool CheckIntersect(Vector3 posA, Vector3 posB, float radA, float radB)
        //{
        //    float radSum = radA + radB;
        //    float radDiff = radA - radB;
        //    float diffX = posA.x - posB.x + radDiff;
        //    float diffY = posA.y - posB.y + radDiff;
        //    float dist2 = diffX * diffX + diffY * diffY;
        //    return radSum * radSum > dist2;
        //}

        //static void ClosestPointOnLine(float lx1, float ly1, float lx2, float ly2, float x0, float y0, out float cx, out float cy)
        //{
        //    float A1 = ly2 - ly1;
        //    float B1 = lx1 - lx2;
        //    float C1 = (ly2 - ly1) * lx1 + (lx1 - lx2) * ly1;
        //    float C2 = -B1 * x0 + A1 * y0;
        //    float det = A1 * A1 - -B1 * B1;
        //    cx = 0;
        //    cy = 0;
        //    if (det != 0)
        //    {
        //        cx = (A1 * C1 - B1 * C2) / det;
        //        cy = (A1 * C2 - -B1 * C1) / det;
        //    }
        //    else
        //    {
        //        cx = x0;
        //        cy = y0;
        //    }
        //}

        //// Start at from and move through to, towards infinity
        //static void ClosestPointOnRay(Vector3 from, Vector3 to, Vector3 p, ref Vector3 cp)
        //{
        //    float A1 = to.y - from.y;
        //    float B1 = from.x - to.x;
        //    float C1 = (to.y - from.y) * from.x + (from.x - to.x) * from.y;
        //    float C2 = -B1 * p.x + A1 * p.y;
        //    float det = A1 * A1 - -B1 * B1;
        //    cp.x = 0;
        //    cp.y = 0;
        //    if (det != 0)
        //    {
        //        cp.x = (A1 * C1 - B1 * C2) / det;
        //        cp.y = (A1 * C2 - -B1 * C1) / det;
        //    }
        //    else
        //    {
        //        cp.x = p.x;
        //        cp.y = p.y;
        //    }

        //    // Determine if cp is before from, in which case it is not a hit.
        //    bool okX = to.x > from.x ? cp.x > from.x : cp.x < from.x;
        //    bool okY = to.y > from.y ? cp.y > from.y : cp.y < from.y;
        //    if (!(okX && okY))
        //    {
        //        cp.x = 10000;
        //        cp.y = 10000;
        //    }
        //}

        //public const int MaxMatches = 50;
        //public static HitMatch[] Matches = new HitMatch[MaxMatches];

        public const int MaxColliders = 50;
        public static Collider2D[] Colliders = new Collider2D[MaxColliders];

        public static int GetOverlapped(Vector3 pos, float radius, int layerMask)
        {
            int count = Physics2D.OverlapCircleNonAlloc(pos, radius, Colliders, layerMask);
            return count;
        }

        public static float DistanceToPlayer(Vector3 pos)
        {
            return Mathf.Abs(Vector3.Distance(GameManager.Instance.PlayerTrans.position, pos));
        }

        public static int GetEnemies(Vector3 pos, float radius)
        {
            int count = Physics2D.OverlapCircleNonAlloc(pos, radius, RawEnemyOverlap, 1 << GameManager.Instance.LayerEnemy);

            int filterCount = 0;
            for (int i = 0; i < count; ++i)
            {
                var enemy = RawEnemyOverlap[i].gameObject.GetComponent<ActorBase>();
                if (enemy.Hp > 0)
                {
                    EnemyOverlap[filterCount++] = enemy;
                }
            }

            return filterCount;
        }

        public static int CountEnemies(Vector3 pos, float radius)
        {
            return Physics2D.OverlapCircleNonAlloc(pos, radius, RawEnemyOverlap, 1 << GameManager.Instance.LayerEnemy);
        }

        public static ActorBase GetIdxClosestEnemy(Vector3 pos, float radius)
        {
            int count = Physics2D.OverlapCircleNonAlloc(pos, radius, RawEnemyOverlap, 1 << GameManager.Instance.LayerEnemy);

            float closestDist = float.MaxValue;
            ActorBase closestEnemy = null;

            for (int i = 0; i < count; ++i)
            {
                var enemy = RawEnemyOverlap[i].gameObject.GetComponent<ActorBase>();

                if (enemy != null && enemy.Hp > 0)
                {
                    float distance = Vector3.Distance(pos, enemy.transform.position);
                    if (distance < closestDist)
                    {
                        closestDist = distance;
                        closestEnemy = enemy;
                    }
                }
            }
            return closestEnemy;
        }

        public static int GetDeadEnemies(Vector3 pos, float radius)
        {
            int count = Physics2D.OverlapCircleNonAlloc(pos, radius, RawEnemyOverlap, 1 << GameManager.Instance.LayerEnemy);

            int filterCount = 0;
            for (int i = 0; i < count; ++i)
            {
                var enemy = RawEnemyOverlap[i].gameObject.GetComponent<ActorBase>();
                if (enemy.Hp <= 0)
                {
                    EnemyOverlap[filterCount++] = enemy;
                }
            }

            return filterCount;
        }
    }
}
