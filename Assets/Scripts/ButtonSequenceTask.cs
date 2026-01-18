using System.Collections.Generic;
using UnityEngine;

namespace RehabVR
{
    public class ButtonSequenceTask : TaskBase
    {
        [SerializeField] private List<PressButton> sequence = new List<PressButton>();
        private int currentIndex;
        private bool completed;

        private void Awake()
        {
            if (sequence.Count == 0)
            {
                sequence.AddRange(GetComponentsInChildren<PressButton>());
            }
        }

        public void ConfigureSequence(List<PressButton> orderedButtons)
        {
            sequence = orderedButtons;
        }

        private void Update()
        {
            if (completed || sequence.Count == 0)
            {
                return;
            }

            PressButton current = sequence[currentIndex];
            if (current.WasPressed)
            {
                Metrics.attempts++;
                currentIndex++;

                if (currentIndex >= sequence.Count)
                {
                    completed = true;
                    Metrics.errors = 0;
                    MarkCompleted();
                }
            }

            for (int i = 0; i < sequence.Count; i++)
            {
                if (i != currentIndex && sequence[i].WasPressed)
                {
                    Metrics.errors++;
                    sequence[i].ResetButton();
                }
            }
        }
    }
}
