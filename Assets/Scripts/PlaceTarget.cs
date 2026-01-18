using UnityEngine;

namespace RehabVR
{
    [RequireComponent(typeof(Collider))]
    public class PlaceTarget : MonoBehaviour
    {
        [SerializeField] private Grabbable targetPiece;
        public bool IsPlaced { get; private set; }
        public float LastOffset { get; private set; }

        private void Awake()
        {
            Collider trigger = GetComponent<Collider>();
            trigger.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsPlaced)
            {
                return;
            }

            if (targetPiece != null && other.gameObject == targetPiece.gameObject)
            {
                IsPlaced = true;
                LastOffset = Vector3.Distance(transform.position, other.transform.position);
            }
        }

        public void Configure(Grabbable piece)
        {
            targetPiece = piece;
        }
    }
}
