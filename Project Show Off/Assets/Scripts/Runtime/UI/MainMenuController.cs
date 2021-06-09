using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Runtime {
    public class MainMenuController : MonoBehaviour {
        private static readonly int speedPropertyID = Animator.StringToHash("Speed");
        
        [SerializeField] private Image fadeImage;
        [SerializeField] private GameObject eventSystemGameObject;
        [SerializeField] private AnimationClip clip;
        
        private Animator animator;

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        public void OnPlayClicked() {
            animator.SetFloat(speedPropertyID, 1.0f);

            // prevents buttons from being clicked
            fadeImage.raycastTarget = true;

            Destroy(eventSystemGameObject);
            StartCoroutine(SwitchSceneAfter(clip.length));
        }

        public void OnSettingsClicked() {
            Debug.Log("Settings clicked");
        }

        public void OnExitClicked() {
            Application.Quit();
        }

        private IEnumerator SwitchSceneAfter(float delay) {
            Debug.Log($"Switching scene after {delay} seconds");
            yield return new WaitForSeconds(delay);
            var operation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            yield return operation;
            if (!operation.isDone) Debug.Log("Loading not done");

            var unloadOperation = SceneManager.UnloadSceneAsync(0);
            yield return unloadOperation;
        }
    }
}