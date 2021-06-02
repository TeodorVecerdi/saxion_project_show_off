using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Runtime.Data;
using UnityCommons;
using UnityEngine;

namespace Runtime {
    public class SoundManager : MonoSingleton<SoundManager> {
        [SerializeField] private SoundSettings soundSettings;
        
        private float sfxVolume = 1f;
        private Dictionary<string, (AudioSource audioSource, SoundSettings.AudioKeyValuePair sound)> soundDictionary;

        private Dictionary<string, (AudioSource audioSource, SoundSettings.AudioKeyValuePair settings)> Sounds {
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
                foreach (var sound in Instance.Sounds) sound.Value.audioSource.volume = Instance.sfxVolume;

                PlayerPrefs.SetFloat("SfxVolume", Instance.sfxVolume);
                PlayerPrefs.Save();
            }
        }

        protected override void OnAwake() {
            LoadSoundDictionary();
            DontDestroyOnLoad(gameObject);
            SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.75f);
        }

        private void LoadSoundDictionary() {
            soundDictionary = new Dictionary<string, (AudioSource audioSource, SoundSettings.AudioKeyValuePair settings)>();
            foreach (var sound in soundSettings.Sounds) {
                var audioSource = gameObject.AddComponent<AudioSource>();
                if (!sound.HasVariations)
                    audioSource.clip = sound.Sound;
                soundDictionary[sound.Key] = (audioSource, sound);
            }
        }

        public static void PlaySound(string soundKey, bool stopIfPlaying = false, bool skipIfAlreadyPlaying = false) {
            if (!Instance.Sounds.ContainsKey(soundKey)) return;

            var (audioSource, settings) = Instance.Sounds[soundKey];

            if (stopIfPlaying)
                audioSource.Stop();

            if (skipIfAlreadyPlaying && audioSource.isPlaying)
                return;

            if (settings.HasVariations)
                audioSource.clip = Rand.ListItem(settings.Variations);

            if (settings.HasPitchVariation) {
                audioSource.pitch = 1 + Rand.Range(settings.PitchVariationMinMax.x, settings.PitchVariationMinMax.y);
            }

            audioSource.PlayOneShot(audioSource.clip);
        }

        public static void StopSound(string soundKey) {
            if (!Instance.Sounds.ContainsKey(soundKey)) return;
            Instance.Sounds[soundKey].audioSource.Stop();
        }

        public static bool IsPlaying(string soundKey) {
            return Instance.Sounds.ContainsKey(soundKey) && Instance.Sounds[soundKey].audioSource.isPlaying;
        }
    }
}