using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public float AngularSpeedThreshold = 500f;     // 최소 각속도 (deg/s) 기준
    public float ChoppingAngleThreshold = 0.5f;     // 무기가 내려진 상태인지 판별할 dot product 임계치 (0 ~ 1)

    private Quaternion _lastRotation;              // 이전 프레임의 회전값 저장
    private float _angularSpeed;                   // 매 프레임 계산한 각속도 (deg/s)
    private ItemData _data;

    void Start()
    {
        _lastRotation = transform.rotation;
        _data = GetComponent<GeneralItem>().Data;
    }

    void Update()
    {
        // 이전 프레임과의 회전 차이로부터 각속도를 계산합니다.
        float angle = Quaternion.Angle(_lastRotation, transform.rotation);
        _angularSpeed = angle / Time.deltaTime;
        _lastRotation = transform.rotation;
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log(collider);
        // 나무 등 수확 대상(HarvestableObject)이면 나무 베기 로직 적용
        HarvestableObject harvestable = collider.gameObject.GetComponent<HarvestableObject>();
        if (harvestable != null)
        {
            // 무기의 '눕혀진' 정도를 dot product로 확인합니다.
            // 예를 들어, 무기의 transform.up이 세계 하향(Vector3.down)과 얼마나 정렬되어 있는지를 판단합니다.
            float dot = Mathf.Abs(Vector3.Dot(transform.up, Vector3.down));

            if (dot <= ChoppingAngleThreshold && _angularSpeed >= AngularSpeedThreshold)
            {
                harvestable.Chop(_data.AttackPower);
                harvestable.SetRandomPos();
#if UNITY_EDITOR
                Debug.Log($"{gameObject.name}이(가) {collider.gameObject.name}에 나무 베기 공격 실행! dot: {dot:F2}, 각속도: {_angularSpeed:F1}°/s");
#endif
            }
        }
        else
        {
            IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();
            if (damageable != null && _angularSpeed >= AngularSpeedThreshold)
            {
                damageable.TakeDamage(_data.AttackPower);
#if UNITY_EDITOR
                Debug.Log($"{gameObject.name}이(가) {collider.gameObject.name}에 일반 공격 실행! 각속도: {_angularSpeed:F1}°/s");
#endif
            }
        }
    }
}
