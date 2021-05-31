using UnityEngine;

namespace Runtime {
    [ExecuteInEditMode]
    public class CloudGradient : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private Gradient lutGradient;
        [SerializeField] private Vector2Int lutTextureSize;
        [SerializeField] private Texture2D lutTexture;
        [SerializeField] private bool generateRealtime;
        [Header("References")]
        [SerializeField] private Material cloudMaterial;
        [SerializeField] private string textureProperty;

        private void Update() {
            if(!generateRealtime) return;
            GenerateLutTexture();
        }

        private void GenerateLutTexture() {
            lutTexture = new Texture2D(lutTextureSize.x, lutTextureSize.y) {wrapMode = TextureWrapMode.Clamp};
            for (var x = 0; x < lutTextureSize.x; x++) {
                var color = lutGradient.Evaluate(x / (float) lutTextureSize.x);
                for (var y = 0; y < lutTextureSize.y; y++) {
                    lutTexture.SetPixel(x, y, color);    
                }
            }

            lutTexture.Apply();
            cloudMaterial.SetTexture(textureProperty, lutTexture);
        }
    }
}