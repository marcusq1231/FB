using System.Linq;
using UnityEngine;

namespace RehabVR
{
    public class PlaceTask : TaskBase
    {
        private PlaceTarget[] targets;
        private bool completed;

        private void Awake()
        {
            targets = GetComponentsInChildren<PlaceTarget>();
        }

        public void ConfigureTargets(PlaceTarget[] newTargets)
        {
            targets = newTargets;
        }

        private void Update()
        {
            if (completed || targets == null || targets.Length == 0)
            {
                return;
            }

            if (targets.All(target => target.IsPlaced))
            {
                completed = true;
                Metrics.avgPrecision = targets.Average(target => target.LastOffset);
                MarkCompleted();
            }
        }

        public override void BeginTask()
        {
            base.BeginTask();
            Metrics.attempts = targets.Length;
        }
    }
}
