using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SelectBorder : MonoBehaviour
{
    private InputDevice _rightController;

    private InventorySlot[] _slots;
    private InventorySlot _selectedInventorySlot;

    private bool _prevAPressed = false;
    private bool _prevBPressed = false;

    void Start()
    {
        // 오른손 컨트롤러 디바이스 가져오기
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, devices);
        if (devices.Count > 0)
            _rightController = devices[0];

        _slots = InventoryManager.Instance.Slots;

        // 처음 한 번 하이라이트
        UpdateSelection();
    }

    void Update()
    {
        // XR 컨트롤러 입력
        _rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isAPressed);
        _rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isBPressed);

        // 키보드 A/B 
        bool isAKey = Input.GetKeyDown(KeyCode.A);
        bool isBKey = Input.GetKeyDown(KeyCode.B);

        // A: 이전 슬롯
        if ((isAPressed && !_prevAPressed) || isAKey)
        {
            InventoryManager.Instance.SelectedSlotIndex = (InventoryManager.Instance.SelectedSlotIndex - 1 + _slots.Length) % _slots.Length;
            UpdateSelection();
        }

        // B: 다음 슬롯
        if ((isBPressed && !_prevBPressed) || isBKey)
        {
            InventoryManager.Instance.SelectedSlotIndex = (InventoryManager.Instance.SelectedSlotIndex + 1) % _slots.Length;
            UpdateSelection();
        }

        _prevAPressed = isAPressed;
        _prevBPressed = isBPressed;
    }

    private void UpdateSelection()
    {
        _selectedInventorySlot = _slots[InventoryManager.Instance.SelectedSlotIndex];
        HighlightSlot(InventoryManager.Instance.SelectedSlotIndex);
    }

    private void HighlightSlot(int selectedIdx)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            var slot = _slots[i];
            // 슬롯 안에 아이템이 있으면 종류별 색으로
            BaseItem item = slot.GetComponentInChildren<BaseItem>();
            if (item != null)
            {
                slot.SetBorderColor(item);        // 기존 SetBorder로 기본 색상 적용
            }
            else
            {
                slot.BorderImage.color = Color.white; // 빈 슬롯은 흰색(또는 원하시는 색)
            }
        }
        // 마지막으로 선택된 슬롯만 노란색 강조
        _slots[selectedIdx].BorderImage.color = Color.yellow;
    }
}
