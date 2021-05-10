using System;
using System.Collections.Generic;
using TMPro;
using UnityCommons;
using UnityEngine;

namespace Runtime {
    public class UIHintController : MonoSingleton<UIHintController> {
        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private float hintDuration;
        [SerializeField] private bool debug;

        private List<string> hints;
        private int currentIndex;
        private float hintTimer;

        protected override void OnAwake() {
            hints = new List<string>();
        }

        private void Update() {
            hintTimer += Time.deltaTime;
            if (hintTimer >= hintDuration) {
                hintTimer -= hintDuration;
                if (hints.Count != 0) {
                    currentIndex = (currentIndex + 1) % hints.Count;
                } else currentIndex = -1;
                UpdateHint();
            }
        }

        public void Add(string hint) {
            if(hints.Contains(hint)) return;
            hints.Add(hint);
        }

        public void Remove(string hint) {
            hints.RemoveAll(s => string.Equals(s, hint, StringComparison.InvariantCulture));
            UpdateHintIndex();
        }

        private void UpdateHintIndex() {
            if (currentIndex >= hints.Count) currentIndex = hints.Count - 1;
            UpdateHint();
        }

        private void UpdateHint() {
            var currentHint = currentIndex < 0 ? "" : hints[currentIndex];
            if (debug) currentHint = $"Hint count : {hints.Count}; Hint : [{currentHint}]";
            hintText.text = currentHint;
        }
    }
}