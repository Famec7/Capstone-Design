using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlinkController : MonoBehaviour
{
    public Image image;
    public Sprite filledSprite;
    public Sprite blinkingSprite;
    public Sprite emptySprite;

    public float BlinkInterval { get; set; } = 1f;

    private Coroutine blinkRoutine;
    private bool isBlinking = false;

    public void StartBlink()
    {
        if (isBlinking) return;

        blinkRoutine = StartCoroutine(BlinkLoop());
        isBlinking = true;
    }

    public void StopBlink()
    {
        if (!isBlinking) return;

        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        isBlinking = false;
        SetFilled();
    }

    private IEnumerator BlinkLoop()
    {
        while (true)
        {
            image.sprite = blinkingSprite;
            yield return new WaitForSeconds(BlinkInterval / 2f);
            image.sprite = filledSprite;
            yield return new WaitForSeconds(BlinkInterval / 2f);
        }
    }

    public void SetEmpty()
    {
        StopBlink();
        image.sprite = emptySprite;
    }

    public void SetFilled()
    {
        if (!isBlinking)
        {
            image.sprite = filledSprite;
        }
    }

    public void UpdateBlinkInterval(float interval)
    {
        BlinkInterval = interval;
    }
}
