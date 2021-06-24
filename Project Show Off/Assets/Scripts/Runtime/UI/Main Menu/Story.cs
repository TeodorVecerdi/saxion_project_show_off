using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime {
    public class Story : MonoBehaviour {
        [SerializeField] private List<StorySlide> stories;
        [SerializeField] private CanvasGroup skipButton;
        [SerializeField] private GameObject eventSystemGameObject;
        [SerializeField] private GameObject mainLightObject;
        [Space]
        [SerializeField] private float fadeTime = 0.75f;
        [SerializeField] private float storyTime = 8.0f;
        [SerializeField] private float textBoxTime = 4.0f;

        private CanvasGroup canvasGroup;
        private int currentStory;
        private bool started;
        private bool showedTextBox;
        private float storyTimer;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Begin() {
            gameObject.SetActive(true);
            canvasGroup.DOFade(1.0f, fadeTime).OnComplete(() => {
                currentStory = 0;
                started = true;
                storyTimer = 0.0f;
                stories[currentStory].Show(fadeTime); 
            });
        }

        private void Update() {
            if (!started) return;

            storyTimer += Time.unscaledDeltaTime;
            if (storyTimer >= textBoxTime && !showedTextBox) {
                showedTextBox = true;
                stories[currentStory].ShowTextBox(fadeTime);
            }

            if (storyTimer >= storyTime) {
                storyTimer = 0.0f;
                stories[currentStory].Hide(fadeTime);
                showedTextBox = false;
                currentStory++;

                if (currentStory >= stories.Count) {
                    skipButton.DOFade(0.0f, 0.5f);
                    started = false;
                    Destroy(eventSystemGameObject);
                    StartCoroutine(SwitchSceneAfter(1.0f));
                    return;
                }

                stories[currentStory].Show(fadeTime);
            }
        }

        public void OnSkipClicked() {
            skipButton.DOFade(0.0f, 0.5f);
            stories[currentStory].Hide(fadeTime);
            started = false;
            Destroy(eventSystemGameObject);
            StartCoroutine(SwitchSceneAfter(1.0f));
        }

        private IEnumerator SwitchSceneAfter(float delay) {
            Destroy(eventSystemGameObject);
            Debug.Log($"Switching scene after {delay} seconds");
            yield return new WaitForSeconds(delay);
            var operation = SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
            yield return operation;
            DestroyImmediate(mainLightObject);
            if (!operation.isDone) Debug.Log("Loading not done");

            var unloadOperation = SceneManager.UnloadSceneAsync(1);
            yield return unloadOperation;
        }
    }
}