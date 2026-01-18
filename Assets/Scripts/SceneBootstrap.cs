using System.Collections.Generic;
using UnityEngine;

namespace RehabVR
{
    public class SceneBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            EnsureLogger();
            SetupRig();
            BuildEnvironment();
            BuildLevels();
        }

        private void EnsureLogger()
        {
            if (TrainingSessionLogger.Instance == null)
            {
                GameObject loggerObject = new GameObject("TrainingSessionLogger");
                loggerObject.AddComponent<TrainingSessionLogger>();
            }
        }

        private void SetupRig()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                mainCamera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
            }

            GameObject rig = new GameObject("XR Rig");
            GameObject cameraOffset = new GameObject("Camera Offset");
            cameraOffset.transform.SetParent(rig.transform, false);

            mainCamera.transform.SetParent(cameraOffset.transform, false);
            XRNodeFollower cameraFollower = mainCamera.GetComponent<XRNodeFollower>();
            if (cameraFollower == null)
            {
                cameraFollower = mainCamera.gameObject.AddComponent<XRNodeFollower>();
            }
            cameraFollower.Configure(UnityEngine.XR.XRNode.CenterEye);

            CreateController(cameraOffset.transform, UnityEngine.XR.XRNode.LeftHand, "Left Controller");
            CreateController(cameraOffset.transform, UnityEngine.XR.XRNode.RightHand, "Right Controller");
        }

        private void CreateController(Transform parent, UnityEngine.XR.XRNode node, string name)
        {
            GameObject controller = new GameObject(name);
            controller.transform.SetParent(parent, false);

            XRNodeFollower follower = controller.AddComponent<XRNodeFollower>();
            follower.Configure(node);

            SphereCollider collider = controller.AddComponent<SphereCollider>();
            collider.radius = 0.05f;
            collider.isTrigger = true;

            SimpleGrabber grabber = controller.AddComponent<SimpleGrabber>();
            grabber.Configure(node);

            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.transform.SetParent(controller.transform, false);
            visual.transform.localScale = Vector3.one * 0.08f;
            Destroy(visual.GetComponent<Collider>());
        }

        private void BuildEnvironment()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.position = new Vector3(0f, 0f, 0f);
            floor.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
        }

        private void BuildLevels()
        {
            GameObject levelsRoot = new GameObject("TrainingLevels");
            LevelManager levelManager = levelsRoot.AddComponent<LevelManager>();
            List<TaskBase> tasks = new List<TaskBase>
            {
                BuildPlaceLevel(levelsRoot.transform),
                BuildKnobLevel(levelsRoot.transform),
                BuildButtonLevel(levelsRoot.transform),
                BuildTrayLevel(levelsRoot.transform),
                BuildComboLevel(levelsRoot.transform)
            };

            levelManager.ConfigureTasks(tasks);
        }

        private TaskBase BuildPlaceLevel(Transform parent)
        {
            GameObject level = new GameObject("Level01_Place");
            level.transform.SetParent(parent, false);
            level.transform.position = new Vector3(0f, 1f, 1f);

            PlaceTask task = level.AddComponent<PlaceTask>();
            task.levelId = "Level01_Place";

            PlaceTarget[] targets = new PlaceTarget[3];
            for (int i = 0; i < targets.Length; i++)
            {
                GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
                piece.name = $"Piece_{i}";
                piece.transform.position = level.transform.position + new Vector3(-0.3f + i * 0.3f, 0.1f, 0f);
                piece.transform.localScale = Vector3.one * 0.08f;
                piece.AddComponent<Rigidbody>();
                piece.AddComponent<Grabbable>();

                GameObject target = new GameObject($"Target_{i}");
                target.transform.SetParent(level.transform, false);
                target.transform.position = level.transform.position + new Vector3(-0.3f + i * 0.3f, 0.1f, 0.4f);

                SphereCollider trigger = target.AddComponent<SphereCollider>();
                trigger.radius = 0.08f;
                trigger.isTrigger = true;

                PlaceTarget placeTarget = target.AddComponent<PlaceTarget>();
                placeTarget.Configure(piece.GetComponent<Grabbable>());
                targets[i] = placeTarget;
            }

            task.ConfigureTargets(targets);
            return task;
        }

        private TaskBase BuildKnobLevel(Transform parent)
        {
            GameObject level = new GameObject("Level02_Knob");
            level.transform.SetParent(parent, false);
            level.transform.position = new Vector3(2f, 1f, 1f);

            KnobTask task = level.AddComponent<KnobTask>();
            task.levelId = "Level02_Knob";

            GameObject knobObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            knobObject.name = "Knob";
            knobObject.transform.SetParent(level.transform, false);
            knobObject.transform.localScale = new Vector3(0.2f, 0.05f, 0.2f);
            knobObject.transform.localPosition = Vector3.zero;
            knobObject.GetComponent<Collider>().isTrigger = true;

            KnobInteractable knob = knobObject.AddComponent<KnobInteractable>();
            knob.Configure(90f, 8f);

            return task;
        }

        private TaskBase BuildButtonLevel(Transform parent)
        {
            GameObject level = new GameObject("Level03_Buttons");
            level.transform.SetParent(parent, false);
            level.transform.position = new Vector3(-2f, 1f, 1f);

            ButtonSequenceTask task = level.AddComponent<ButtonSequenceTask>();
            task.levelId = "Level03_Buttons";

            List<PressButton> buttons = new List<PressButton>();
            for (int i = 0; i < 3; i++)
            {
                GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                button.name = $"Button_{i}";
                button.transform.SetParent(level.transform, false);
                button.transform.localPosition = new Vector3(i * 0.25f - 0.25f, 0f, 0f);
                button.transform.localScale = new Vector3(0.1f, 0.02f, 0.1f);
                button.GetComponent<Collider>().isTrigger = true;

                PressButton pressButton = button.AddComponent<PressButton>();
                pressButton.buttonId = i;
                buttons.Add(pressButton);
            }

            task.ConfigureSequence(buttons);
            return task;
        }

        private TaskBase BuildTrayLevel(Transform parent)
        {
            GameObject level = new GameObject("Level04_Tray");
            level.transform.SetParent(parent, false);
            level.transform.position = new Vector3(0f, 1f, -1f);

            TrayBalanceTask task = level.AddComponent<TrayBalanceTask>();
            task.levelId = "Level04_Tray";

            GameObject tray = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tray.name = "Tray";
            tray.transform.SetParent(level.transform, false);
            tray.transform.localScale = new Vector3(0.4f, 0.02f, 0.4f);
            tray.transform.localPosition = Vector3.zero;
            tray.AddComponent<Rigidbody>().isKinematic = true;

            GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.name = "Ball";
            ball.transform.SetParent(level.transform, false);
            ball.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            ball.transform.localScale = Vector3.one * 0.08f;
            Rigidbody ballBody = ball.AddComponent<Rigidbody>();
            ball.AddComponent<Grabbable>();

            task.Configure(ballBody, 5f);
            return task;
        }

        private TaskBase BuildComboLevel(Transform parent)
        {
            GameObject level = new GameObject("Level05_Combo");
            level.transform.SetParent(parent, false);
            level.transform.position = new Vector3(0f, 1f, 2f);

            ComboTask task = level.AddComponent<ComboTask>();
            task.levelId = "Level05_Combo";

            GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            piece.name = "ComboPiece";
            piece.transform.SetParent(level.transform, false);
            piece.transform.localPosition = new Vector3(-0.3f, 0.1f, 0f);
            piece.transform.localScale = Vector3.one * 0.08f;
            piece.AddComponent<Rigidbody>();
            Grabbable grabbable = piece.AddComponent<Grabbable>();

            GameObject target = new GameObject("ComboTarget");
            target.transform.SetParent(level.transform, false);
            target.transform.localPosition = new Vector3(-0.3f, 0.1f, 0.3f);
            SphereCollider trigger = target.AddComponent<SphereCollider>();
            trigger.radius = 0.08f;
            trigger.isTrigger = true;
            PlaceTarget placeTarget = target.AddComponent<PlaceTarget>();
            placeTarget.Configure(grabbable);

            GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            button.name = "ComboButton";
            button.transform.SetParent(level.transform, false);
            button.transform.localPosition = new Vector3(0.2f, 0f, 0f);
            button.transform.localScale = new Vector3(0.1f, 0.02f, 0.1f);
            button.GetComponent<Collider>().isTrigger = true;
            PressButton pressButton = button.AddComponent<PressButton>();

            GameObject knobObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            knobObject.name = "ComboKnob";
            knobObject.transform.SetParent(level.transform, false);
            knobObject.transform.localPosition = new Vector3(0.4f, 0f, 0f);
            knobObject.transform.localScale = new Vector3(0.15f, 0.04f, 0.15f);
            knobObject.GetComponent<Collider>().isTrigger = true;
            KnobInteractable knob = knobObject.AddComponent<KnobInteractable>();
            knob.Configure(45f, 10f);

            task.Configure(placeTarget, pressButton, knob);
            return task;
        }
    }
}
