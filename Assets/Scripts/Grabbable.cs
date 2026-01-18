using UnityEngine;

namespace RehabVR
{
    public class Grabbable : MonoBehaviour
    {
        public Rigidbody Rigidbody { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }
    }
}
