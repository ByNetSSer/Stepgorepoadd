using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUI : MonoBehaviour
{
    [Header("UI - Referencias")]
    public TextMeshProUGUI monsterNameText;
    public Image monsterIconImage;

    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    public Slider timeSlider;
    public TextMeshProUGUI timeText;

    // Llamado desde el CombatManager
    public void SetMonsterData(MonsterSO monster)
    {
        monsterNameText.text = monster.monsterName;
        monsterIconImage.sprite = monster.icon;

        healthSlider.maxValue = monster.maxHealth;
        healthSlider.value = monster.maxHealth;

        timeSlider.maxValue = monster.timeLimit;
        timeSlider.value = monster.timeLimit;

        // Textos iniciales
        healthText.text = monster.maxHealth.ToString();
        timeText.text = monster.timeLimit.ToString("0.0") + "s";
    }

    // Actualizar vida visual
    public void UpdateHealth(int currentHealth)
    {
        healthSlider.value = currentHealth;
        healthText.text = currentHealth.ToString();
    }

    // Actualizar tiempo visual
    public void UpdateTime(float currentTime)
    {
        timeSlider.value = currentTime;
        timeText.text = currentTime.ToString("0.0") + "s";
    }
}
