using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewSoundSettings", menuName = "Data/Sound Settings", order = 0)]
    public class SoundSettings : ScriptableObject {
        [FormerlySerializedAs("soundList")] [SerializeField] private List<AudioKeyValuePair> sounds;

        public List<AudioKeyValuePair> Sounds => sounds;

        [Serializable]
        public class AudioKeyValuePair {
            public string Key;
            public bool HasVariations;
            public AudioClip Sound;
            public List<AudioClip> Variations;
        }
    }
}