using System;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    [RequireComponent(typeof(Button), typeof(Image))]
    public class BuildableEntry : MonoBehaviour, IEventSubscriber {
        [Header("Settings")]
        [SerializeField] private Color selectedColor;
        [SerializeField] private float selectedColorTransitionDuration = 0.125f;
        [Header("References")]
        [SerializeField] private Image buildableImage;
        
        private Button button;
        private Image borderImage;
        private BuildableObjectPreview prefab;
        private List<IDisposable> eventUnsubscribers;

        private void Awake() {
            button = GetComponent<Button>();
            borderImage = GetComponent<Image>();
            button.onClick.AddListener(OnBuildClicked);
            
            eventUnsubscribers = new List<IDisposable>();
            eventUnsubscribers.Add(EventQueue.Subscribe(this, EventType.BeginBuild));
            eventUnsubscribers.Add(EventQueue.Subscribe(this, EventType.CancelBuild));
            eventUnsubscribers.Add(EventQueue.Subscribe(this, EventType.PerformBuild));
        }

        private void OnDestroy() {
            foreach (var eventUnsubscriber in eventUnsubscribers) {
                eventUnsubscriber.Dispose();
            }
            eventUnsubscribers.Clear();
        }

        public void BuildUI(BuildableObject buildableObject) {
            buildableImage.sprite = buildableObject.ObjectSprite;
            prefab = buildableObject.Prefab;
        }

        private void OnBuildClicked() {
            SetSelection(true);
            EventQueue.QueueEvent(new BeginBuildEvent(this, prefab));
        }

        private void SetSelection(bool selected) {
            borderImage.DOColor(selected ? selectedColor : Color.white, selectedColorTransitionDuration);
        }
        

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case BeginBuildEvent beginBuildEvent: {
                    if (beginBuildEvent.Sender is BuildableEntry entry && entry == this || beginBuildEvent.Prefab == prefab) return false;
                    SetSelection(false);
                    return false;
                }
                case EmptyEvent {Type: EventType.PerformBuild}:
                case EmptyEvent {Type: EventType.CancelBuild}: {
                    SetSelection(false);
                    return false;
                }
                default: return false;
            }
        }
    }
}