using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BusControlFMOD : MonoBehaviour
{
    private FMOD.Studio.EventInstance instance;
    private FMOD.Studio.Bus bus;

    [SerializeField]
    private string busPath;


    [SerializeField, Range(0f,1f)]
    private float volume = 1.0f;

    private void Awake() {
        bus = FMODUnity.RuntimeManager.GetBus(busPath);
        bus.setVolume(volume);
    }

    public void SetVolume(float volume) {
        bus.setVolume(volume);
    }

}
