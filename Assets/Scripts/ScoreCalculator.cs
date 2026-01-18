using UnityEngine;

namespace RehabVR
{
    public static class ScoreCalculator
    {
        public static int CalculateScore(LevelMetrics metrics)
        {
            float timePenalty = metrics.durationSeconds * 0.5f;
            float errorPenalty = metrics.errors * 5f;
            float dropPenalty = metrics.dropCount * 10f;
            float precisionPenalty = metrics.avgPrecision * 20f;
            float stabilityPenalty = metrics.avgStability * 10f;

            float raw = 100f - timePenalty - errorPenalty - dropPenalty - precisionPenalty - stabilityPenalty;
            return Mathf.Clamp(Mathf.RoundToInt(raw), 0, 100);
        }
    }
}
