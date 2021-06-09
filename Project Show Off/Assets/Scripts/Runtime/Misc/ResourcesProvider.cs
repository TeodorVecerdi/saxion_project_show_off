﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Runtime.Data;
using UnityCommons;
using UnityEngine;

namespace Runtime {
    public class ResourcesProvider : MonoSingleton<ResourcesProvider> {
        [SerializeField] private string trashMaterialsFolder;
        [SerializeField] private string trashPickupsFolder;
        [SerializeField] private string buildableObjectsFolder;
        
        private ReadOnlyCollection<TrashMaterial> trashMaterials;
        private ReadOnlyCollection<TrashPickup> trashPickups;
        private ReadOnlyCollection<BuildableObject> buildableObjects;

        protected override void OnAwake() {
            trashMaterials = Resources.LoadAll<TrashMaterial>(trashMaterialsFolder).ToList().AsReadOnly();
            trashPickups = Resources.LoadAll<TrashPickup>(trashPickupsFolder).ToList().AsReadOnly();
            buildableObjects = Resources.LoadAll<BuildableObject>(buildableObjectsFolder).ToList().AsReadOnly();
        }
        
        public static IReadOnlyList<TrashMaterial> TrashMaterials => Instance.trashMaterials;
        public static IReadOnlyList<TrashPickup> TrashPickups => Instance.trashPickups;
        public static IReadOnlyList<BuildableObject> BuildableObjects => Instance.buildableObjects;
    }
}