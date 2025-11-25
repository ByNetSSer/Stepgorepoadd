using UnityEngine;

using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public PlayerData player;
    public MonsterSO[] monstersList;
    public RandomBattleManager randomBattleManager;

    private MonsterSO currentMonster;
    private int currentHealth;
    private float currentTime;
    private bool isFighting = false;

    public CloudManager cloudManager;
    private ArrowType currentArrow;
    public CombatUI ui;

    [Header("Configuración")]
    public float wrongPenalty = 1.2f;

    // =====================================================
    // INICIO DEL COMBATE
    // =====================================================
    public void StartCombat()
    {
        if (isFighting) return;

        currentMonster = monstersList[Random.Range(0, monstersList.Length)];
        currentHealth = currentMonster.maxHealth;
        currentTime = currentMonster.timeLimit;
        ui.SetMonsterData(currentMonster);
        GenerateNewArrow();

        isFighting = true;

        Debug.Log("?? Pelea contra: " + currentMonster.monsterName);
        Debug.Log("Primera flecha: " + currentArrow);
    }

    // =====================================================
    // GENERAR FLECHA ALEATORIA
    // =====================================================
    private void GenerateNewArrow()
    {
        int r = Random.Range(0, 4);
        currentArrow = (ArrowType)r;

        Debug.Log("Nueva flecha generada: " + currentArrow);
        ui.ShowArrow(currentArrow);
    }

    // =====================================================
    // INPUT CORRECTO
    // =====================================================
    public void CorrectInput()
    {
        if (!isFighting) return;

        int damage = player.GetDamage() - currentMonster.resistance;
        if (damage < 1) damage = 1;

        currentHealth -= damage;
        ui.UpdateHealth(currentHealth);
        Debug.Log("? Correcto! Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            ui.UpdateHealth(currentHealth);
            EndCombat(true);
            return;
        }

        GenerateNewArrow();
    }

    // =====================================================
    // INPUT INCORRECTO
    // =====================================================
    public void WrongInput()
    {
        if (!isFighting) return;

        currentTime -= wrongPenalty;
        ui.UpdateTime(currentTime);

        Debug.Log("? Fallo! Tiempo restante: " + currentTime);

        if (currentTime <= 0)
        {
            currentTime = 0;
            ui.UpdateTime(currentTime);
            EndCombat(false);
        }
    }

    // =====================================================
    // VALIDAR INPUT
    // =====================================================
    public void TryInput(ArrowType pressed)
    {
        if (!isFighting) return;

        if (pressed == currentArrow)
            CorrectInput();
        else
            WrongInput();
    }

    // =====================================================
    // FINAL DEL COMBATE - ACTUALIZADO CON RECOMPENSAS
    // =====================================================
    public void EndCombat(bool win)
    {
        if (!isFighting) return;

        isFighting = false;

        if (win)
        {
            Debug.Log("GANASTE el combate!");

            // 🔥 DAR RECOMPENSA POR DERROTAR MONSTRUO
            GiveCombatReward();

            // ACTIVAR INVULNERABILIDAD
            if (randomBattleManager != null)
            {
                randomBattleManager.ActivateInvulnerability();
            }
        }
        else
        {
            Debug.Log("PERDISTE el combate...");
        }

        Invoke("TriggerCloudTransition", 0.1f);
    }

    // =====================================================
    // DAR RECOMPENSA POR COMBATE
    // =====================================================
    private void GiveCombatReward()
    {
        if (currentMonster == null) return;

        // Calcular recompensa base del monstruo
        int baseReward = currentMonster.rewardCoins;

        // Opcional: agregar bono por tiempo restante
        int timeBonus = Mathf.RoundToInt(currentTime * 2f);

        int totalReward = baseReward + timeBonus;

        // Dar las monedas al jugador
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoins(totalReward);
            Debug.Log($"💰 Recompensa de combate: {baseReward} + {timeBonus} (bonus tiempo) = {totalReward} monedas");
        }
        else
        {
            Debug.LogWarning("CurrencyManager no encontrado para dar recompensa");
        }
    }

    private void TriggerCloudTransition()
    {
        cloudManager.PlayCombatTransition(true);
    }

    // Para pruebas en PC
    void Update()
    {
        if (!isFighting) return;

        currentTime -= Time.deltaTime;
        ui.UpdateTime(currentTime);

        // Inputs para PC
        if (Input.GetKeyDown(KeyCode.UpArrow)) TryInput(ArrowType.Up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) TryInput(ArrowType.Down);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) TryInput(ArrowType.Left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) TryInput(ArrowType.Right);

        if (currentTime <= 0)
        {
            currentTime = 0;
            ui.UpdateTime(currentTime);
            Debug.Log("? El monstruo escapó");
            EndCombat(false);
        }
    }

    public bool IsFighting()
    {
        return isFighting;
    }
}
public enum ArrowType
{
    Up,
    Down,
    Left,
    Right
}
