using System.Collections.Generic;
using UnityEngine;

namespace RehabVR
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<TaskBase> tasks = new List<TaskBase>();
        private int currentIndex;

        private void Start()
        {
            if (tasks.Count == 0)
            {
                tasks.AddRange(GetComponentsInChildren<TaskBase>());
            }

            if (tasks.Count == 0)
            {
                Debug.LogWarning("No tasks found for LevelManager.");
                return;
            }

            currentIndex = 0;
            StartTask(tasks[currentIndex]);
        }

        public void ConfigureTasks(List<TaskBase> configuredTasks)
        {
            tasks = configuredTasks;
        }

        private void StartTask(TaskBase task)
        {
            foreach (TaskBase t in tasks)
            {
                t.gameObject.SetActive(t == task);
            }

            task.Completed += HandleTaskCompleted;
            task.BeginTask();
        }

        private void HandleTaskCompleted(TaskBase task)
        {
            task.Completed -= HandleTaskCompleted;
            task.EndTask();
            currentIndex++;

            if (currentIndex < tasks.Count)
            {
                StartTask(tasks[currentIndex]);
            }
            else
            {
                Debug.Log("All tasks completed.");
            }
        }
    }
}
