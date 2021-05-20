using UnityEngine;

namespace Runtime {
    public class PlaceableObject : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private string objectName;
        [SerializeField] private Sprite objectSprite;
    }
}