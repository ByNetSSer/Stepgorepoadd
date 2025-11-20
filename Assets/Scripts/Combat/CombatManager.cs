using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public PlayerData player;
    public MonsterSO[] monstersList;

    private MonsterSO currentMonster;

    private int currentHealth;
    private float currentTime;

    private bool isFighting = false;

    private ArrowType currentArrow;
    public CombatUI ui;
    [Header("Configuración")]
    public float wrongPenalty = 1.2f; // tiempo restado al fallar


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
            EndCombat(true);
            return;
        }

        // Genera la siguiente flecha
        GenerateNewArrow();
    }

    // =====================================================
    // INPUT INCORRECTO
    // =====================================================
    public void WrongInput()
    {
        if (!isFighting) return;
        ui.UpdateTime(currentTime);
        currentTime -= wrongPenalty;

        Debug.Log("? Fallo! Tiempo restante: " + currentTime);

        if (currentTime <= 0)
        {
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
    // FINAL DEL COMBATE
    // =====================================================
    public void EndCombat(bool win)
    {
        isFighting = false;

        if (win)
        {
            Debug.Log("?? Ganaste el combate!");

            player.AddCoins(currentMonster.rewardCoins);
            player.AddExp(currentMonster.rewardExp);
        }
        else
        {
            Debug.Log("?? El monstruo escapó.");
        }
    }

    // Para pruebas en PC
    void Update()
    {
        if (!isFighting) return;

        currentTime -= Time.deltaTime;
        ui.UpdateTime(currentTime);
        if (Input.GetKeyDown(KeyCode.UpArrow)) TryInput(ArrowType.Up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) TryInput(ArrowType.Down);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) TryInput(ArrowType.Left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) TryInput(ArrowType.Right);

        if (currentTime <= 0)
        {
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
