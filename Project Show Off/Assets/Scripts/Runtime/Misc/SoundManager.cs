using System;
using System.Collections.Generic;
using UnityCommons;
using UnityEngine;

namespace Runtime {
    public class SoundManager : MonoSingleton<SoundManager> {
        [SerializeField] private List<AudioKeyValuePair> soundList;
        private float sfxVolume = 1f;
        private Dictionary<string, AudioSource> soundDictionary;

        private Dictionary<string, AudioSource> Sounds {
            get {
                if (soundDictionary == null)
                    LoadSoundDictionary();
                return soundDictionary;
            }
        }

        public static float SfxVolume {
            get => Instance.sfxVolume;
            set {
                Instance.sfxVolume = value;
                foreach (var sound in Instance.Sounds) sound.Value.volume = Instance.sfxVolume;

                PlayerPrefs.SetFloat("SfxVolume", Instance.sfxVolume);
                PlayerPrefs.Save();
            }
        }

        private void LoadSoundDictionary() {
            soundDictionary = new Dictionary<string, AudioSource>();
            foreach (var sound in soundList) {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = sound.Sound;
                audioSource.loop = sound.Loop;
                soundDictionary[sound.Key] = audioSource;
            }
        }

        protected override void OnAwake() {
            SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.75f);
        }

        public static void PlaySound(string soundKey, bool stopIfPlaying = false, bool skipIfAlreadyPlaying = false) {
            if (Instance.Sounds[soundKey] == null) return;

            if (stopIfPlaying)
                Instance.Sounds[soundKey].Stop();

            if (skipIfAlreadyPlaying && Instance.Sounds[soundKey].isPlaying)
                return;

            if (Instance.Sounds[soundKey].loop)
                Instance.Sounds[soundKey].Play();
            else
                Instance.Sounds[soundKey].PlayOneShot(Instance.Sounds[soundKey].clip);
        }

        public static void StopSound(string soundKey) {
            if (Instance.Sounds[soundKey] == null) return;
            Instance.Sounds[soundKey].Stop();
        }

        public static bool IsPlaying(string soundKey) {
            return Instance.Sounds[soundKey] != null && Instance.Sounds[soundKey].isPlaying;
        }

        [Serializable]
        private class AudioKeyValuePair {
            [SerializeField] internal string Key;
            [SerializeField] internal AudioClip Sound;
            [SerializeField] internal bool Loop;
        }
    }
}