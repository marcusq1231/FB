using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RehabVR.Editor
{
    public static class SampleSceneSetup
    {
        private const string SampleScenePath = "Assets/Scenes/SampleScene.unity";

        [InitializeOnLoadMethod]
        private static void RegisterSceneHook()
        {
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (scene.path != SampleScenePath)
            {
                return;
            }

            if (GameObject.Find("SceneBootstrap") != null)
            {
                return;
            }

            GameObject bootstrap = new GameObject("SceneBootstrap");
            bootstrap.AddComponent<SceneBootstrap>();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }
}
