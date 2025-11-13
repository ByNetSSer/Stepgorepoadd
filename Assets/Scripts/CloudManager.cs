using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class CloudManager : MonoBehaviour
{
    [Header("Configuración de Nubes")]
    public List<Cloud> clouds = new List<Cloud>();
    public float targetOffsetY = -1300f;

    [Header("Duraciones")]
    public float upDuration = 0.8f;
    public float bounceDuration = 0.4f;
    public float downDuration = 1.0f;
    public float delayStep = 0.05f;
    public float holdTime = 1.5f;

    // Estado global
    public enum CloudState { Hidden, Visible, Animating }
    public CloudState state = CloudState.Visible;
    private bool isAnimating => state == CloudState.Animating;

    private void Start()
    {
        // Inicializar posiciones
        foreach (Cloud c in clouds)
        {
            Vector3 targetPos = new Vector3(c.transform.localPosition.x, targetOffsetY, 0f);
            c.Initialize(targetPos);
        }
        state = CloudState.Visible;
    }

    public bool TryHide() => TryHide(false);
    public bool TryShow() => TryShow(false);
    public bool TryHide(bool ignoreLock)
    {
        if (isAnimating && !ignoreLock) return false;
        if (state == CloudState.Hidden && !ignoreLock) return false;

        StartHide(ignoreLock);
        return true;
    }

    public bool TryShow(bool ignoreLock)
    {
        if (isAnimating && !ignoreLock) return false;
        if (state == CloudState.Visible && !ignoreLock) return false;

        StartShow(ignoreLock);
        return true;
    }

    public bool TryToggle()
    {
        if (isAnimating) return false;
        if (state == CloudState.Visible) return TryHide(false);
        else if (state == CloudState.Hidden) return TryShow(false);
        return false;
    }
    public void StartShow(bool ignoreLock)
    {
        state = CloudState.Animating;

        float maxEndTime = 0f;
        // Orden por Y ascendente para que nubes bajas suban primero si quieres
        var ordered = clouds.OrderBy(c => c.startPosition.y).ToList();

        for (int i = 0; i < ordered.Count; i++)
        {
            Cloud c = ordered[i];
            float delay = i * delayStep;
            Tween t = c.MoveUpTween(upDuration, bounceDuration, delay);
            float endTime = delay + upDuration + bounceDuration;
            if (endTime > maxEndTime) maxEndTime = endTime;
        }

        // Cuando todas terminen, pasar a Visible
        DOVirtual.DelayedCall(maxEndTime, () =>
        {
            state = CloudState.Visible;
        });
    }

    public void StartHide(bool ignoreLock = false)
    {
        state = CloudState.Animating;

        float maxEndTime = 0f;
        // Orden por Y descendente para que las más arriba se vayan primero (opcional)
        var ordered = clouds.OrderByDescending(c => c.startPosition.y).ToList();

        for (int i = 0; i < ordered.Count; i++)
        {
            Cloud c = ordered[i];
            float delay = i * delayStep;
            Tween t = c.MoveDownTween(downDuration, delay);
            float endTime = delay + downDuration;
            if (endTime > maxEndTime) maxEndTime = endTime;
        }

        DOVirtual.DelayedCall(maxEndTime, () =>
        {
            state = CloudState.Hidden;
        });
    }

    // --- Transición de carga: muestra -> espera -> esconde
    public bool PlayLoadingTransition()
    {
        if (isAnimating) return false;

        // Forzamos la secuencia: Show (ignore lock) -> hold -> Hide (ignore lock)
        state = CloudState.Animating;
        float showMax = 0f;
        var orderedShow = clouds.OrderBy(c => c.startPosition.y).ToList();
        for (int i = 0; i < orderedShow.Count; i++)
        {
            Cloud c = orderedShow[i];
            float delay = i * delayStep;
            Tween t = c.MoveUpTween(upDuration, bounceDuration, delay);
            float endTime = delay + upDuration + bounceDuration;
            if (endTime > showMax) showMax = endTime;
        }

        // 2) Después de showMax + holdTime lanzar hide
        DOVirtual.DelayedCall(showMax + holdTime, () =>
        {
            float hideMax = 0f;
            var orderedHide = clouds.OrderByDescending(c => c.startPosition.y).ToList();
            for (int i = 0; i < orderedHide.Count; i++)
            {
                Cloud c = orderedHide[i];
                float delay = i * delayStep;
                Tween t = c.MoveDownTween(downDuration, delay);
                float endTime = delay + downDuration;
                if (endTime > hideMax) hideMax = endTime;
            }

            // 3) Al terminar todo, setear estado Hidden
            DOVirtual.DelayedCall(hideMax, () =>
            {
                state = CloudState.Hidden;
            });
        });

        return true;
    }

    // Helpers útiles
    public bool IsInteractable => state == CloudState.Hidden; // tus botones pueden leer esto antes de ejecutar acciones
    public void ForceHideInstant()
    {
        foreach (var c in clouds) c.ForceHideInstant();
        state = CloudState.Hidden;
    }
    public void ForceShowInstant()
    {
        foreach (var c in clouds) c.ResetInstant();
        state = CloudState.Visible;
    }
}
