using UnityEngine;

public class AttachUIToHands : MonoBehaviour
{
    [Header("손 오브젝트")]
    public Transform Controller;

    [Header("UI 루트")]
    public Transform HandUI;

    [Header("위치 오프셋")]
    public Vector3 Offset = new Vector3(0.05f, 0f, 0.1f);

    [Header("회전 오프셋")]
    public Vector3 rotationEuler = new Vector3(0f, 90f, 0f);

    void Start()
    {
        if (Controller && HandUI)
        {
            HandUI.SetParent(Controller, false);
            HandUI.localPosition = Offset;
            HandUI.localRotation = Quaternion.Euler(rotationEuler);
        }
    }
}