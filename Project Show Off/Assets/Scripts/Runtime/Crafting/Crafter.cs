using System;
using System.Collections.Generic;
using Runtime.Data;
using UnityEngine;

namespace Runtime {
    public class Crafter : MonoBehaviour {
        private const string kOpenCrafterHintFormat = "Press {0} to open crafter";

        [SerializeField] private List<CraftingRecipe> recipes;
        [SerializeField] private CrafterView crafterView;
        [SerializeField] private KeyCode openCrafterKey = KeyCode.F;
        
        private PlayerInventory playerInventory;
        private bool canOpenMenu;

        public List<CraftingRecipe> Recipes => recipes;

        private void Update() {
            if(!canOpenMenu || crafterView.IsMenuOpen) return;
            if (Input.GetKeyDown(openCrafterKey)) {
                crafterView.OpenView(this, playerInventory);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if(!other.CompareTag("Player")) return;
            canOpenMenu = true;
            UIHintController.Instance.Add(string.Format(kOpenCrafterHintFormat, openCrafterKey));
            playerInventory = other.GetComponent<PlayerInventory>();
        }

        private void OnTriggerExit(Collider other) {
            if(!other.CompareTag("Player")) return;
            canOpenMenu = false;
            UIHintController.Instance.Remove(string.Format(kOpenCrafterHintFormat, openCrafterKey));
            playerInventory = null;
        }
    }
}