using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RehabVR
{
    [Serializable]
    public class LevelMetrics
    {
        public string levelId;
        public string startTime;
        public string endTime;
        public float durationSeconds;
        public int attempts;
        public int errors;
        public int dropCount;
        public float avgPrecision;
        public float avgStability;
        public int score;
    }

    [Serializable]
    public class TrainingSession
    {
        public string sessionId;
        public string startedAt;
        public List<LevelMetrics> levels = new List<LevelMetrics>();
    }

    public class TrainingSessionLogger : MonoBehaviour
    {
        public static TrainingSessionLogger Instance { get; private set; }

        [SerializeField] private string fileName = "training_sessions.json";

        private TrainingSession session;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            session = new TrainingSession
            {
                sessionId = Guid.NewGuid().ToString(),
                startedAt = DateTime.UtcNow.ToString("o")
            };
        }

        public LevelMetrics StartLevel(string levelId)
        {
            LevelMetrics metrics = new LevelMetrics
            {
                levelId = levelId,
                startTime = DateTime.UtcNow.ToString("o")
            };
            session.levels.Add(metrics);
            return metrics;
        }

        public void CompleteLevel(LevelMetrics metrics)
        {
            metrics.endTime = DateTime.UtcNow.ToString("o");
            if (DateTime.TryParse(metrics.startTime, out DateTime start) &&
                DateTime.TryParse(metrics.endTime, out DateTime end))
            {
                metrics.durationSeconds = (float)(end - start).TotalSeconds;
            }
            SaveSession();
        }

        private void SaveSession()
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            string json = JsonUtility.ToJson(session, true);
            File.WriteAllText(path, json);
            Debug.Log($"Training session saved: {path}");
        }
    }
}
