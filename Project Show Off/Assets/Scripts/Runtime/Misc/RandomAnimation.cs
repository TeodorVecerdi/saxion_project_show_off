using UnityCommons;
using UnityEngine;

namespace Runtime {
    [RequireComponent(typeof(Animator))]
    public class RandomAnimation : MonoBehaviour {
        private void Start() {
            var anim = GetComponent<Animator>();
            anim.Play(anim.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, Rand.Float);
        }
    }
}