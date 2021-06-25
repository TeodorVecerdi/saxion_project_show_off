using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime.Tutorial {
    public sealed class TutorialController : MonoBehaviour, IEventSubscriber {
        [SerializeField] private List<TutorialSlide> allTutorials;
        [SerializeField] private float transitionFromY = 0.0f;
        [SerializeField] private float transitionToY = 384.0f;
        
        private Dictionary<string, TutorialSlide> tutorialDictionary;
        private TutorialSlide activeTutorialSlide;
        private IDisposable resetTutorialsEventUnsubscribeToken;

        [Button]
        public void ResetTutorials() {
            PlayerPrefs.DeleteKey("current_tutorial");
        }

        private void Awake() {
            resetTutorialsEventUnsubscribeToken = this.Subscribe(EventType.ResetTutorial);
            
            tutorialDictionary = new Dictionary<string, TutorialSlide>();
            foreach (var tutorialSlide in allTutorials) {
                tutorialDictionary.Add(tutorialSlide.TutorialKey, tutorialSlide);
                tutorialSlide.LoadTransitionSettings(transitionFromY, transitionToY);
                
                // ensure all slides are disabled
                tutorialSlide.gameObject.SetActive(false);
            }

            var currentTutorial = PlayerPrefs.GetString("current_tutorial");
            if (string.IsNullOrEmpty(currentTutorial) || !tutorialDictionary.ContainsKey(currentTutorial)) {
                activeTutorialSlide = allTutorials[0];
            } else if (string.Equals(currentTutorial, "none", StringComparison.InvariantCulture)) {
                activeTutorialSlide = null;
            } else {
                activeTutorialSlide = tutorialDictionary[currentTutorial];
            }
        }

        private void Start() {
            if(activeTutorialSlide == null) return;

            activeTutorialSlide.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, transitionToY);
            activeTutorialSlide.gameObject.SetActive(true);
            activeTutorialSlide.Show(2.0f);
        }

        private void OnDestroy() {
            resetTutorialsEventUnsubscribeToken.Dispose();
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case EmptyEvent {Type: EventType.ResetTutorial}: {
                    foreach (var tutorialSlide in allTutorials) {
                        tutorialSlide.ResetTutorial();
                    }

                    var activeTutorial = allTutorials.FirstOrDefault(slide => slide.gameObject.activeSelf);
                    if (activeTutorial == null) {
                        activeTutorialSlide = allTutorials[0];
                        activeTutorialSlide.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, transitionToY);
                        activeTutorialSlide.gameObject.SetActive(true);
                        activeTutorialSlide.Show(2.0f);
                        return true;
                    }
                    
                    activeTutorial.Hide(0.0f).OnComplete(() => {
                        activeTutorial.gameObject.SetActive(false);
                        
                        activeTutorialSlide = allTutorials[0];
                        activeTutorialSlide.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, transitionToY);
                        activeTutorialSlide.gameObject.SetActive(true);
                        activeTutorialSlide.Show(2.0f);
                    });
                    
                    return true;
                }
                default: return false;
            }
        }
    }
}