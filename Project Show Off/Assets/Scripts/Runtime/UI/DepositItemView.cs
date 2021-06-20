using Runtime.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime {
    public sealed class DepositItemView : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI countText;

        public void Build(ItemStack itemStack) {
            icon.sprite = itemStack.TrashMaterial.Icon;
            UpdateItemCount(itemStack.Mass);
        }

        public void UpdateItemCount(float mass) {
            countText.text = $"{mass:F1}";
        }
    }
}