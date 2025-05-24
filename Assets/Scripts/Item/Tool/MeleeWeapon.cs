using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public float AngularSpeedThreshold = 500f;     // 최소 각속도 (deg/s) 기준
    public float ChoppingAngleThreshold = 0.5f;     // 무기가 내려진 상태인지 판별할 dot product 임계치 (0 ~ 1)

    private Quaternion _lastRotation;              // 이전 프레임의 회전값 저장
    private float _angularSpeed;                   // 매 프레임 계산한 각속도 (deg/s)
    private ItemData _data;

    [SerializeField]
    private List<HarvestableObjectData> _ignoreObjectDatas;

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

    protected void OnTriggerEnter(Collider collider)
    {
        Debug.Log(collider);
        HarvestableObject harvestable = collider.gameObject.GetComponent<HarvestableObject>();


        if (harvestable != null)
        {
            if (HasAttackIgnoreObject(harvestable.HarvestData)) return;

            float dot = Mathf.Abs(Vector3.Dot(transform.up, Vector3.down));

            if (dot <= ChoppingAngleThreshold && _angularSpeed >= AngularSpeedThreshold)
            {
                harvestable.Chop(_data.AttackPower);
                //harvestable.SetRandomPos();
#if UNITY_EDITOR
                Debug.Log($"{gameObject.name}이(가) {collider.gameObject.name}에 공격 실행! dot: {dot:F2}, 각속도: {_angularSpeed:F1}°/s");
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

    private bool HasAttackIgnoreObject(HarvestableObjectData _harvestable)
    {
        foreach (var harvestable in _ignoreObjectDatas)
        {
            if (_harvestable.name == harvestable.name) return true;
        }

        return false;
    }

}
