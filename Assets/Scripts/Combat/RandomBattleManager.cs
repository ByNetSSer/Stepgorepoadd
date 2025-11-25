using DG.Tweening;
using UnityEngine;

public class RandomBattleManager : MonoBehaviour
{
    [Header("Referencias")]
    public StepCounter stepCounter;
    public SceneButton sceneButtonToCombat;
    public CombatManager combatManager;

    [Header("Ajustes de probabilidad")]
    [Range(0f, 1f)]
    public float battleChancePerStep = 0.05f;

    [Header("Tiempo de invulnerabilidad")]
    public float invulnerabilityTime = 10f; // Tiempo en segundos

    private bool battleQueued = false;
    private float invulnerabilityTimer = 0f;
    private bool isInvulnerable = false;

    void Start()
    {
        stepCounter.OnStepsUpdated += OnStepDetected;
    }

    void Update()
    {
        // Actualizar timer de invulnerabilidad
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            if (invulnerabilityTimer <= 0f)
            {
                isInvulnerable = false;
                Debug.Log("?? Invulnerabilidad terminada");
            }
        }
    }

    private void OnStepDetected(int totalSteps)
    {
        // No iniciar batalla si ya hay una en curso o si estamos en modo invulnerable
        if (battleQueued || IsFightActive() || isInvulnerable) return;

        float roll = Random.value;

        if (roll <= battleChancePerStep)
        {
            battleQueued = true;
            StartBattleTransition();
        }
    }

    bool IsFightActive()
    {
        return combatManager != null && combatManager.IsFighting();
    }

    void StartBattleTransition()
    {
        Debug.Log("?? Batalla encontrada, iniciando transición...");
        sceneButtonToCombat.OnClick();

        float delay = sceneButtonToCombat.cloudManager.upDuration +
                      sceneButtonToCombat.cloudManager.bounceDuration +
                      sceneButtonToCombat.cloudManager.holdTime;

        DOVirtual.DelayedCall(delay + 0.1f, StartCombatAfterTransition);
    }

    void StartCombatAfterTransition()
    {
        Debug.Log("?? Transición completada, iniciando combate...");
        combatManager.StartCombat();
        battleQueued = false;
    }

    // Método público para activar la invulnerabilidad después de ganar
    public void ActivateInvulnerability()
    {
        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityTime;
        Debug.Log($"?? Invulnerabilidad activada por {invulnerabilityTime} segundos");
    }

    // Para ver el estado en el inspector (opcional)
    public bool IsInvulnerable() => isInvulnerable;
    public float GetInvulnerabilityTimeLeft() => invulnerabilityTimer;
}