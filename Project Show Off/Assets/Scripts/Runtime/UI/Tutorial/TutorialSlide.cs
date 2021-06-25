using System;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityCommons;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Tutorial {
    [RequireComponent(typeof(TutorialContent))]
    public abstract class TutorialSlide : MonoBehaviour {
        [HorizontalLine(color: EColor.Red, order = -10000), Header("Other Settings", order = -20000)]
        [SerializeField] private TutorialSlide nextTutorial;
        [SerializeField] protected bool overrideTransitionSettings;
        [SerializeField, ShowIf("overrideTransitionSettings")] private float transitionDelay;
        [SerializeField, ShowIf("overrideTransitionSettings")] private float transitionDuration;
        
        private const float baseTransitionDuration = 0.5f;
        private RectTransform rectTransform;
        private float transitionFromY;
        private float transitionToY;
        private bool isFinished;
        private TutorialContent tutorialContent;

        protected float FillAmount;
        public abstract string TutorialKey { get; }
        protected abstract void Process();
        protected abstract void OnReset();
        
        protected virtual void OnAwake() {}

        protected void FinishTutorial() {
            Debug.Log($"Finished tutorial {TutorialKey}");
            isFinished = true;
            Hide(1.0f).OnComplete(() => {
                gameObject.SetActive(false);
                if (nextTutorial != null) {
                    nextTutorial.gameObject.SetActive(true);
                    nextTutorial.Show(1.0f);
                    PlayerPrefs.SetString("current_tutorial", nextTutorial.TutorialKey);
                } else {
                    PlayerPrefs.SetString("current_tutorial", "none");
                }
            });
        }

        private void Awake() {
            rectTransform = (RectTransform) transform;
            tutorialContent = GetComponent<TutorialContent>();
            OnAwake();
        }

        private void Update() {
            if (isFinished) return;
            
            Process();
            tutorialContent.TutorialProgressBar.fillAmount = FillAmount.Clamped01();
        }

        public void LoadTransitionSettings(float transitionFrom, float transitionTo) {
            transitionFromY = transitionFrom;
            transitionToY = transitionTo;
        }

        public Tweener Hide(float delay) {
            var realDuration = overrideTransitionSettings ? transitionDuration : baseTransitionDuration;
            var realDelay = overrideTransitionSettings ? transitionDelay : delay;
            return rectTransform.DOAnchorPosY(transitionToY, realDuration).From(new Vector2(0.0f, transitionFromY)).SetDelay(realDelay);
        }

        public Tweener Show(float delay) {
            var realDuration = overrideTransitionSettings ? transitionDuration : baseTransitionDuration;
            var realDelay = overrideTransitionSettings ? transitionDelay : delay;
            return rectTransform.DOAnchorPosY(transitionFromY, realDuration).From(new Vector2(0.0f, transitionToY)).SetDelay(realDelay);
        }

        public void ResetTutorial() {
            isFinished = false;
            FillAmount = 0.0f;
            try {
                tutorialContent.TutorialProgressBar.fillAmount = 0.0f;
            } catch (Exception e) {
                Debug.LogError(e);
            }
            OnReset();
        }
    }
}