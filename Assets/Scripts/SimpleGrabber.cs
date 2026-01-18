using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RehabVR
{
    [RequireComponent(typeof(Collider))]
    public class SimpleGrabber : MonoBehaviour
    {
        [SerializeField] private XRNode xrNode = XRNode.RightHand;
        [SerializeField] private float gripThreshold = 0.6f;
        [SerializeField] private float releaseThreshold = 0.2f;

        private InputDevice device;
        private readonly HashSet<Grabbable> candidates = new HashSet<Grabbable>();
        private Grabbable current;
        private Transform originalParent;

        private void Awake()
        {
            Collider trigger = GetComponent<Collider>();
            trigger.isTrigger = true;
        }

        public void Configure(XRNode node)
        {
            xrNode = node;
            device = InputDevices.GetDeviceAtXRNode(xrNode);
        }

        private void Update()
        {
            if (!device.isValid)
            {
                device = InputDevices.GetDeviceAtXRNode(xrNode);
            }

            if (!device.isValid)
            {
                return;
            }

            if (current == null)
            {
                if (device.TryGetFeatureValue(CommonUsages.grip, out float grip) && grip >= gripThreshold)
                {
                    GrabNearest();
                }
            }
            else
            {
                if (device.TryGetFeatureValue(CommonUsages.grip, out float grip) && grip <= releaseThreshold)
                {
                    Release();
                }
            }
        }

        private void GrabNearest()
        {
            foreach (Grabbable candidate in candidates)
            {
                current = candidate;
                originalParent = current.transform.parent;
                current.transform.SetParent(transform, true);

                if (current.Rigidbody != null)
                {
                    current.Rigidbody.isKinematic = true;
                    current.Rigidbody.useGravity = false;
                }
                break;
            }
        }

        private void Release()
        {
            if (current == null)
            {
                return;
            }

            current.transform.SetParent(originalParent, true);

            if (current.Rigidbody != null)
            {
                current.Rigidbody.isKinematic = false;
                current.Rigidbody.useGravity = true;

                if (device.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity))
                {
                    current.Rigidbody.velocity = velocity;
                }

                if (device.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 angularVelocity))
                {
                    current.Rigidbody.angularVelocity = angularVelocity;
                }
            }

            current = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Grabbable grabbable))
            {
                candidates.Add(grabbable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Grabbable grabbable))
            {
                candidates.Remove(grabbable);
            }
        }
    }
}
