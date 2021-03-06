﻿using Assets.Script.Enemies;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script
{
    class BlackboardScript
    {
        public static PlayerScript PlayerScript;
        public static Transform PlayerTrans;
        public static List<ActorBase> DeadEnemies = new List<ActorBase>();
        public static List<ActorBase> Enemies = new List<ActorBase>();

        public static void DestroyAllEnemies()
        {
            for (int i = 0; i < Enemies.Count; ++i)
            {
                Enemies[i].ReturnToCache();
            }

            Enemies.Clear();
        }

        public static void DestroyAllCorpses()
        {
            for (int i = 0; i < DeadEnemies.Count; ++i)
            {
                DeadEnemies[i].ReturnToCache();
            }

            DeadEnemies.Clear();
        }

        public static bool CheckOverlap(Vector3 posA, Vector3 posB, float radA, float radB)
        {
            float diffX = posA.x - posB.x;
            float diffY = posA.y - posB.y;
            float dist2 = diffX * diffX + diffY * diffY;
            float radSum = radA + radB;
            return dist2 < radSum * radSum;
        }

        public static bool CheckIntersect(Vector3 posA, Vector3 posB, float radA, float radB)
        {
            float radSum = radA + radB;
            float radDiff = radA - radB;
            float diffX = posA.x - posB.x + radDiff;
            float diffY = posA.y - posB.y + radDiff;
            float dist2 = diffX * diffX + diffY * diffY;
            return radSum * radSum > dist2;
        }

        public struct HitMatch
        {
            public int Idx;
            public bool IsHeadshot;
            public float Distance;
        }

        static void ClosestPointOnLine(float lx1, float ly1, float lx2, float ly2, float x0, float y0, out float cx, out float cy)
        {
            float A1 = ly2 - ly1;
            float B1 = lx1 - lx2;
            float C1 = (ly2 - ly1) * lx1 + (lx1 - lx2) * ly1;
            float C2 = -B1 * x0 + A1 * y0;
            float det = A1 * A1 - -B1 * B1;
            cx = 0;
            cy = 0;
            if (det != 0)
            {
                cx = (A1 * C1 - B1 * C2) / det;
                cy = (A1 * C2 - -B1 * C1) / det;
            }
            else
            {
                cx = x0;
                cy = y0;
            }
        }

        // Start at from and move through to, towards infinity
        static void ClosestPointOnRay(Vector3 from, Vector3 to, Vector3 p, ref Vector3 cp)
        {
            float A1 = to.y - from.y;
            float B1 = from.x - to.x;
            float C1 = (to.y - from.y) * from.x + (from.x - to.x) * from.y;
            float C2 = -B1 * p.x + A1 * p.y;
            float det = A1 * A1 - -B1 * B1;
            cp.x = 0;
            cp.y = 0;
            if (det != 0)
            {
                cp.x = (A1 * C1 - B1 * C2) / det;
                cp.y = (A1 * C2 - -B1 * C1) / det;
            }
            else
            {
                cp.x = p.x;
                cp.y = p.y;
            }

            // Determine if cp is before from, in which case it is not a hit.
            bool okX = to.x > from.x ? cp.x > from.x : cp.x < from.x;
            bool okY = to.y > from.y ? cp.y > from.y : cp.y < from.y;
            if (!(okX && okY))
            {
                cp.x = 10000;
                cp.y = 10000;
            }
        }

        public const int MaxMatches = 50;
        public static HitMatch[] Matches = new HitMatch[MaxMatches];

        public static int GetEnemies(Vector3 from, Vector3 to, int maxCount = MaxMatches)
        {
            int matchIdx = 0;
            Vector3 cp = Vector3.zero;
            for (int i = 0; i < Enemies.Count; ++i)
            {
                if (Enemies[i].Hp <= 0 || Enemies[i].gameObject.layer != GameManager.Instance.LayerEnemy)
                    continue;

                Vector3 circleCenter = Enemies[i].transform.position;
                float rad2 = Enemies[i].RadiusFirstCheck * Enemies[i].RadiusFirstCheck;
                ClosestPointOnRay(from, to, circleCenter, ref cp);
                if (Vector3.SqrMagnitude(circleCenter - cp) < rad2)
                {
                    HitMatch hit = Matches[matchIdx];
                    hit.Idx = i;
                    hit.IsHeadshot = false;

                    // Possible hit
                    circleCenter = Enemies[i].transform.position + Enemies[i].HeadOffset;
                    ClosestPointOnRay(from, to, circleCenter, ref cp);
                    rad2 = Enemies[i].RadiusHead * Enemies[i].RadiusHead;
                    if (Vector3.SqrMagnitude(circleCenter - cp) < rad2)
                    {
                        // Head hit
                        hit.IsHeadshot = true;
                        hit.Distance = Vector3.SqrMagnitude(Enemies[i].transform.position - from);
                        Matches[matchIdx++] = hit;
                    }
                    else
                    {
                        circleCenter = Enemies[i].transform.position + Enemies[i].BodyOffset;
                        ClosestPointOnRay(from, to, circleCenter, ref cp);
                        rad2 = Enemies[i].RadiusBody * Enemies[i].RadiusBody;
                        if (Vector3.SqrMagnitude(circleCenter - cp) < rad2)
                        {
                            // Body hit
                            hit.Distance = Vector3.SqrMagnitude(Enemies[i].transform.position - from);
                            Matches[matchIdx++] = hit;
                        }
                    }

                    if (matchIdx == Matches.Length || matchIdx == maxCount)
                        break;
                }
            }
            return matchIdx;
        }

        public const int MaxColliders = 50;
        public static Collider2D[] Colliders = new Collider2D[MaxColliders];

        public static int GetOverlapped(Vector3 pos, float radius, int layerMask)
        {
            int count = Physics2D.OverlapCircleNonAlloc(pos, radius, Colliders, layerMask);
            return count;
        }

        public static float DistanceToPlayer(Vector3 pos)
        {
            return Vector3.Distance(GameManager.Instance.PlayerTrans.position, pos);
        }

        public static int GetIdxClosestEnemy(Vector3 pos, float radius, int maxCount = MaxMatches)
        {
            int count = GetEnemies(pos, radius, maxCount);
            float closestDist = float.MaxValue;
            int closestIdx = -1;
            for (int i = 0; i < count; ++i)
            {
                if (Matches[i].Distance < closestDist)
                {
                    closestDist = Matches[i].Distance;
                    closestIdx = Matches[i].Idx;
                }
            }
            return closestIdx;
        }

        public static int GetEnemies(Vector3 pos, float radius, int maxCount = MaxMatches)
        {
            int matchIdx = 0;
            for (int i = 0; i < Enemies.Count; ++i)
            {
                if (Enemies[i].Hp <= 0 || Enemies[i].gameObject.layer != GameManager.Instance.LayerEnemy)
                    continue;

                if (CheckOverlap(Enemies[i].transform.position, pos, Enemies[i].RadiusFirstCheck, radius))
                {
                    HitMatch hit = Matches[matchIdx];
                    hit.Idx = i;
                    hit.IsHeadshot = false;

                    // Possible hit
                    if (CheckOverlap(Enemies[i].transform.position + Enemies[i].HeadOffset, pos, Enemies[i].RadiusHead, radius))
                    {
                        // Head hit
                        hit.IsHeadshot = true;
                        hit.Distance = Vector3.SqrMagnitude(Enemies[i].transform.position - pos);
                        Matches[matchIdx++] = hit;
                    }
                    else if (CheckOverlap(Enemies[i].transform.position + Enemies[i].BodyOffset, pos, Enemies[i].RadiusBody, radius))
                    {
                        // Body hit
                        hit.Distance = Vector3.SqrMagnitude(Enemies[i].transform.position - pos);
                        Matches[matchIdx++] = hit;
                    }

                    if (matchIdx == Matches.Length || matchIdx == maxCount)
                        break;
                }
            }
            return matchIdx;
        }

        public static int GetDeadEnemies(Vector3 pos, float radius, int maxCount = MaxMatches)
        {
            int matchIdx = 0;
            for (int i = 0; i < DeadEnemies.Count; ++i)
            {
                if (DeadEnemies[i] != null && DeadEnemies[i].Hp > 0 || !DeadEnemies[i].IsCorpse)
                    continue;

                if (CheckOverlap(DeadEnemies[i].transform.position, pos, DeadEnemies[i].RadiusFirstCheck, radius))
                {
                    HitMatch hit = Matches[matchIdx];
                    hit.Idx = i;
                    hit.Distance = Vector3.SqrMagnitude(DeadEnemies[i].transform.position - pos);
                    Matches[matchIdx++] = hit;

                    if (matchIdx == Matches.Length || matchIdx == maxCount)
                        break;
                }
            }
            return matchIdx;
        }
    }
}
