using UnityEngine;

namespace Runtime {
    public class DestroyOnAwake : MonoBehaviour {
        private void Awake() {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}