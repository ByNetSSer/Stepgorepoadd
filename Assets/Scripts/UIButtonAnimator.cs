using DG.Tweening;
using UnityEngine;

public class UIButtonAnimator : MonoBehaviour
{
    [Header("Configuración de Animación")]
    public bool isVisible = true;
    public float fadeDuration = 0.5f;
    public float pulseScale = 1.05f;
    public float pulseSpeed = 1.5f;
    public float rotationAmount = 5f;
    public float rotationSpeed = 3f;

    private RectTransform rect;
    public CanvasGroup canvasGroup;
    private Tween pulseTween;
    private Tween rotationTween;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

    }

    void Start()
    {
        // Iniciar animaciones sutiles
        StartIdleAnimations();
    }

    public void StartIdleAnimations()
    {
        // Pulso de escala
        pulseTween?.Kill();
        pulseTween = rect
            .DOScale(pulseScale, pulseSpeed)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // Rotación izquierda-derecha
        rotationTween?.Kill();
        rotationTween = rect
            .DORotate(new Vector3(0, 0, rotationAmount), rotationSpeed)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void StopIdleAnimations()
    {
        pulseTween?.Kill();
        rotationTween?.Kill();
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }


    public void ToggleVisibility(bool show)
    {
        isVisible = show;
        canvasGroup.DOFade(show ? 1f : 0f, fadeDuration)
                   .SetEase(Ease.InOutQuad);
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
    }

    public void MoveTo(Vector3 targetPos)
    {
        rect.DOAnchorPos(targetPos, 0.8f)
            .SetEase(Ease.OutBounce);
    }
}
