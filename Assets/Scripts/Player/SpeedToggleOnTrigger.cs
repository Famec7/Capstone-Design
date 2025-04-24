using UnityEngine;
using UnityEngine.InputSystem;                         
using UnityEngine.XR.Interaction.Toolkit;              

[RequireComponent(typeof(ActionBasedContinuousMoveProvider))]  
public class SpeedToggleOnTrigger : MonoBehaviour
{
    [Header("References")]
    public ActionBasedContinuousMoveProvider MoveProvider; 
    public InputActionProperty LeftTrigger;                

    [Header("Settings")]
    public float BoostMultiplier = 2f;     

    private float _originalSpeed;    
    private bool _isBoosted = false; 

    void Awake()
    {
        if (MoveProvider == null)
        {
            MoveProvider = GetComponent<ActionBasedContinuousMoveProvider>();
        }

        _originalSpeed = MoveProvider.moveSpeed;
    }

    void OnEnable()
    {
        LeftTrigger.action.started += OnTriggerPressed;
        LeftTrigger.action.canceled += OnTriggerReleased;
    }

    void OnDisable()
    {
        LeftTrigger.action.started -= OnTriggerPressed;
        LeftTrigger.action.canceled -= OnTriggerReleased;
    }

    // 트리거를 누르면 호출
    void OnTriggerPressed(InputAction.CallbackContext ctx)
    {
        if (!_isBoosted)
        {
            MoveProvider.moveSpeed = _originalSpeed * BoostMultiplier;
            StatusManager.Instance.IsRunning = true;
            _isBoosted = true;
        }
    }

    // 트리거에서 손을 떼면 호출
    void OnTriggerReleased(InputAction.CallbackContext ctx)
    {
        if (_isBoosted)
        {
            MoveProvider.moveSpeed = _originalSpeed;
            StatusManager.Instance.IsRunning = false;
            _isBoosted = false;
        }
    }
}
