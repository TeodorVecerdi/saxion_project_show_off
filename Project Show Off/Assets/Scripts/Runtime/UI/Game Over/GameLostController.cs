using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime {
    public class GameLostController : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI scoreText;
        private void Start() {
            scoreText.text = $"SCORE: {PlayerPrefs.GetInt("Score")}";
            PlayerPrefs.DeleteKey("Score");
            PlayerPrefs.Save();
        }

        public void OnExitClicked() {
            SceneManager.LoadScene(1);
        }
    }
}