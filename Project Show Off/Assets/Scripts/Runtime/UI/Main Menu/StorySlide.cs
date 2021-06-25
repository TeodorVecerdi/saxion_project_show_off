using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Runtime {
    public class StorySlide : MonoBehaviour {
        [SerializeField] private List<CanvasGroup> textBoxes;
        [SerializeField] private float timePerTextBox = 3.0f;
        private CanvasGroup self;
        private int currentTextBox;
        private bool started;
        private float timer;
        
        public bool Done { get; private set; }

        private void Awake() {
            self = GetComponent<CanvasGroup>();
            Done = false;
        }

        private void Update() {
            if(!started) return;
            timer += Time.unscaledDeltaTime;
            if (timer >= timePerTextBox) {
                timer = 0.0f;
                if (currentTextBox >= 0) {
                    textBoxes[currentTextBox].DOFade(0.0f, 0.5f);
                }

                currentTextBox++;
                if (currentTextBox < textBoxes.Count) {
                    textBoxes[currentTextBox].DOFade(1.0f, 0.5f);
                } else {
                    started = false;
                    Done = true;
                }
            }
        }

        public void Show(float fadeTime) {
            self.DOFade(1.0f, fadeTime);
            started = true;
            currentTextBox = -1;
        }

        public void Hide(float fadeTime) {
            self.DOFade(0.0f, fadeTime);
        }

    }
}