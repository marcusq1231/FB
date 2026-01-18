using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RehabVR
{
    public class XRNodeFollower : MonoBehaviour
    {
        [SerializeField] private XRNode xrNode = XRNode.LeftHand;
        [SerializeField] private bool updatePosition = true;
        [SerializeField] private bool updateRotation = true;

        private InputDevice device;

        private void OnEnable()
        {
            TryInitialize();
        }

        public void Configure(XRNode node)
        {
            xrNode = node;
            TryInitialize();
        }

        private void TryInitialize()
        {
            device = InputDevices.GetDeviceAtXRNode(xrNode);
        }

        private void Update()
        {
            if (!device.isValid)
            {
                TryInitialize();
            }

            if (updatePosition && device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position))
            {
                transform.localPosition = position;
            }

            if (updateRotation && device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation))
            {
                transform.localRotation = rotation;
            }
        }
    }
}
