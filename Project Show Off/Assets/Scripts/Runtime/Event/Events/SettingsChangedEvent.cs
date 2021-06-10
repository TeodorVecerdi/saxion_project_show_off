namespace Runtime.Event {
    public sealed class SettingsChangedEvent : EventData {
        public override EventType Type => EventType.SettingsChanged;
        
        public bool EnableMotionBlur { get; }
        public float SfxVolume { get; }
        public float MusicVolume { get; }
        public float MouseSensitivity { get; }

        public SettingsChangedEvent(object sender, bool enableMotionBlur, float sfxVolume, float musicVolume, float mouseSensitivity) : base(sender) {
            EnableMotionBlur = enableMotionBlur;
            SfxVolume = sfxVolume;
            MusicVolume = musicVolume;
            MouseSensitivity = mouseSensitivity;
        }
    }
}