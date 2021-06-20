using System;
using DG.Tweening;
using Runtime.Event;
using UnityEngine;
using UnityEngine.UI;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public sealed class BuildModeTransition : MonoBehaviour, IEventSubscriber {
        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.25f;

        [Header("References")]
        [SerializeField] private Image fadeImage;

        private IDisposable gameModeToggleEventUnsubscribeToken;
        private bool waitingToSendEvent;

        private void Awake() {
            gameModeToggleEventUnsubscribeToken = this.Subscribe(EventType.GameModeChange);
        }

        private void Start() {
            InputManager.BuildModeActions.Disable();
        }

        private void OnDestroy() {
            gameModeToggleEventUnsubscribeToken.Dispose();
        }

        private void StartFade() {
            if (waitingToSendEvent) {
                // if mid-fade wasn't reached it would cause build-mode states to become de-synced
                EventQueue.RaiseEventImmediately(new EmptyEvent(this, EventType.GameModeToggle));
                fadeImage.DOKill();
            }
            waitingToSendEvent = true;
            
            if (GeneralInput.IsBuildModeActive) EnterBuildMode();
            else ExitBuildMode();
        }

        private void EnterBuildMode() {
            InputManager.PlayerActions.Disable();
            
            fadeImage.DOFade(1.0f, fadeDuration).From(0.0f).OnComplete(() => {
                EventQueue.QueueEvent(new EmptyEvent(this, EventType.GameModeToggle));
                waitingToSendEvent = false;
                fadeImage.DOFade(0.0f, fadeDuration).From(1.0f).OnComplete(() => {
                    InputManager.BuildModeActions.Enable();
                });
            });
        }
        
        private void ExitBuildMode() {
            InputManager.BuildModeActions.Disable();
            
            fadeImage.DOFade(1.0f, fadeDuration).From(0.0f).OnComplete(() => {
                EventQueue.QueueEvent(new EmptyEvent(this, EventType.GameModeToggle));
                waitingToSendEvent = false;
                fadeImage.DOFade(0.0f, fadeDuration).From(1.0f).OnComplete(() => {
                    InputManager.PlayerActions.Enable();
                });
            });
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case EmptyEvent {Type: EventType.GameModeChange}: {
                    StartFade();
                    return false;
                }
                default: return false;
            }
        }
    }
}