using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class AutoHidePopup : MonoBehaviour
{
    [Header("팝업 지속시간")]
    [Min(0)]
    [SerializeField]
    private float displayDuration = 2f;
    
    [Header("페이드 옵션")]
    [SerializeField]
    private float fadeInDuration = 0.5f;
    [SerializeField]
    private float fadeOutDuration = 0.5f;
    
    private TextMeshProUGUI _textMeshPro;

    private void Awake()
    {
        if (TryGetComponent(out _textMeshPro))
        {
            var color = _textMeshPro.color;
            color.a = 0;
            _textMeshPro.color = color;
        }
        else
        {
            Debug.LogError("AutoHidePopup: TextMeshProUGUI 컴포넌트가 할당되지 않았습니다.");
        }
    }

    /// <summary>
    /// 팝업을 표시합니다.
    /// </summary>
    public void ShowPopup()
    {
        StopAllCoroutines();
        StartCoroutine(IE_FadeIn());
    }

    private IEnumerator IE_FadeIn()
    {
        float time = 0.0f;

        while (time < fadeInDuration)
        {
            time += Time.deltaTime;
            SetAlpha(Mathf.Lerp(0f, 1f, time / fadeInDuration));
            yield return null;
        }
        
        SetAlpha(1f);
        yield return new WaitForSeconds(displayDuration);
        StartCoroutine(IE_FadeOut());
    }
    
    private IEnumerator IE_FadeOut()
    {
        float time = 0.0f;

        while (time < fadeOutDuration)
        {
            time += Time.deltaTime;
            SetAlpha(Mathf.Lerp(1f, 0f, time / fadeOutDuration));
            yield return null;
        }
        
        SetAlpha(0f);
    }
    
    private void SetAlpha(float alpha)
    {
        if (_textMeshPro != null)
        {
            var color = _textMeshPro.color;
            color.a = alpha;
            _textMeshPro.color = color;
        }
    }
}