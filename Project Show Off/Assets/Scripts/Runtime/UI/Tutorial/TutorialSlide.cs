using System;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityCommons;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Tutorial {
    public abstract class TutorialSlide : MonoBehaviour {
        [HorizontalLine(color: EColor.Green, order = -10000), Header("References", order = -20000)]
        [SerializeField, Required("A reference to the 'Icon' Image is required.")] 
        private Image tutorialIcon;
        [SerializeField, Required("A reference to the 'Progress Bar' Image is required.")] 
        private Image tutorialProgressBar;
        [SerializeField, Required("A reference to the 'Line 1' Text is required.")] 
        private TextMeshProUGUI tutorialLine1;
        [SerializeField, Required("A reference to the 'Line 2' Text is required.")] 
        private TextMeshProUGUI tutorialLine2;

        [HorizontalLine(color: EColor.Blue, order = -10000), Header("Tutorial Settings", order = -20000)]
        [SerializeField, HideIf("debug_tutorialLine1Null"), OnValueChanged("OnTutorialLinesUpdated")] 
        private string line1Text;
        [SerializeField, HideIf("debug_tutorialLine2Null"), OnValueChanged("OnTutorialLinesUpdated")] 
        private string line2Text;
        [SerializeField, HideIf("debug_tutorialIconNull"), OnValueChanged("OnTutorialIconUpdated")] 
        private Sprite tutorialSprite;
        [Space]
        [SerializeField] private TutorialSlide nextTutorial;
        [SerializeField] protected bool overrideTransitionSettings;
        [SerializeField, ShowIf("overrideTransitionSettings")] private float transitionDelay;
        [SerializeField, ShowIf("overrideTransitionSettings")] private float transitionDuration;
        
#if  UNITY_EDITOR //debug: begin !! Naughty Attributes
        // ReSharper disable InconsistentNaming UnusedMember.Local
        //debug: !! used by NaughtyAttributes to draw the inspector
        private bool debug_tutorialLine1Null => tutorialLine1 == null;
        private bool debug_tutorialLine2Null => tutorialLine2 == null;
        private bool debug_tutorialIconNull => tutorialIcon == null;
        
        // ReSharper restore InconsistentNaming UnusedMember.Local

        protected void OnTutorialLinesUpdated() {
            if (tutorialLine1 != null) tutorialLine1.text = line1Text;
            if (tutorialLine2 != null) tutorialLine2.text = line2Text;
        }

        protected void OnTutorialIconUpdated() {
            if (tutorialIcon != null) tutorialIcon.sprite = tutorialSprite;
        }
#endif //debug: end !! Naughty Attributes

        private const float baseTransitionDuration = 0.5f;
        private RectTransform rectTransform;
        private float transitionFromX;
        private float transitionToX;
        private bool isFinished;

        protected float FillAmount;
        public abstract string TutorialKey { get; }
        protected abstract void Process();
        
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
            transitionFromX = -8.0f;
            transitionToX = rectTransform.sizeDelta.x + 8.0f;
            
            OnAwake();
        }

        private void Update() {
            if (isFinished) return;
            
            Process();
            tutorialProgressBar.fillAmount = FillAmount.Clamped01();
        }

        public void LoadTransitionSettings(float transitionFrom, float transitionTo) {
            transitionFromX = transitionFrom;
            transitionToX = transitionTo;
        }

        public Tweener Hide(float delay) {
            var realDuration = overrideTransitionSettings ? transitionDuration : baseTransitionDuration;
            var realDelay = overrideTransitionSettings ? transitionDelay : delay;
            return rectTransform.DOAnchorPosX(transitionToX, realDuration).From(new Vector2(transitionFromX, -8.0f)).SetDelay(realDelay);
        }

        public Tweener Show(float delay) {
            var realDuration = overrideTransitionSettings ? transitionDuration : baseTransitionDuration;
            var realDelay = overrideTransitionSettings ? transitionDelay : delay;
            return rectTransform.DOAnchorPosX(transitionFromX, realDuration).From(new Vector2(transitionToX, -8.0f)).SetDelay(realDelay);
        }
    }
}