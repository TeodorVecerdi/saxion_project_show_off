using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime {
    public class GameWinController : MonoBehaviour {
        [SerializeField] private CanvasGroup canvasGroup1;
        [SerializeField] private CanvasGroup canvasGroup2;
        [SerializeField] private TextMeshProUGUI scoreText1;
        [SerializeField] private TextMeshProUGUI scoreText2;
        [SerializeField] private TMP_InputField nameField;

        private int score;

        private void Start() {
            score = PlayerPrefs.GetInt("Score");
            PlayerPrefs.DeleteKey("Score");
            PlayerPrefs.Save();

            scoreText1.text = $"SCORE: {score}";
            scoreText2.text = $"SCORE: {score}";
        }

        public void OnExitClicked() {
            SceneManager.LoadScene(1);
        }

        public void OnWantsToSaveScoreClicked() {
            canvasGroup1.DOFade(0.0f, 0.25f);
            canvasGroup2.DOFade(1.0f, 0.25f);
        }

        public void OnSaveScoreClicked() {
            if(string.IsNullOrEmpty(nameField.text)) return;

            var thread = new Thread(async () => {
                await AddAWS(nameField.text);
                SceneManager.LoadScene(1);
            });
            thread.Start();
        }

        private async Task AddAWS(string playerName) {
            var guid = Guid.NewGuid().ToString();
            using var httpClient = new HttpClient();
            var uri = new Uri("https://edgjs1jvk1.execute-api.eu-central-1.amazonaws.com/default/add-scoreboard");
            var response = await httpClient.PutAsync(uri, new StringContent($"{{\"score\": \"{score}\", \"name\": \"{playerName}\", \"guid\": \"{guid}\"}}", Encoding.UTF8));
            Debug.Log($"[{response.StatusCode}] {response.ReasonPhrase}");
            Debug.Log(await response.Content.ReadAsStringAsync());
        }
    }
}