using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Runtime {
    [RequireComponent(typeof(Animation))]
    public sealed class MainMenuController : MonoBehaviour {
        [Header("References")]
        [SerializeField] private CanvasGroup settingsContainer;
        [SerializeField] private CanvasGroup creditsContainer;
        [SerializeField] private CanvasGroup buttonContainer;
        
        [Header("Scene Transition References")]
        [SerializeField] private Image fadeImage;
        [SerializeField] private GameObject eventSystemGameObject;
        [SerializeField] private GameObject mainLightObject;

        private SettingsController settingsController;
        private Animation animation;
        private bool isSettingsMenuOpen;
        private bool isCreditsMenuOpen;

        private void Awake() {
            animation = GetComponent<Animation>();
            settingsController = settingsContainer.GetComponent<SettingsController>();
        }

        private void OnEnable() {
            InputManager.UIActions.Cancel.performed += OnCloseSettingsMenu;
        }

        private void OnDisable() {
            InputManager.UIActions.Cancel.performed -= OnCloseSettingsMenu;
        }

        private void OnCloseSettingsMenu(InputAction.CallbackContext obj) {
            if(!isSettingsMenuOpen) return;
            ShowSettings(false);
        }

        public void OnPlayClicked() {
            // prevents buttons from being clicked
            fadeImage.raycastTarget = true;

            Destroy(eventSystemGameObject);
            
            animation.Play();
            StartCoroutine(SwitchSceneAfter(animation.clip.length));
            SoundManager.PlaySound("Click");
        }

        public void OnSettingsClicked(bool showSettings) {
            ShowSettings(showSettings);
            SoundManager.PlaySound("Click");
        }

        private void ShowSettings(bool showSettings) {
            isSettingsMenuOpen = showSettings;
            settingsController.SetEnabled(showSettings);
            if (!showSettings) buttonContainer.gameObject.SetActive(true);
            else settingsContainer.gameObject.SetActive(true);

            buttonContainer.DOFade(showSettings ? 0.0f : 1.0f, 0.25f).OnComplete(() => {
                if (showSettings) buttonContainer.gameObject.SetActive(false);
            });
            settingsContainer.DOFade(showSettings ? 1.0f : 0.0f, 0.25f).OnComplete(() => {
                if (!showSettings) settingsContainer.gameObject.SetActive(false);
            });
        }

        public void OnCreditsClicked(bool showCredits) {
            ShowCredits(showCredits);
            SoundManager.PlaySound("Click");
        }
        
        private void ShowCredits(bool showCredits) {
            isCreditsMenuOpen = showCredits;
            if (!showCredits) buttonContainer.gameObject.SetActive(true);
            else creditsContainer.gameObject.SetActive(true);

            creditsContainer.DOFade(showCredits ? 0.0f : 1.0f, 0.25f).OnComplete(() => {
                if (showCredits) buttonContainer.gameObject.SetActive(false);
            });
            creditsContainer.DOFade(showCredits ? 1.0f : 0.0f, 0.25f).OnComplete(() => {
                if (!showCredits) creditsContainer.gameObject.SetActive(false);
            });
        }

        public void OnExitClicked() {
            Application.Quit();
        }

        private IEnumerator SwitchSceneAfter(float delay) {
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