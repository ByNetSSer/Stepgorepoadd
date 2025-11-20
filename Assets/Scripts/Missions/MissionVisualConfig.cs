using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MissionVisualConfig : MonoBehaviour
{
    [Header("Color Settings")]
    public Color normalColor = Color.white;
    public Color completedColor = new Color(0.2f, 0.8f, 0.2f); // Verde
    public Color claimedColor = new Color(0.5f, 0.5f, 0.5f); // Gris
    public Color expiredColor = new Color(0.8f, 0.2f, 0.2f); // Rojo

    [Header("UI References")]
    public Image missionBackground;
    public TextMeshProUGUI statusText;
    public Slider progressSlider;
    public Button claimButton;
    public TextMeshProUGUI timeRemainingText;

    public void UpdateVisuals(Mission mission)
    {
        // Actualizar colores según estado
        if (mission.isClaimed)
        {
            missionBackground.color = claimedColor;
            statusText.text = "RECLAMADO";
            statusText.color = claimedColor;
        }
        else if (mission.isCompleted)
        {
            missionBackground.color = completedColor;
            statusText.text = "COMPLETADO";
            statusText.color = completedColor;
        }
        else if (mission.IsExpired())
        {
            missionBackground.color = expiredColor;
            statusText.text = "EXPIRO";
            statusText.color = expiredColor;
        }
        else
        {
            missionBackground.color = normalColor;
            statusText.text = "EN PROGRESO";
            statusText.color = normalColor;
        }

        // Actualizar barra de progreso
        progressSlider.value = mission.GetProgressPercentage();

        // Configurar botón
        claimButton.interactable = mission.isCompleted && !mission.isClaimed;
    }
}