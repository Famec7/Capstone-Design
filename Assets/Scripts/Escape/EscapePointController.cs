using UnityEngine;

public class EscapePointController : MonoBehaviour
{
    [Tooltip("탈출 UI 매니저 연결")]
    [SerializeField] private EscapeUIManager uiManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            uiManager.ShowUI(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            uiManager.ShowUI(false);
    }
}