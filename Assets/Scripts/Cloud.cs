using DG.Tweening;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Vector3 targetPosition;
    public float overshoot = 60f;      // cuánto pasan hacia arriba en el "lego" effect
    public bool isMoving = false;      // protección por nube

    public void Initialize(Vector3 target)
    {
        startPosition = transform.localPosition;
        targetPosition = target;
    }

    // Retorna el tween para MoveUp (subida con rebote).
    // NOTA: el tween se reproducirá automáticamente al ser creado.
    public Tween MoveUpTween(float upDuration, float bounceDuration, float delay = 0f)
    {
        if (isMoving) { /* aún así creamos el tween para coordinar, pero no evitamos */ }
        isMoving = true;

        float midY = startPosition.y + overshoot;

        Sequence seq = DOTween.Sequence();
        seq.SetDelay(delay);
        seq.Append(transform.DOLocalMoveY(midY, upDuration).SetEase(Ease.OutSine));
        seq.Append(transform.DOLocalMoveY(startPosition.y, bounceDuration).SetEase(Ease.InOutSine));
        seq.OnComplete(() => isMoving = false);

        return seq;
    }

    // Retorna el tween para MoveDown (bajar fuera de escena).
    public Tween MoveDownTween(float downDuration, float delay = 0f)
    {
        if (isMoving) { /* ok */ }
        isMoving = true;

        Tween t = transform.DOLocalMoveY(targetPosition.y, downDuration)
            .SetEase(Ease.InOutSine)
            .SetDelay(delay)
            .OnComplete(() => isMoving = false);

        return t;
    }

    // Utility: reset instantáneo a posición inicial
    public void ResetInstant()
    {
        transform.DOKill(true);
        transform.localPosition = startPosition;
        isMoving = false;
    }

    // Utility: force hide instant
    public void ForceHideInstant()
    {
        transform.DOKill(true);
        transform.localPosition = targetPosition;
        isMoving = false;
    }
}
