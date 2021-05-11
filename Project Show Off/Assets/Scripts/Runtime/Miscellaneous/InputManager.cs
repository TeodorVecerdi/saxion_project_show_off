using UnityCommons;

namespace Runtime {
    public class InputManager : MonoSingleton<InputManager> {
        public static DefaultInputActions Actions => Instance.inputActions; 
        
        private DefaultInputActions inputActions;
        protected override void OnAwake() {
            inputActions = new DefaultInputActions();
        }
    }
}