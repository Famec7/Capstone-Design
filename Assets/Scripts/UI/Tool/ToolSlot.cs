using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ToolSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private InputActionReference _gripAction1;
    [SerializeField]
    private InputActionReference _gripAction2;
    [SerializeField]
    private ItemData _data;

    public ToolUI ToolUI;
    private bool _isHovered = false;

    public bool IsSelect;

    public GameObject Tool;

    private void Awake()
    {
        ToolUI = transform.parent.parent.GetComponent<ToolUI>();
    }

    private void OnEnable()
    {
        _gripAction1.action.performed += OnGripPerformed;
        _gripAction1.action.Enable();

        _gripAction2.action.performed += OnGripPerformed;
        _gripAction2.action.Enable();
    }

    private void OnDisable()
    {
        _gripAction2.action.performed -= OnGripPerformed;
        _gripAction2.action.Disable();
    }

    private void OnGripPerformed(InputAction.CallbackContext ctx)
    {
        if (_isHovered && !IsSelect)
        {
            Tool.SetActive(true);
            AttachToHand(Tool.GetComponent<BaseItem>());
            Tool.transform.position = transform.position;
            IsSelect = true;
            ToolUI.DisableSlot(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovered = true;

        if (!IsSelect)
        {
            GetComponent<Image>().color = Color.yellow;
            GetComponent<RectTransform>().localScale = Vector3.one * 1.2f;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;

        if (!IsSelect)
        {
            GetComponent<Image>().color = Color.black;
            GetComponent<RectTransform>().localScale = Vector3.one;
        }
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
    }
}
