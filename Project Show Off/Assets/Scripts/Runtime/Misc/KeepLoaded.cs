using System;
using UnityEngine;

namespace Runtime {
    public class KeepLoaded : MonoBehaviour {
        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }
    }
}