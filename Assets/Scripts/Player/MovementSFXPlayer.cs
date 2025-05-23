using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MovementSFXPlayer : MonoBehaviour
{
    [Header("References")]
    // 연속 이동 프로바이더
    public ActionBasedContinuousMoveProvider moveProvider;
    // 효과음 재생용 AudioSource
    public AudioSource sfxSource;
    // 발소리 클립
    public AudioClip[] footstepClip;

    [Header("Settings")]
    // 발소리 재생 간격 (초)
    public float playInterval = 2f;

    private Coroutine sfxRoutine;

    void OnEnable()
    {
        // 이동 시작/종료 이벤트 구독
        moveProvider.beginLocomotion += OnStartLocomotion;
        moveProvider.endLocomotion += OnEndLocomotion;
    }

    void OnDisable()
    {
        moveProvider.beginLocomotion -= OnStartLocomotion;
        moveProvider.endLocomotion -= OnEndLocomotion;
    }

    private void OnStartLocomotion(LocomotionSystem system)
    {
        // 이미 재생 중이 아니면 코루틴 시작
        if (sfxRoutine == null)
            sfxRoutine = StartCoroutine(PlayFootstepSFX());
    }

    private void OnEndLocomotion(LocomotionSystem system)
    {

    }

    private IEnumerator PlayFootstepSFX()
    {
        int randomValue = Random.Range(0, 3);
        sfxSource.PlayOneShot(footstepClip[randomValue]);
        yield return new WaitForSeconds(playInterval);
        sfxRoutine = null;
    }
}
