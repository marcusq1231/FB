using System;
using UnityEngine;

namespace RehabVR
{
    public abstract class TaskBase : MonoBehaviour
    {
        public string levelId = "Level";
        public event Action<TaskBase> Completed;

        public LevelMetrics Metrics { get; private set; }

        public virtual void BeginTask()
        {
            Metrics = TrainingSessionLogger.Instance.StartLevel(levelId);
        }

        protected void MarkCompleted()
        {
            Completed?.Invoke(this);
        }

        public virtual void EndTask()
        {
            if (Metrics == null)
            {
                return;
            }

            Metrics.score = ScoreCalculator.CalculateScore(Metrics);
            TrainingSessionLogger.Instance.CompleteLevel(Metrics);
        }
    }
}
