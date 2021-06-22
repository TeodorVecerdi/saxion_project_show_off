using System;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using UnityEngine.InputSystem;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public sealed class BuildableObjectController : MonoBehaviour, IEventSubscriber {
        [Header("Settings")]
        [SerializeField] private float buildableRotationSpeed = 45.0f;
        [SerializeField] private float buildableRotationTime = 5.0f;
        [Space]
        [SerializeField] private Color buildIndicatorValidColor;
        [SerializeField] private Color buildIndicatorInvalidColor;
        [Header("References")]
        [SerializeField] private Transform buildModeCenter;
        [SerializeField] private Material buildLocationIndicator;
        
        private BuildableObject currentBuildable;
        private BuildableObjectPreview currentObject;
        private Transform currentTransform;
        private Quaternion newRotation;
        private Quaternion y180deg;
        private bool isBuilding;
        private bool oldIsBuilding;
        private bool isValidSpot;
       
        private bool IsValidSpot {
            get => isValidSpot;
            set {
                if(value == isValidSpot) return;
                buildLocationIndicator.DOColor(value ? buildIndicatorValidColor : buildIndicatorInvalidColor, 0.125f);
                isValidSpot = value;
            }
        }

        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.BeginBuild), 
                this.Subscribe(EventType.GameModeChange)
            };
            y180deg = Quaternion.Euler(180.0f * Vector3.up);
        }

        private void OnDestroy() {
            foreach (var eventUnsubscribeToken in eventUnsubscribeTokens) {
                eventUnsubscribeToken.Dispose();
            }
            eventUnsubscribeTokens.Clear();
        }

        private void Update() {
            if(!isBuilding) return;
            if (!oldIsBuilding) {
                // hacky way to skip a frame
                oldIsBuilding = isBuilding;
                return;
            }

            if (InputManager.CancelBuildTriggered) {
                EventQueue.QueueEvent(new EmptyEvent(this, EventType.CancelBuild));
                SoundManager.PlaySound("Click");
                Destroy(currentObject.gameObject);
                ObjectCleanup();
                return;
            }

            if (InputManager.PerformBuildTriggered && IsValidSpot) {
                EventQueue.QueueEvent(new PerformBuildEvent(this, currentBuildable));
                SoundManager.PlaySound("Build");
                ObjectCleanup();
                return;
            }
            
            if(Mouse.current == null) return;
            
            var mousePosition = Mouse.current.position.ReadValue();
            var ray = ResourcesProvider.MainCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out var hit, 100_000.0f, LayerMask.GetMask("Ground"))) {
                if ((currentTransform.position - hit.point).sqrMagnitude > 0.001f) {
                    IsValidSpot = Physics.Raycast(ray, 100_000.0f, LayerMask.GetMask("Build Area"));
                }
                currentTransform.position = hit.point;
            }

            var rotationDelta = InputManager.ObjectRotation;
            newRotation *= Quaternion.Euler(-rotationDelta * buildableRotationSpeed * Time.deltaTime * Vector3.up);
            currentTransform.rotation = Quaternion.Slerp(currentTransform.rotation, newRotation, buildableRotationTime * Time.deltaTime);
        }

        private void ObjectCleanup() {
            currentTransform = null;
            currentObject = null;
            currentBuildable = null;
            isBuilding = false;
            oldIsBuilding = false;
            IsValidSpot = false;
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case BeginBuildEvent beginBuildEvent: {
                    if (currentBuildable == beginBuildEvent.BuildableObject) return false;
                    currentBuildable = beginBuildEvent.BuildableObject;
                    currentObject = Instantiate(currentBuildable.Prefab, buildModeCenter.position, buildModeCenter.rotation * y180deg);
                    currentTransform = currentObject.transform;
                    newRotation = currentTransform.rotation;
                    isBuilding = true;
                    IsValidSpot = false;
                    return false;
                }
                case EmptyEvent {Type: EventType.GameModeChange}: {
                    if (currentObject != null) {
                        EventQueue.QueueEvent(new EmptyEvent(this, EventType.CancelBuild));
                
                        Destroy(currentObject.gameObject);
                        ObjectCleanup();
                    }

                    return false;
                }
                default: return false;
            }
        }
    }
}