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

        private CanvasGroup canvasGroup;
        private int currentStory;
        private bool started;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Begin() {
            gameObject.SetActive(true);
            canvasGroup.DOFade(1.0f, fadeTime).OnComplete(() => {
                currentStory = 0;
                started = true;
                stories[currentStory].Show(fadeTime); 
            });
        }

        private void Update() {
            if (!started) return;

            if (stories[currentStory].Done) {
                stories[currentStory].Hide(fadeTime);
                currentStory++;
                if (currentStory < stories.Count) {
                    stories[currentStory].Show(fadeTime);
                } else {
                    skipButton.DOFade(0.0f, fadeTime);
                    started = false;
                    Destroy(eventSystemGameObject);
                    StartCoroutine(SwitchSceneAfter(1.0f));
                    return;
                }
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