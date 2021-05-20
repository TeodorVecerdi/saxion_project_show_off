﻿using System;
using System.Collections.Generic;
using Runtime.Event;
using UnityEngine;
using UnityEngine.InputSystem;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class BuildableObjectController : MonoBehaviour, IEventSubscriber {
        [Header("Settings")]
        [SerializeField] private float buildableRotationSpeed = 45.0f;
        [SerializeField] private float buildableRotationTime = 5.0f;
        [Header("References")]
        [SerializeField] private Transform buildModeCenter;
        [SerializeField] private Camera buildModeCamera;
        
        private BuildableObjectPreview currentPrefab;
        private BuildableObjectPreview currentObject;
        private Transform currentTransform;
        private Quaternion newRotation;
        private Quaternion y180deg;
        private bool isBuilding;
        private bool oldIsBuilding;

        private List<IDisposable> eventUnsubscribers;

        private void Awake() {
            eventUnsubscribers = new List<IDisposable>();
            eventUnsubscribers.Add(EventQueue.Subscribe(this, EventType.BeginBuild));
            eventUnsubscribers.Add(EventQueue.Subscribe(this, EventType.CancelBuild));
            y180deg = Quaternion.Euler(180.0f * Vector3.up);
        }

        private void OnDestroy() {
            foreach (var eventUnsubscriber in eventUnsubscribers) {
                eventUnsubscriber.Dispose();
            }
            eventUnsubscribers.Clear();
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
                
                Destroy(currentObject.gameObject);
                currentTransform = null;
                currentObject = null;
                currentPrefab = null;
                isBuilding = false;
                oldIsBuilding = false;
                return;
            }

            if (InputManager.PerformBuildTriggered) {
                // todo yeet and check for valid spot or check earlier while moving????
                currentTransform = null;
                currentObject = null;
                currentPrefab = null;
                isBuilding = false;
                oldIsBuilding = false;
                EventQueue.QueueEvent(new EmptyEvent(this, EventType.PerformBuild));
                return;
            }
            
            if(Mouse.current == null) return;
            
            var mousePosition = Mouse.current.position.ReadValue();
            var ray = buildModeCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out var hit, 10000.0f, LayerMask.GetMask("Ground"))) {
                currentTransform.position = hit.point;
                // todo check for valid spot
            }

            var rotationDelta = InputManager.ObjectRotation;
            newRotation *= Quaternion.Euler(-rotationDelta * buildableRotationSpeed * Time.deltaTime * Vector3.up);
            currentTransform.rotation = Quaternion.Slerp(currentTransform.rotation, newRotation, buildableRotationTime * Time.deltaTime);
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case BeginBuildEvent beginBuildEvent: {
                    if (currentPrefab == beginBuildEvent.Prefab) return false;
                    currentPrefab = beginBuildEvent.Prefab;
                    currentObject = Instantiate(currentPrefab, buildModeCenter.position, buildModeCenter.rotation * y180deg);
                    currentTransform = currentObject.transform;
                    newRotation = currentTransform.rotation;
                    isBuilding = true;
                    return false;
                }
                default: return false;
            }
        }
    }
}