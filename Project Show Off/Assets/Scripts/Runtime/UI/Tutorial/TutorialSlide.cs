using System;
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
        
#if  UNITY_EDITOR //debug: !! Naughty Attributes
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
#endif
        
        protected float FillAmount;
        protected abstract string TutorialKey { get; }
        protected abstract void Process();

        private void Update() {
            Process();
            tutorialProgressBar.fillAmount = FillAmount.Clamped01();
        }
    }
}