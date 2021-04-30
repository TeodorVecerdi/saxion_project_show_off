using Runtime.Event;
using TMPro;
using UnityEngine;

namespace Runtime {
    public class ItemPickupTest : MonoBehaviour {
        [SerializeField] private EventQueue eventQueue;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Transform cameraTransform;
        private Pickup pickupUnderMouse;

        private void Update() {
            if (transform.hasChanged) {
                transform.hasChanged = false;
                DoPickupRaycast();
            }

            if (pickupUnderMouse != null && Input.GetKeyDown(KeyCode.F)) {
                Destroy(pickupUnderMouse.gameObject);
                eventQueue.QueueEvent(new MaterialPickedUpEvent(this, pickupUnderMouse.ItemStack));
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
}

