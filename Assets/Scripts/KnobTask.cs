using UnityEngine;

namespace RehabVR
{
    public class KnobTask : TaskBase
    {
        [SerializeField] private KnobInteractable knob;
        private bool completed;

        private void Awake()
        {
            if (knob == null)
            {
                knob = GetComponentInChildren<KnobInteractable>();
            }
        }

        private void Update()
        {
            if (completed || knob == null)
            {
                return;
            }

            if (knob.IsSolved)
            {
                completed = true;
                Metrics.avgPrecision = knob.AngleError;
                MarkCompleted();
            }
        }
    }
}
