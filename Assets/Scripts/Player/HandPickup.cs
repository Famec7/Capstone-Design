using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class HandPickup : MonoBehaviour
{
    [SerializeField] private Transform _handTransform;
    [SerializeField] private InputActionReference _gripAction;
    [SerializeField] private XRDirectInteractor _interactor;

    private bool _prevGrip = false;
    private bool _isInZone = false;

    void OnEnable() => _gripAction.action.Enable();
    void OnDisable() => _gripAction.action.Disable();

    void Update()
    {
        bool grip = _gripAction.action.ReadValue<float>() > 0.8f;

        if (!_prevGrip && grip && _isInZone && InventoryInteractionLock.CanInteract)
        {
            var item = InventoryManager.Instance
                         .RemoveItem(InventoryManager.Instance.SelectedSlotIndex);
            if (item != null)
            {
                AttachToHand(item);
                InventoryInteractionLock.Trigger();
            }
        }

        _prevGrip = grip;
    }

    private void AttachToHand(BaseItem item)
    {
        var grab = item.GetComponent<CustomGrabInteractable>();
        if (grab == null) return;

        item.IsInInventory = false;
        item.transform.SetParent(null, true);
        item.InitTransform();

        var rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
        }

        grab.enabled = true;
        grab.interactionManager.SelectEnter(_interactor, grab);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BackpackZone"))
            _isInZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BackpackZone"))
            _isInZone = false;
    }
}
