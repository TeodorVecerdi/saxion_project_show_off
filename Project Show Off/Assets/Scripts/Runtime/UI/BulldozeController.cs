using System;
using System.Collections.Generic;
using Runtime.Event;
using UnityEngine;
using UnityEngine.InputSystem;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    [RequireComponent(typeof(BuildableToolbarController))]
    public class BulldozeController : MonoBehaviour, IEventSubscriber {
        [SerializeField] private BulldozeConfirmation bulldozeConfirmation;

        private BuildableToolbarController toolbarController;
        private BulldozeObject currentObject;
        private bool isBulldozing;
        private bool oldIsBulldozing;
        private bool isShowingConfirmation;

        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            toolbarController = GetComponent<BuildableToolbarController>();
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.GameModeChange)
            };
        }

        private void OnDestroy() {
            foreach (var eventUnsubscribeToken in eventUnsubscribeTokens) {
                eventUnsubscribeToken.Dispose();
            }

            eventUnsubscribeTokens.Clear();
        }

        private void Update() {
            if (!isBulldozing || isShowingConfirmation) return;
            if (!oldIsBulldozing) {
                // hacky way to skip a frame
                oldIsBulldozing = isBulldozing;
                return;
            }

            Debug.Log("UPDATE");

            if (InputManager.CancelBuildTriggered) {
                StopBulldoze();
            }

            if (InputManager.PerformBuildTriggered && currentObject != null) {
                bulldozeConfirmation.Show();
                isShowingConfirmation = true;
            }

            var mouse = Mouse.current;
            if (!(mouse is {wasUpdatedThisFrame: true})) return;

            var mousePosition = mouse.position.ReadValue();
            var ray = ResourcesProvider.MainCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out var hit, 100_000.0f, LayerMask.GetMask("Bulldoze"))) {
                var hitObject = hit.transform.GetComponent<BulldozeObject>();
                
                if (hitObject == currentObject) return;
                if(currentObject != null) currentObject.HideIndicator();
                hitObject.ShowIndicator();
                currentObject = hitObject;
            } else {
                if (currentObject != null) {
                    currentObject.HideIndicator();
                    currentObject = null;
                }
            }
        }

        private void BeginBulldoze() {
            if (isBulldozing) return;
            isBulldozing = true;
            toolbarController.SetViewVisible(false);
            SoundManager.PlaySound("Click");
        }

        private void StopBulldoze() {
            if (!isBulldozing) return;
            isBulldozing = false;
            oldIsBulldozing = false;
            if (currentObject != null)
                currentObject.HideIndicator();
            currentObject = null;
            toolbarController.SetViewVisible(true);
            SoundManager.PlaySound("Click");
        }

        public void OnBulldozeConfirmClicked() {
            isShowingConfirmation = false;
            oldIsBulldozing = false;
            bulldozeConfirmation.Hide();
            currentObject.Bulldoze();
            currentObject = null;
            SoundManager.PlaySound("Click");
        }

        public void OnBulldozeCancelClicked() {
            isShowingConfirmation = false;
            oldIsBulldozing = false;
            bulldozeConfirmation.Hide();
            SoundManager.PlaySound("Click");
        }

        public void OnBulldozeButtonClicked() {
            BeginBulldoze();
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case EmptyEvent {Type: EventType.GameModeChange}: {
                    if (!isBulldozing || GeneralInput.IsBuildModeActive) return false;
                   
                    if (isShowingConfirmation) {
                        bulldozeConfirmation.Hide();
                        isShowingConfirmation = false;
                    }
                    isBulldozing = false;
                    oldIsBulldozing = false;
                    toolbarController.SetViewVisible(true);
                    if (currentObject != null)
                        currentObject.HideIndicator();
                    currentObject = null;
                    return false;
                }
                default: return false;
            }
        }
    }
}