using System;
using DG.Tweening;
using NaughtyAttributes;
using Runtime.Event;
using TMPro;
using UnityCommons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class SettingsController : MonoBehaviour {
        [SerializeField] private TMP_Dropdown qualityLevelDropdown;
        [SerializeField] private Toggle motionBlurToggle;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider mouseSensitivitySlider;

        [SerializeField] private UnityEvent onEnable;
        [SerializeField] private UnityEvent onDisable;

        private CanvasGroup canvasGroup;

        private int qualityLevel;
        private bool enableMotionBlur;
        private float sfxVolume;
        private float musicVolume;
        private float mouseSensitivity;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start() {
            // Register listeners
            qualityLevelDropdown.onValueChanged.AddListener(OnQualityLevelChanged);
            motionBlurToggle.onValueChanged.AddListener(OnMotionBlurChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);

            // Get settings 
            qualityLevel = PlayerPrefs.GetInt("Settings_QualityLevel", QualitySettings.GetQualityLevel());
            enableMotionBlur = PlayerPrefs.GetInt("Settings_MotionBlur", 1) == 1;
            sfxVolume = PlayerPrefs.GetFloat("Settings_SfxVolume", 1.0f);
            musicVolume = PlayerPrefs.GetFloat("Settings_MusicVolume", 1.0f);
            mouseSensitivity = PlayerPrefs.GetFloat("Settings_MouseSensitivity", 1.0f);

            // Set visually
            qualityLevelDropdown.SetValueWithoutNotify(qualityLevel);
            motionBlurToggle.SetIsOnWithoutNotify(enableMotionBlur);
            sfxVolumeSlider.SetValueWithoutNotify(sfxVolume);
            musicVolumeSlider.SetValueWithoutNotify(musicVolume);
            mouseSensitivitySlider.SetValueWithoutNotify(mouseSensitivity);

            // Set settings
            PlayerPrefs.SetInt("Settings_QualityLevel", qualityLevel);
            PlayerPrefs.SetInt("Settings_MotionBlur", enableMotionBlur ? 1 : 0);
            PlayerPrefs.SetFloat("Settings_SfxVolume", sfxVolume);
            PlayerPrefs.SetFloat("Settings_MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("Settings_MouseSensitivity", mouseSensitivity);
            PlayerPrefs.Save();

            QualitySettings.SetQualityLevel(qualityLevel, true);

            // Send event so settings can be applied 
            EventQueue.QueueEvent(new SettingsChangedEvent(this, enableMotionBlur, sfxVolume, musicVolume, mouseSensitivity));
        }

        private void OnDestroy() {
            qualityLevelDropdown.onValueChanged.RemoveListener(OnQualityLevelChanged);
            motionBlurToggle.onValueChanged.RemoveListener(OnMotionBlurChanged);
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
            musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            mouseSensitivitySlider.onValueChanged.RemoveListener(OnMouseSensitivityChanged);
        }

        public void SetEnabled(bool isEnabled) {
            if (isEnabled) onEnable?.Invoke();
            else onDisable?.Invoke();
        }

        private void OnQualityLevelChanged(int qualityIndex) {
            qualityLevel = qualityIndex;
            QualitySettings.SetQualityLevel(qualityLevel, false);
            PlayerPrefs.SetInt("Settings_QualityLevel", qualityLevel);
            PlayerPrefs.Save();
            EventQueue.QueueEvent(new SettingsChangedEvent(this, enableMotionBlur, sfxVolume, musicVolume, mouseSensitivity));
        }

        private void OnMotionBlurChanged(bool motionBlur) {
            enableMotionBlur = motionBlur;
            PlayerPrefs.SetInt("Settings_MotionBlur", enableMotionBlur ? 1 : 0);
            PlayerPrefs.Save();
            EventQueue.QueueEvent(new SettingsChangedEvent(this, enableMotionBlur, sfxVolume, musicVolume, mouseSensitivity));
            SoundManager.PlaySound("Click");
        }

        private void OnSFXVolumeChanged(float volume) {
            sfxVolume = volume;
            PlayerPrefs.SetFloat("Settings_SFXVolume", sfxVolume);
            PlayerPrefs.Save();
            EventQueue.QueueEvent(new SettingsChangedEvent(this, enableMotionBlur, sfxVolume, musicVolume, mouseSensitivity));
        }

        private void OnMusicVolumeChanged(float volume) {
            musicVolume = volume;
            PlayerPrefs.SetFloat("Settings_MusicVolume", musicVolume);
            PlayerPrefs.Save();
            EventQueue.QueueEvent(new SettingsChangedEvent(this, enableMotionBlur, sfxVolume, musicVolume, mouseSensitivity));
        }

        private void OnMouseSensitivityChanged(float sensitivity) {
            mouseSensitivity = sensitivity;
            PlayerPrefs.SetFloat("Settings_MouseSensitivity", mouseSensitivity);
            PlayerPrefs.Save();
            EventQueue.QueueEvent(new SettingsChangedEvent(this, enableMotionBlur, sfxVolume, musicVolume, mouseSensitivity));
        }

        public void OnResetTutorialsClicked() {
            PlayerPrefs.DeleteKey("current_tutorial");
            EventQueue.QueueEvent(new EmptyEvent(this, EventType.ResetTutorial));
            SoundManager.PlaySound("Click");
        }

        public void OnExitClicked() {
            Time.timeScale = 1.0f;
            SceneManager.LoadSceneAsync(1);
        }
    }
}