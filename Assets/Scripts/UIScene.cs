using DG.Tweening;
using UnityEngine;

public class UIScene : MonoBehaviour
{
    [Header("Animación de Entrada")]
    public bool hasEnterAnimation = true;
    public float enterDuration = 0.6f;
    public Ease enterEase = Ease.OutBack;
    public Vector3 enterOffset = new Vector3(0, -200f, 0);

    [Header("Animación de Salida")]
    public bool hasExitAnimation = true;
    public float exitDuration = 0.4f;
    public Ease exitEase = Ease.InBack;
    public Vector3 exitOffset = new Vector3(0, 200f, 0);

    private Vector3 initialPos;
    private Tween currentTween;

    private void Awake()
    {
        initialPos = transform.localPosition;
    }

    public void Show(bool instant = false)
    {
        gameObject.SetActive(true);
        currentTween?.Kill();

        if (instant || !hasEnterAnimation)
        {
            transform.localPosition = initialPos;
            return;
        }

        transform.localPosition = initialPos;
        currentTween = transform.DOLocalMove(enterOffset, enterDuration)
            .SetEase(enterEase);
    }

    public void Hide(bool instant = false)
    {
        currentTween?.Kill();

        if (instant || !hasExitAnimation)
        {
            gameObject.SetActive(false);
            return;
        }

        currentTween = transform.DOLocalMove(initialPos + exitOffset, exitDuration)
            .SetEase(exitEase)
            .OnComplete(() => gameObject.SetActive(false));
    }

    public bool IsVisible => gameObject.activeSelf;
}
