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
            icon.sprite = itemStack.Item.ItemSprite;
            UpdateItemCount(itemStack.Count);
        }

        public void UpdateItemCount(int itemCount) {
            countText.text = $"{itemCount}";
        }
    }
}