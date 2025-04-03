using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    private ItemData _data;

    public float AngularSpeedThreshold = 90f;
    // 이전 프레임의 회전값 저장
    private Quaternion _lastRotation;
    // 매 프레임 계산한 각속도 (deg/s)
    private float _angularSpeed;
    private void Start()
    {
        _lastRotation = transform.rotation;
        _data = GetComponent<GeneralItem>().Data;
    }

    void Update()
    {
        float angle = Quaternion.Angle(_lastRotation, transform.rotation);
        _angularSpeed = angle / Time.deltaTime;
        _lastRotation = transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
      
    }

    private void OnTriggerEnter(Collider collider)
    {
        // 회전 속도가 임계치 이상일 때만 공격으로 처리
        if (_angularSpeed < AngularSpeedThreshold)
            return;

        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_data.AttackPower);
#if UNITY_EDITOR
            Debug.Log($"{gameObject.name}이(가) {collider.gameObject.name}에 {_data.AttackPower} 데미지를 입힘 (각속도: {_angularSpeed:F1}°/s)");
#endif
        }
    }
}
