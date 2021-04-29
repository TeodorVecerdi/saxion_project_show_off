using System;
using Runtime;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
public class ItemPickupTest : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Transform cameraTransform;
    
    private Pickup pickupUnderMouse;
    private PlayerInventory playerInventory;

    private void Awake() {
        playerInventory = GetComponent<PlayerInventory>();
    }

    private void Update() {
        if (transform.hasChanged) {
            transform.hasChanged = false;
            DoPickupRaycast();
        }

        if (pickupUnderMouse != null && Input.GetKeyDown(KeyCode.F)) {
            Destroy(pickupUnderMouse.gameObject);
            playerInventory.AddMaterial(pickupUnderMouse.ItemStack);
            pickupUnderMouse = null;
            DoPickupRaycast();
        }
    }

    private void DoPickupRaycast() {
        var ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out var hitInfo, 10, LayerMask.GetMask("Pickup"))) {
            pickupUnderMouse = hitInfo.transform.GetComponent<Pickup>();
            text.text = pickupUnderMouse.ItemStack.Item.ItemName;
        } else {
            pickupUnderMouse = null;
            text.text = "None";
        }
    }
}

