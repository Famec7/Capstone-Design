using Michsky.MUIP;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

// 재료 타입 정의
public enum MaterialType { Wood, Stone }

[RequireComponent(typeof(BoxCollider))]
public class RepairSlotHighlighter : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image slotBackground;
    public Image itemIcon;

    [Header("Settings")]
    [SerializeField] private MaterialType requiredType;
    [SerializeField] private Sprite woodIconSprite;
    [SerializeField] private Sprite stoneIconSprite;
    [SerializeField] private Color normalColor = new Color(1, 1, 1, 0.8f);
    [SerializeField] private Color validColor = Color.white;
    [SerializeField] private Color invalidColor = Color.white;
    [SerializeField] private Color validBorderColor = Color.yellow;
    [SerializeField] private Color invalidBorderColor = Color.red;
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float highlightScale = 1.1f;
    
    private static RepairSlotHighlighter activeSlot;
    private Outline outline;
    private bool hovering;
    private bool isValid;
    private XRGrabInteractable grabbed;

    private void Awake()
    {
        if (itemIcon == null)
            Debug.LogError($"[{name}] itemIcon(Image) 참조가 빠졌습니다!");
        else
        {
            itemIcon.sprite = (requiredType == MaterialType.Wood) ? woodIconSprite : stoneIconSprite;
            itemIcon.color = new Color(1, 1, 1, 0.3f);
        }

        if (slotBackground == null)
            Debug.LogError($"[{name}] slotBackground(Image) 참조가 빠졌습니다!");
        else
        {
            outline = slotBackground.GetComponent<Outline>();
            if (outline == null)
                Debug.LogError($"[{name}] slotBackground에 Outline 컴포넌트가 없습니다!");
        }

        var col = GetComponent<BoxCollider>();
        if (col == null)
            Debug.LogError($"[{name}] BoxCollider가 없습니다!");
        else
            col.isTrigger = true;

        SetNormal();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activeSlot != null || itemIcon.color.a >= 1f)
            return;

        var grab = other.GetComponent<XRGrabInteractable>();
        var mat = other.GetComponent<RepairMaterial>();
        if (grab == null || mat == null || !grab.isSelected)
            return;

        activeSlot = this;
        grabbed = grab;
        grabbed.selectExited.AddListener(OnItemReleased);

        hovering = true;
        isValid = (mat.type == requiredType);
        if (isValid) SetValid(); else SetInvalid();
    }

    private void OnTriggerExit(Collider other)
    {
        if (grabbed != null)
            grabbed.selectExited.RemoveListener(OnItemReleased);

        if (activeSlot == this)
            activeSlot = null;

        hovering = false;
        SetNormal();
        grabbed = null;
    }

    private void OnItemReleased(SelectExitEventArgs args)
    {
        if (!hovering)
            return;

        if (isValid)
        {
            EscapeUIManager.Instance.RegisterMaterial(requiredType, itemIcon.sprite);
            Destroy(grabbed.gameObject);
        }

        if (activeSlot == this)
            activeSlot = null;

        SetNormal();
        hovering = false;

        if (grabbed != null)
            grabbed.selectExited.RemoveListener(OnItemReleased);
    }

    private void SetNormal()
    {
        slotBackground.color = normalColor;
        outline.effectColor = Color.clear;
        transform.localScale = Vector3.one * normalScale;
    }

    private void SetValid()
    {
        slotBackground.color = validColor;
        outline.effectColor = validBorderColor;
        transform.localScale = Vector3.one * highlightScale;
    }

    private void SetInvalid()
    {
        slotBackground.color = invalidColor;
        outline.effectColor = invalidBorderColor;
        transform.localScale = Vector3.one * highlightScale;
    }
}
