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
                Particles.I.ClickTrail.Emit(1);
                trail -= 0.1f;
            }
        }

        public static void DrawJaggedTrail(Vector2 from, Vector2 to)
        {
            var direction = to - from;
            float distance = direction.magnitude;
            direction.Normalize();

            // Create perpendicular vector for jagged offsets
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);

            float stepSize = 0.2f;
            float jaggedIntensity = 0.4f; // Controls how jagged the trail is
            int numSteps = Mathf.CeilToInt(distance / stepSize);

            Vector2 previousPosition = from;

            for (int i = 0; i <= numSteps; i++)
            {
                // Calculate position along straight line
                float t = (float)i / numSteps;
                Vector2 straightPosition = Vector2.Lerp(from, to, t);

                // Add random perpendicular offset for jagged effect
                float randomOffset = (Random.value - 0.5f) * jaggedIntensity;
                Vector2 jaggedOffset = perpendicular * randomOffset;
                Vector2 currentPosition = straightPosition + jaggedOffset;

                // If not the first point, interpolate between previous and current to maintain step size
                if (i > 0)
                {
                    Vector2 segmentDirection = (currentPosition - previousPosition).normalized;
                    float segmentLength = Vector2.Distance(previousPosition, currentPosition);
                    int segmentSteps = Mathf.CeilToInt(segmentLength / stepSize);

                    for (int j = 1; j <= segmentSteps; j++)
                    {
                        float segmentT = (float)j / segmentSteps;
                        Vector2 position = Vector2.Lerp(previousPosition, currentPosition, segmentT);

                        Particles.I.ClickTrail.transform.position = position;
                        Particles.I.ClickTrail.Emit(1);
                    }
                }
                else
                {
                    // First particle at start position
                    Particles.I.ClickTrail.transform.position = currentPosition;
                    Particles.I.ClickTrail.Emit(1);
                }

                previousPosition = currentPosition;
            }
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
