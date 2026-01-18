using UnityEngine;

namespace RehabVR
{
    [RequireComponent(typeof(Collider))]
    public class PressButton : MonoBehaviour
    {
        public int buttonId;
        public bool WasPressed { get; private set; }

        private void Awake()
        {
            Collider trigger = GetComponent<Collider>();
            trigger.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (WasPressed)
            {
                return;
            }

            if (other.GetComponent<SimpleGrabber>() != null)
            {
                WasPressed = true;
            }
        }

        public void ResetButton()
        {
            WasPressed = false;
        }
    }
}
