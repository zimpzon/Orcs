using UnityEngine;

namespace Assets.Script.Misc
{
    internal class Trails
    {
        public static void DrawJaggedTrailSimple(Vector2 from, Vector2 to)
        {
            var direction = to - from;
            float distance = direction.magnitude;
            direction.Normalize();

            // Create perpendicular vector for jagged offsets
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);

            float trail = distance;
            float jaggedIntensity = 1.5f; // Controls how jagged the trail is

            while (trail > 0)
            {
                // Add random perpendicular offset for jagged effect
                float randomOffset = (Random.value - 0.5f) * jaggedIntensity;
                Vector2 jaggedOffset = perpendicular * randomOffset;

                Vector2 position = from + direction * trail + jaggedOffset;

                Particles.I.ClickTrail.transform.position = position;
                Particles.I.ClickTrail.Emit(2);
                trail -= 0.1f;
            }
        }

        // Reusable buffer to avoid GC from List and array allocations
        private static Vector3[] jaggedTrailPoints = new Vector3[256];

        public static void DrawJaggedTrail(Vector2 from, Vector2 to, LineRenderer lineRenderer)
        {
            var direction = to - from;
            float distance = direction.magnitude;
            direction.Normalize();

            // Create perpendicular vector for jagged offsets
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);

            float stepSize = 0.2f;
            float jaggedIntensity = 0.4f;
            int numSteps = Mathf.CeilToInt(distance / stepSize);
            int maxPoints = numSteps + 1;

            // Resize buffer if needed (double size to reduce allocations)
            if (jaggedTrailPoints.Length < maxPoints)
                jaggedTrailPoints = new Vector3[maxPoints * 2];

            Vector2 previousPosition = from;

            int pointIndex = 0;

            for (int i = 0; i <= numSteps; i++)
            {
                float t = (float)i / numSteps;
                Vector2 straightPosition = Vector2.Lerp(from, to, t);

                float randomOffset = (Random.value - 0.5f) * jaggedIntensity;
                Vector2 jaggedOffset = perpendicular * randomOffset;
                Vector2 currentPosition = straightPosition + jaggedOffset;

                // Save to buffer directly
                jaggedTrailPoints[pointIndex++] = new Vector3(currentPosition.x, currentPosition.y, 0);

                // Emit particles
                if (i > 0)
                {
                    Vector2 segmentDirection = (currentPosition - previousPosition).normalized;
                    float segmentLength = Vector2.Distance(previousPosition, currentPosition);
                    int segmentSteps = Mathf.CeilToInt(segmentLength / stepSize);

                    for (int j = 1; j <= segmentSteps; j++)
                    {
                        float segmentT = (float)j / segmentSteps;
                        Vector2 position = Vector2.Lerp(previousPosition, currentPosition, segmentT);

                        if (Random.value > 0.1)
                        {
                            Particles.I.ClickTrail.transform.position = position;
                            Particles.I.ClickTrail.Emit(2);
                        }
                    }
                }
                else
                {
                    Particles.I.ClickTrail.transform.position = currentPosition;
                    Particles.I.ClickTrail.Emit(1);
                }

                previousPosition = currentPosition;
            }

            // Set positions using the buffer
            lineRenderer.positionCount = pointIndex;
            lineRenderer.SetPositions(jaggedTrailPoints);
        }

        public static void DrawTrail(Vector2 from, Vector2 to)
        {
            var direction = to - from;
            float distance = direction.magnitude;
            direction.Normalize();

            float trail = distance;
            while (trail > 0)
            {
                Particles.I.ClickTrail.transform.position = from + direction * trail;
                Particles.I.ClickTrail.Emit(1);
                trail -= 0.1f;
            }
        }
    }
}
