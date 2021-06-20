using UnityEngine;

namespace Runtime {
    public sealed class PauseGame : MonoBehaviour {
        public void SetPaused(bool isPaused) {
            Time.timeScale = isPaused ? 0.0f : 1.0f;
        }
    }
}