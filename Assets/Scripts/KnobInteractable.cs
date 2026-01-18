using UnityEngine;

namespace RehabVR
{
    public class KnobInteractable : MonoBehaviour
    {
        [SerializeField] private float targetAngle = 90f;
        [SerializeField] private float tolerance = 5f;
        private SimpleGrabber grabber;
        private float currentAngle;

        public bool IsSolved { get; private set; }
        public float AngleError => Mathf.Abs(targetAngle - currentAngle);

        private void Update()
        {
            if (grabber != null)
            {
                Vector3 local = transform.InverseTransformPoint(grabber.transform.position);
                currentAngle = Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
                transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);

                if (!IsSolved && Mathf.Abs(targetAngle - currentAngle) <= tolerance)
                {
                    IsSolved = true;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out SimpleGrabber foundGrabber))
            {
                grabber = foundGrabber;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out SimpleGrabber foundGrabber) && foundGrabber == grabber)
            {
                grabber = null;
            }
        }

        public void ResetKnob()
        {
            IsSolved = false;
            currentAngle = 0f;
            transform.localRotation = Quaternion.identity;
        }

        public void Configure(float newTargetAngle, float newTolerance)
        {
            targetAngle = newTargetAngle;
            tolerance = newTolerance;
        }
    }
}
