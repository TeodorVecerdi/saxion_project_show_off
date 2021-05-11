using UnityEngine;

namespace Runtime {
    public static class InputManager {
        public static DefaultInputActions Actions { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() {
            Actions = new DefaultInputActions();
            Actions.Enable();
        }
    }
}