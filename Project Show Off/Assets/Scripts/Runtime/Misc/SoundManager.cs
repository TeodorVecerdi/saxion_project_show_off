using System;
using System.Collections.Generic;
using Runtime.Data;
using Runtime.Event;
using UnityCommons;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class SoundManager : MonoSingleton<SoundManager>, IEventSubscriber {
        
        private float sfxVolume = 1f;
        private float musicVolume = 1f;
        private Dictionary<string, (AudioSource audioSource, SoundSettings.AudioKeyValuePair settings)> sounds;
        private IDisposable settingsChangedEventUnsubscribeToken;

        public static float SfxVolume {
            get => Instance.sfxVolume;
            set {
                Instance.sfxVolume = value;
                foreach (var sound in Instance.sounds) {
                    if(sound.Value.settings.IsMusic) continue; 
                    sound.Value.audioSource.volume = Instance.sfxVolume;
                }
            }
        }
        
        public static float MusicVolume {
            get => Instance.musicVolume;
            set {
                Instance.musicVolume = value;
                foreach (var sound in Instance.sounds) {
                    if(!sound.Value.settings.IsMusic) continue; 
                    sound.Value.audioSource.volume = Instance.sfxVolume;
                }
            }
        }

        protected override void OnAwake() {
            LoadSoundDictionary();
            settingsChangedEventUnsubscribeToken = this.Subscribe(EventType.SettingsChanged);
            
            SfxVolume = PlayerPrefs.GetFloat("Settings_SfxVolume", 1.0f);
            MusicVolume = PlayerPrefs.GetFloat("Settings_MusicVolume", 1.0f);
        }

        private void OnDestroy() {
            settingsChangedEventUnsubscribeToken?.Dispose();
        }

        private void LoadSoundDictionary() {
            sounds = new Dictionary<string, (AudioSource audioSource, SoundSettings.AudioKeyValuePair settings)>();
            foreach (var sound in ResourcesProvider.SoundSettings.Sounds) {
                var audioSource = gameObject.AddComponent<AudioSource>();
                if (!sound.HasVariations)
                    audioSource.clip = sound.Sound;
                sounds[sound.Key] = (audioSource, sound);
            }
        }

        public static void PlaySound(string soundKey, bool stopIfPlaying = false, bool skipIfAlreadyPlaying = false) {
            if (!Instance.sounds.ContainsKey(soundKey)) return;

            var (audioSource, settings) = Instance.sounds[soundKey];

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
            if (!Instance.sounds.ContainsKey(soundKey)) return;
            Instance.sounds[soundKey].audioSource.Stop();
        }

        public static bool IsPlaying(string soundKey) {
            return Instance.sounds.ContainsKey(soundKey) && Instance.sounds[soundKey].audioSource.isPlaying;
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case SettingsChangedEvent settingsChangedEvent: {
                    SfxVolume = settingsChangedEvent.SfxVolume;
                    MusicVolume = settingsChangedEvent.MusicVolume;
                    return false;
                }
                default: return false;
            }
        }
    }
}