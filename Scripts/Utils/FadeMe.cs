using UnityEngine;
using System.Collections;

public class FadeMe : MonoBehaviour
{
    CanvasGroup canvasGroup;

    private float alphaTime = 2f;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void startFadeOut(float customTime = 2f)
    {
        alphaTime = customTime;

        StartCoroutine("FadeOut");
    }

    public void startFadeIn(float customTime = 2f)
    {
        alphaTime = customTime;

        StartCoroutine("FadeIn");
    }

    IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / alphaTime;
            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / alphaTime;
            yield return null;
        }
    }
}