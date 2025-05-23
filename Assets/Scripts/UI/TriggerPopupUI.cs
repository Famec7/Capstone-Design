using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerPopupUI : MonoBehaviour
{
    [Header("팝업 UI")]
    [SerializeField]
    private GameObject popupUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 팝업 UI 활성화
            popupUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 팝업 UI 비활성화
            popupUI.SetActive(false);
        }
    }
}