using DG.Tweening;
using UnityEngine;

public class RandomBattleManager : MonoBehaviour
{
    [Header("Referencias")]
    public StepCounter stepCounter;
    public SceneButton sceneButtonToCombat; // ? transición a UI de combate
    public CombatManager combatManager;     // ? script de pelea

    [Header("Ajustes de probabilidad")]
    [Range(0f, 1f)]
    public float battleChancePerStep = 0.05f; // 5% por paso

    private bool battleQueued = false;

    void Start()
    {
        stepCounter.OnStepsUpdated += OnStepDetected;
    }

    private void OnStepDetected(int totalSteps)
    {
        if (battleQueued) return;
        if (combatManager != null && IsFightActive()) return;

        float roll = Random.value;

        if (roll <= battleChancePerStep)
        {
            battleQueued = true;
            StartBattleTransition();
        }
    }
    bool IsFightActive()
    {
        // Accede al bool isFighting del CombatManager
        // lo hacemos mediante una función pública para buena práctica
        return combatManager.IsFighting();
    }
    void StartBattleTransition()
    {
        Debug.Log("?? Batalla encontrada, iniciando transición...");

        // Ejecutar transición mediante SceneButton
        sceneButtonToCombat.OnClick();

        // Esperar la animación del CloudManager
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
}
