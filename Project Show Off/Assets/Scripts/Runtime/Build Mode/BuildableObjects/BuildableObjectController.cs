﻿using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Data;
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
        
        private BuildableObject currentBuildable;
        private BuildableObjectPreview currentObject;
        private Transform currentTransform;
        private Quaternion newRotation;
        private Quaternion y180deg;
        private bool isBuilding;
        private bool oldIsBuilding;
        private bool isValidSpot;

        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.BeginBuild), 
                this.Subscribe(EventType.CancelBuild)
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
                
                Destroy(currentObject.gameObject);
                currentTransform = null;
                currentObject = null;
                currentBuildable = null;
                isBuilding = false;
                oldIsBuilding = false;
                isValidSpot = false;
                return;
            }

            if (InputManager.PerformBuildTriggered && isValidSpot) {
                EventQueue.QueueEvent(new PerformBuildEvent(this, currentBuildable));
                currentTransform = null;
                currentObject = null;
                currentBuildable = null;
                isBuilding = false;
                oldIsBuilding = false;
                isValidSpot = false;
                return;
            }
            
            if(Mouse.current == null) return;
            
            var mousePosition = Mouse.current.position.ReadValue();
            var ray = buildModeCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out var hit, 10000.0f, LayerMask.GetMask("Ground"))) {
                if ((currentTransform.position - hit.point).sqrMagnitude > 0.01f) {
                    currentTransform.position = hit.point;
                    isValidSpot = false;
                    //Todo: Optimize using raycast and physical meshes?
                    foreach (var quad in currentBuildable.BuildAreas.SelectMany(area => area.Quads)) {
                        var points = quad.Points;
                        if (Utilities.IsMouseInQuad(mousePosition, buildModeCamera.WorldToScreenPoint(points[0]), buildModeCamera.WorldToScreenPoint(points[1]),
                                                    buildModeCamera.WorldToScreenPoint(points[2]), buildModeCamera.WorldToScreenPoint(points[3]))) {
                            isValidSpot = true;
                            break;
                        }
                    }
                }
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
                    if (currentBuildable == beginBuildEvent.BuildableObject) return false;
                    currentBuildable = beginBuildEvent.BuildableObject;
                    currentObject = Instantiate(currentBuildable.Prefab, buildModeCenter.position, buildModeCenter.rotation * y180deg);
                    currentTransform = currentObject.transform;
                    newRotation = currentTransform.rotation;
                    isBuilding = true;
                    isValidSpot = false;
                    return false;
                }
                default: return false;
            }
        }
    }
}