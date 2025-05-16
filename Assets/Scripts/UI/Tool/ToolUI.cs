using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ToolUI : MonoBehaviour
{

    private ToolSlot[] slots;

    [SerializeField]
    private Vector3 _offset = new Vector3(0f, 1.2f, 1.3f);

    [SerializeField] private InputActionReference _yButton; // 이전 슬롯 (Value/Axis)
    private GameObject _childObject;
    void Start()
    {
        // 바로 밑에 자식(인덱스 0)을 캐싱
        if (transform.childCount > 0)
            _childObject = transform.GetChild(0).gameObject;
        else
            Debug.LogWarning($"{name}에 자식 오브젝트가 없습니다.");

        slots = GetComponentsInChildren<ToolSlot>(); 
    }

    void OnEnable()
    {
        _yButton.action.performed += OnYPerformed;
        _yButton.action.Enable();
    }

    void OnDisable()
    {
        _yButton.action.performed -= OnYPerformed;
        _yButton.action.Disable();
    }


    private void OnYPerformed(InputAction.CallbackContext ctx)
    {
        if (_childObject != null)
            _childObject.SetActive(!_childObject.activeSelf);

        if (_childObject.activeSelf)
        {
            foreach(var slot in slots)
            {
                if(slot.IsSelect)
                {
                    DisableSlot(slot);
                    break;
                }
            }
        }
    }

    public void DisableSlot(ToolSlot _slot)
    {
        Image fillImage = _slot.transform.GetChild(0).GetComponent<Image>();
        Image toolImage =  _slot.transform.GetChild(1).GetComponent<Image>();
        
        fillImage.color = Color.gray;
        toolImage.color = new Color(1, 1, 1, 0.5f);

        _slot.GetComponent<RectTransform>().localScale = Vector3.one;
        _slot.GetComponent<Image>().color = Color.black;
    }

    public void AbleSlot(ToolSlot _slot)
    {
        Image fillImage = _slot.transform.GetChild(0).GetComponent<Image>();
        Image toolImage = _slot.transform.GetChild(1).GetComponent<Image>();

        fillImage.color = new Color32(157 , 195, 230,255);
        toolImage.color = new Color(0, 0, 0, 255);

        _slot.GetComponent<RectTransform>().localScale = Vector3.one;
        _slot.GetComponent<Image>().color = Color.black;
        _slot.IsSelect = false;
    }
}
