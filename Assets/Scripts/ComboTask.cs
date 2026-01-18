using UnityEngine;

namespace RehabVR
{
    public class ComboTask : TaskBase
    {
        [SerializeField] private PlaceTarget placeTarget;
        [SerializeField] private PressButton pressButton;
        [SerializeField] private KnobInteractable knob;

        private int stage;
        private bool completed;

        private void Update()
        {
            if (completed)
            {
                return;
            }

            if (stage == 0 && placeTarget != null && placeTarget.IsPlaced)
            {
                Metrics.avgPrecision = placeTarget.LastOffset;
                stage = 1;
            }

            if (stage == 1 && pressButton != null && pressButton.WasPressed)
            {
                stage = 2;
            }

            if (stage == 2 && knob != null && knob.IsSolved)
            {
                completed = true;
                Metrics.avgPrecision = (Metrics.avgPrecision + knob.AngleError) * 0.5f;
                MarkCompleted();
            }
        }

        public void Configure(PlaceTarget target, PressButton button, KnobInteractable knobInteractable)
        {
            placeTarget = target;
            pressButton = button;
            knob = knobInteractable;
        }
    }
}
