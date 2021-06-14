using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Runtime.Data;
using UnityCommons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime {
    public class ResourcesProvider : MonoSingleton<ResourcesProvider> {
        [SerializeField] private string trashMaterialsFolder;
        [SerializeField] private string trashPickupsFolder;
        [SerializeField] private string buildableObjectsFolder;
        [SerializeField] private string cameraName = "Game Camera";
        [SerializeField] private SoundSettings soundSettings;

        private ReadOnlyCollection<TrashMaterial> trashMaterials;
        private ReadOnlyCollection<TrashPickup> trashPickups;
        private ReadOnlyCollection<BuildableObject> buildableObjects;
        private Camera mainCamera;

        protected override void OnAwake() {
            trashMaterials = Resources.LoadAll<TrashMaterial>(trashMaterialsFolder).ToList().AsReadOnly();
            trashPickups = Resources.LoadAll<TrashPickup>(trashPickupsFolder).ToList().AsReadOnly();
            buildableObjects = Resources.LoadAll<BuildableObject>(buildableObjectsFolder).ToList().AsReadOnly();
            LoadCamera();
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnDestroy() {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private void OnSceneChanged(Scene arg0, Scene arg1) {
            LoadCamera();
        }

        private void LoadCamera() {
            var newCamera = GameObject.Find(Instance.cameraName)?.GetComponent<Camera>();
            if (newCamera != null)
                mainCamera = newCamera;
        }

        public static IReadOnlyList<TrashMaterial> TrashMaterials => Instance.trashMaterials;
        public static IReadOnlyList<TrashPickup> TrashPickups => Instance.trashPickups;
        public static IReadOnlyList<BuildableObject> BuildableObjects => Instance.buildableObjects;
        public static Camera MainCamera => Instance.mainCamera;
        public static SoundSettings SoundSettings => Instance.soundSettings;
    }
}