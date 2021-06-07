using System;
using DG.Tweening;
using Runtime.Event;

namespace Runtime.Tutorial {
    public class EnterBuildModeTutorial : TutorialSlide, IEventSubscriber {
        public override string TutorialKey => "enter_build_mode";

        private IDisposable gameModeChangeEventUnsubscribeToken;
        private bool completed;

        protected override void OnAwake() {
            gameModeChangeEventUnsubscribeToken = this.Subscribe(EventType.GameModeChange);
        }

        private void OnDestroy() {
            gameModeChangeEventUnsubscribeToken?.Dispose();
        }

        protected override void Process() {
            if (!completed) return;
            
            completed = false;
            gameModeChangeEventUnsubscribeToken?.Dispose();
            DOTween.To(value => FillAmount = value, 0.0f, 1.0f, 0.25f).OnComplete(FinishTutorial);
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case EmptyEvent {Type: EventType.GameModeChange}: {
                    completed = true;
                    return false;
                }
                default: return false;
            }
        }
    }
}