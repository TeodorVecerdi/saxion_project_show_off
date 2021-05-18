using System;
using Runtime.Data;
using Runtime.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class InventoryItemView : MonoBehaviour {
        [Header("References"), SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI countText;

        public void Build(ItemStack itemStack) {
            icon.sprite = itemStack.TrashCategory.CategorySprite;
            UpdateItemCount(itemStack.Mass);
        }

        public void UpdateItemCount(float mass) {
            countText.text = $"{mass} MU";
        }
    }
}