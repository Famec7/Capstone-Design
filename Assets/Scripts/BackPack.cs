using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InventoryInteractionLock
{
    private static float _lastTime = -Mathf.Infinity;
    public static float Cooldown = 0.2f;

    public static bool CanInteract =>
        Time.time - _lastTime >= Cooldown;

    public static void Trigger() =>
        _lastTime = Time.time;
}



public class BackPack : MonoBehaviour
{
    [SerializeField]
    private InputActionReference _gripAction1;

    [SerializeField]
    private InputActionReference _gripAction2;
    private void OnTriggerEnter(Collider other)
    {
        if (!InventoryInteractionLock.CanInteract || !InventoryManager.Instance.HasEmptySlot()) return;
  
        bool grip = _gripAction1.action.ReadValue<float>() > 0.8f
                  || _gripAction2.action.ReadValue<float>() > 0.8f;
        if (!grip) return;

        if (other.TryGetComponent<CustomGrabInteractable>(out var grab))
        {
            // 잡고 있던 인터랙터 해제
            foreach (var inter in grab.interactorsSelecting.ToList())
                grab.interactionManager.SelectExit(inter, grab);

            if (other.TryGetComponent<BaseItem>(out var item) && !item.IsInInventory)
            {
                InventoryManager.Instance.AddItem(item);
                item.IsInInventory = true;
                grab.enabled = false;


                InventoryInteractionLock.Trigger();
            }
        }
    }
}
