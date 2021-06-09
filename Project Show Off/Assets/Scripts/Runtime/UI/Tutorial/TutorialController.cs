using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Runtime.Tutorial {
    public class TutorialController : MonoBehaviour {
        [SerializeField] private List<TutorialSlide> allTutorials;
        [SerializeField] private float transitionFromX;
        [SerializeField] private float transitionToX;
        
        private Dictionary<string, TutorialSlide> tutorialDictionary;
        private TutorialSlide activeTutorialSlide;

        [Button]
        public void ResetTutorials() {
            PlayerPrefs.DeleteKey("current_tutorial");
        }

        private void Awake() {
            tutorialDictionary = new Dictionary<string, TutorialSlide>();
            foreach (var tutorialSlide in allTutorials) {
                tutorialDictionary.Add(tutorialSlide.TutorialKey, tutorialSlide);
                tutorialSlide.LoadTransitionSettings(transitionFromX, transitionToX);
                
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

            activeTutorialSlide.GetComponent<RectTransform>().anchoredPosition = new Vector2(transitionToX, -8.0f);
            activeTutorialSlide.gameObject.SetActive(true);
            activeTutorialSlide.Show(2.0f);
        }
    }
}