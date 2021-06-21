using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusControlFMOD : MonoBehaviour
{
    private FMOD.Studio.EventInstance instance;
    private FMOD.Studio.Bus bus;

    [SerializeField]
    private string busPath;


    [SerializeField]
    [Range(-80f, 10f)]
    private float busVolume;
    private float volume;

    void Start()
    {
        bus = FMODUnity.RuntimeManager.GetBus(busPath);
    }
    void Update()
    {
        volume = Mathf.Pow(10.0f, busVolume / 20f);
        bus.setVolume(volume);
    }

}
