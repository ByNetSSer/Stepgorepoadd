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

    [Header("Flecha Única")]
    public Image arrowImage;

    [Header("Sprites de Flechas")]
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;



    // -----------------------------------------------------------
    // DATOS DEL MONSTRUO
    // -----------------------------------------------------------
    public void SetMonsterData(MonsterSO monster)
    {
        monsterNameText.text = monster.monsterName;
        monsterIconImage.sprite = monster.icon;

        healthSlider.maxValue = monster.maxHealth;
        healthSlider.value = monster.maxHealth;

        timeSlider.maxValue = monster.timeLimit;
        timeSlider.value = monster.timeLimit;

        healthText.text = monster.maxHealth.ToString();
        timeText.text = monster.timeLimit.ToString("0.0") + "s";
    }

    // -----------------------------------------------------------
    // ACTUALIZAR VIDA
    // -----------------------------------------------------------
    public void UpdateHealth(int currentHealth)
    {
        healthSlider.value = currentHealth;
        healthText.text = currentHealth.ToString();
    }

    // -----------------------------------------------------------
    // ACTUALIZAR TIEMPO
    // -----------------------------------------------------------
    public void UpdateTime(float currentTime)
    {
        timeSlider.value = currentTime;
        timeText.text = currentTime.ToString("0.0") + "s";
    }


    // -----------------------------------------------------------
    // MOSTRAR FLECHA ÚNICA
    // -----------------------------------------------------------
    public void ShowArrow(ArrowType type)
    {
        switch (type)
        {
            case ArrowType.Up:
                arrowImage.sprite = spriteUp;
                break;

            case ArrowType.Down:
                arrowImage.sprite = spriteDown;
                break;

            case ArrowType.Left:
                arrowImage.sprite = spriteLeft;
                break;

            case ArrowType.Right:
                arrowImage.sprite = spriteRight;
                break;
        }
    }
}
