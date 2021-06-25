using TMPro;
using UnityEngine;

namespace Runtime {
    public class LeaderboardEntry : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI nameText;

        public void Build(int index, string score, string playerName) {
            scoreText.text = $"{index}. <b>{score}</b>";
            nameText.text = playerName;
        }
    }
}