using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [Header("UI")]
    public TextMeshProUGUI coinsText;

    [Header("Player Currency")]
    public int coins = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional si quieres que sobreviva escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadCoins();
        UpdateUI();
    }

    // ============================================================
    //      AGREGAR MONEDAS (GANAR RECOMPENSAS)
    // ============================================================
    public void AddCoins(int amount)
    {
        coins += amount;
        SaveCoins();
        UpdateUI();

        Debug.Log("💰 Monedas añadidas: " + amount + " | Total = " + coins);
    }

    // ============================================================
    //                   SETEAR EL TEXTO
    // ============================================================
    public void UpdateUI()
    {
        if (coinsText != null)
            coinsText.text = coins.ToString();
    }

    // ============================================================
    //                GUARDADO / CARGA OPCIONAL
    // ============================================================
    private void SaveCoins()
    {
        PlayerPrefs.SetInt("player_coins", coins);
    }

    private void LoadCoins()
    {
        coins = PlayerPrefs.GetInt("player_coins", 0);
    }
}
