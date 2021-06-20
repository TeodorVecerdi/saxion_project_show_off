using System;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using UnityEngine.UI;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    [RequireComponent(typeof(Button), typeof(Image))]
    public sealed class BuildableEntry : MonoBehaviour, IEventSubscriber {
        [Header("Settings")]
        [SerializeField] private Color selectedColor;
        [SerializeField] private float selectedColorTransitionDuration = 0.125f;
        [Header("References")]
        [SerializeField] private Image buildableImage;
        [SerializeField] private Image disabledImage;
        [SerializeField] private BuildableEntryTooltip tooltip;
        
        private Button button;
        private Image borderImage;
        private BuildableObject buildableObject;
        private List<IDisposable> eventUnsubscribeTokens;
        private bool isEnabled;

        public MaterialInventory Requirements => buildableObject.ConstructionRequirements;

        private void Awake() {
            button = GetComponent<Button>();
            borderImage = GetComponent<Image>();
            button.onClick.AddListener(OnBuildClicked);

            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.BeginBuild), 
                this.Subscribe(EventType.CancelBuild), 
                this.Subscribe(EventType.PerformBuild)
            };
        }

        private void OnDestroy() {
            foreach (var eventUnsubscribeToken in eventUnsubscribeTokens) {
                eventUnsubscribeToken.Dispose();
            }
            eventUnsubscribeTokens.Clear();
        }

        public void LoadUI(BuildableObject buildableObject) {
            this.buildableObject = buildableObject;
            buildableImage.sprite = buildableObject.ObjectSprite;
            
            tooltip.LoadUI(buildableObject);
        }

        public void SetEnabled(bool isEnabled) {
            this.isEnabled = isEnabled;
            var color = disabledImage.color;
            color.a = isEnabled ? 0.0f : 0.8f;
            disabledImage.color = color;
        }

        public void OnPointerEnter() {
            tooltip.Show(true);
        }

        public void OnPointerExit() {
            tooltip.Show(false);
        }

        private void OnBuildClicked() {
            if(!isEnabled) return;
            SetSelection(true);
            SoundManager.PlaySound("Click");
            EventQueue.QueueEvent(new BeginBuildEvent(this, buildableObject));
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
                    if (beginBuildEvent.Sender is BuildableEntry entry && entry == this || beginBuildEvent.BuildableObject == buildableObject) return false;
                    SetSelection(false);
                    return false;
                }
                case PerformBuildEvent performBuildEvent:
                case EmptyEvent {Type: EventType.CancelBuild}: {
                    SetSelection(false);
                    return false;
                }
                default: return false;
            }
        }
    }
}