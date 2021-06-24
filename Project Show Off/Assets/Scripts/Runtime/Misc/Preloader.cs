using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Runtime.Preloader {
    public class Preloader : MonoBehaviour {
        [SerializeField] private Image fillImage;
        [SerializeField] private CanvasGroup logos;
        [SerializeField] private TextMeshProUGUI loadingText;

        [SerializeField] private GameObject eventSystem;
        [SerializeField] private AudioListener audioListener;
        
        private void Start() {
            StartCoroutine(LoadMainMenu());
        }

        private IEnumerator LoadMainMenu() {
            var operation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            
            Destroy(eventSystem);
            Destroy(audioListener);
            
            operation.completed += OnSceneLoad;
            while (!operation.isDone) {
                fillImage.fillAmount = operation.progress;
                yield return null;
            }
        }

        private void OnSceneLoad(AsyncOperation operation) {
            fillImage.DOFade(0.0f, 0.5f);
            logos.DOFade(0.0f, 0.5f);
            loadingText.DOFade(0.0f, 0.5f).OnComplete(() => {
                SceneManager.UnloadSceneAsync(0);
            });
        }
    }
}