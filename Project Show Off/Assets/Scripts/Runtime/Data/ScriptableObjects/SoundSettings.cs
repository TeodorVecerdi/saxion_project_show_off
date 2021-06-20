using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewSoundSettings", menuName = "Data/Sound Settings", order = 0)]
    public sealed class SoundSettings : ScriptableObject {
        [SerializeField] private List<AudioKeyValuePair> sounds;

        public List<AudioKeyValuePair> Sounds => sounds;

        [Serializable]
        public sealed class AudioKeyValuePair {
            public string Key;
            public bool IsMusic;
            public bool HasVariations;
            public AudioClip Sound;
            public List<AudioClip> Variations;
            public bool HasPitchVariation;
            public Vector2 PitchVariationMinMax;
        }
    }
}