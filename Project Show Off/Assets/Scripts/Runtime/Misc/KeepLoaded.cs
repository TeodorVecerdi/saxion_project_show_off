using System;
using UnityEngine;

namespace Runtime {
    public sealed class KeepLoaded : MonoBehaviour {
        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }
    }
}