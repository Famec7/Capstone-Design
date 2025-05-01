using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ToolSlot : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private InputActionReference _gripAction1;
    [SerializeField]
    private InputActionReference _gripAction2;
    [SerializeField]
    private ItemData _data;

    private ToolUI _toolUI;
    private bool _isHovered = false;

    public bool IsSelect;

    private void Awake()
    {
        _toolUI= transform.parent.parent.GetComponent<ToolUI>();
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
        if (_isHovered)
        {
            BaseItem item = ItemFactory.Instance.CreateItem(_data.ItemName);
            item.gameObject.transform.position = transform.position;
            IsSelect = true;
            _toolUI.DisableSlot(this);
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

}
