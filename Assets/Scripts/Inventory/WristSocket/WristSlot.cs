using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class WristSlot : MonoBehaviour
{
    [SerializeField] private InputActionReference _gripAction;
    [SerializeField] private XRDirectInteractor _interactor;

    private bool _hasItem = false;      // 슬롯에 아이템이 있는지
    private bool _canInteract = true;   // 슬롯 충돌 로직 허용 플래그
    private BaseItem _item;             // 슬롯에 든 아이템

    private void OnTriggerEnter(Collider other)
    {
        if (!_canInteract || _hasItem) return;
        if (_gripAction.action.ReadValue<float>() <= 0.5f) return;

        if (other.TryGetComponent<CustomGrabInteractable>(out var grab))
        {
            // 기존 그랩 해제
            foreach (var inter in grab.interactorsSelecting.ToList())
                grab.interactionManager.SelectExit(inter, grab);

            if (other.TryGetComponent<BaseItem>(out var item) && !item.IsInInventory)
            {
                _item = item;
                item.IsInInventory = true;
                SetItem(item);
                grab.enabled = false;
                _hasItem = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_canInteract || !_hasItem) return;
        if (_gripAction.action.ReadValue<float>() <= 0.5f) return;

        if (other.TryGetComponent<XRDirectInteractor>(out var interactor))
        {
            AttachToHand(_item);
            _hasItem = false;
        }
    }

    private void SetItem(BaseItem item)
    {
        item.transform.SetParent(transform);

        StartCoroutine(BlockSlotTemporarily(0.1f));
    }

    private void AttachToHand(BaseItem item)
    {
        // 기존 Detach 로직
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

        // 꺼낸 직후 0.2초간 슬롯 비활성
        StartCoroutine(BlockSlotTemporarily(0.1f));
    }

    private IEnumerator BlockSlotTemporarily(float delay)
    {
        // 1) 슬롯 로직 끄기
        _canInteract = false;
        // 2) delay 동안 대기
        yield return new WaitForSeconds(delay);
        // 3) 다시 슬롯 허용, 그리고 _hasItem=false 처리
        _canInteract = true;
    }
}
