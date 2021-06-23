using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime {
    public class FadeOutOnAwake : MonoBehaviour {
        [SerializeField] private Graphic target;
        [SerializeField] private float duration = 0.5f;

        private void Awake() {
            target.DOFade(0.0f, duration).SetUpdate(true);
        }
    }
}