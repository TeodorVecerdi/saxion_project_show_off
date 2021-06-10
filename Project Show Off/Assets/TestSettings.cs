using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityCommons;
using UnityEngine;

public class TestSettings : MonoBehaviour {
    [Range(0, 2)] public int QualityLevel;
    [Button]
    public void PrintQualitySettings() {
        var names = QualitySettings.names;
        Debug.Log(names.ToListString());
    }

    [Button]
    public void SetQuality() {
        QualitySettings.SetQualityLevel(QualityLevel, true);
    }
}