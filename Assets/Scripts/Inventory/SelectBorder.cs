using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectBorder : MonoBehaviour
{
    private InventorySlot[] _slots;

    [SerializeField] private InputActionReference _aButton; // 이전 슬롯 (Value/Axis)
    [SerializeField] private InputActionReference _bButton; // 다음 슬롯 (Value/Axis)

    private void OnEnable()
    {
        _aButton.action.performed += OnAPerformed;
        _bButton.action.performed += OnBPerformed;
        _aButton.action.Enable();
        _bButton.action.Enable();
    }

    private void OnDisable()
    {
        _aButton.action.performed -= OnAPerformed;
        _bButton.action.performed -= OnBPerformed;
        _aButton.action.Disable();
        _bButton.action.Disable();
    }

    private void Start()
    {
        _slots = InventoryManager.Instance.Slots;
        UpdateSelection(); // 최초 하이라이트
    }

    private void OnAPerformed(InputAction.CallbackContext ctx)
    {
        MoveSelection(-1);
    }

    private void OnBPerformed(InputAction.CallbackContext ctx)
    {
        MoveSelection(+1);
    }

    private void MoveSelection(int delta)
    {
        int count = _slots.Length;
        int newIndex = (InventoryManager.Instance.SelectedSlotIndex + delta + count) % count;
        InventoryManager.Instance.SelectedSlotIndex = newIndex;
        UpdateSelection();
    }

    private void UpdateSelection()
    {
        int idx = InventoryManager.Instance.SelectedSlotIndex;
        HighlightSlot(idx);
    }

    private void HighlightSlot(int selectedIdx)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            var slot = _slots[i];
            var item = slot.GetComponentInChildren<BaseItem>();
            if (item != null)
                slot.SetBorderColor(item);
            else
                slot.BorderImage.color = Color.white;
        }
        // 선택된 슬롯만 노란색 강조
        _slots[selectedIdx].BorderImage.color = Color.yellow;
    }
}
