using System;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class CrafterView : MonoBehaviour, IEventSubscriber {
        [Header("References")] [SerializeField] private RectTransform recipeContainer;
        [SerializeField] private GameObject mainUI;
        [SerializeField] private Button closeButton;
        [SerializeField] private CraftingRecipeView recipePrefab;

        private bool isMenuOpen;
        private bool isWaitingForInventory;
        private Crafter currentCrafter;
        private IDisposable inventoryResponseEventUnsubscriber;

        public bool IsMenuOpen => isMenuOpen;

        private void Awake() {
            inventoryResponseEventUnsubscriber = EventQueue.Subscribe(this, EventType.DepositInventoryResponse);
        }

        private void OnEnable() {
            InputManager.UIActions.Cancel.performed += CloseViewPerformed;
            closeButton.onClick.AddListener(CloseView);
        }

        private void OnDisable() {
            InputManager.UIActions.Cancel.performed -= CloseViewPerformed;
            closeButton.onClick.RemoveListener(CloseView);
        }

        private void OnDestroy() {
            inventoryResponseEventUnsubscriber.Dispose();
        }

        public void OpenView(Crafter crafter) {
            isMenuOpen = true;
            currentCrafter = crafter;

            UIHintController.Instance.RequestHide(this);
            InputManager.PlayerActions.Disable();
            EventQueue.QueueEvent(new ChangeMouseLockEvent(this, false));
            mainUI.SetActive(true);

            EventQueue.QueueEvent(new EmptyEvent(this, EventType.DepositInventoryRequest));
            isWaitingForInventory = true;
        }

        private void CloseView() {
            if (!isMenuOpen) return;
            isMenuOpen = false;

            UIHintController.Instance.ReleaseHide(this);
            InputManager.PlayerActions.Enable();
            EventQueue.QueueEvent(new ChangeMouseLockEvent(this, true));
            mainUI.SetActive(false);

            // Todo: Use object pooling
            while (recipeContainer.childCount > 0) {
                var child = recipeContainer.GetChild(0);
                child.SetParent(null);
                Destroy(child.gameObject);
            }
        }

        private void LoadUI(MaterialInventory materialInventory) {
            if (currentCrafter == null) {
                Debug.LogError("Attempting to load crafter UI without a crafter or player inventory");
                CloseView();
                return;
            }

            // Todo: Use object pooling
            foreach (var recipe in currentCrafter.Recipes) {
                var recipeView = Instantiate(recipePrefab, recipeContainer);
                recipeView.Build(recipe, materialInventory);
            }
        }

        public void CloseViewPerformed(InputAction.CallbackContext context) {
            CloseView();
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case DepositInventoryResponseEvent responseEvent: {
                    if (!isWaitingForInventory) return false;
                    isWaitingForInventory = false;
                    LoadUI(responseEvent.Inventory);
                    return false;
                }
                default: return false;
            }
        }
    }
}