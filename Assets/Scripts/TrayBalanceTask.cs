using UnityEngine;

namespace RehabVR
{
    public class TrayBalanceTask : TaskBase
    {
        [SerializeField] private Transform tray;
        [SerializeField] private Rigidbody ball;
        [SerializeField] private float requiredDuration = 5f;
        [SerializeField] private float allowedDistance = 0.4f;
        [SerializeField] private float dropHeight = 0.5f;

        private float stableTimer;
        private float stabilitySum;
        private int stabilitySamples;
        private bool completed;

        private void Awake()
        {
            if (tray == null)
            {
                tray = transform;
            }
        }

        public void Configure(Rigidbody newBall, float durationSeconds)
        {
            ball = newBall;
            requiredDuration = durationSeconds;
        }

        private void Update()
        {
            if (completed || ball == null)
            {
                return;
            }

            float distance = Vector3.Distance(tray.position, ball.position);
            bool onTray = distance <= allowedDistance && ball.position.y > tray.position.y - 0.1f;

            if (onTray)
            {
                float tilt = Vector3.Angle(tray.up, Vector3.up);
                stabilitySum += tilt;
                stabilitySamples++;
                stableTimer += Time.deltaTime;
            }
            else
            {
                if (ball.position.y < dropHeight)
                {
                    Metrics.dropCount++;
                }
                stableTimer = 0f;
            }

            if (stableTimer >= requiredDuration)
            {
                completed = true;
                Metrics.avgStability = stabilitySamples > 0 ? stabilitySum / stabilitySamples : 0f;
                MarkCompleted();
            }
        }
    }
}
