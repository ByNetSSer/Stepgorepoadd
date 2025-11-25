using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI progressText;
    public Slider progressSlider;
    public Button claimButton;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI timeRemainingText;

    private Mission currentMission;

    public void Initialize(Mission mission)
    {
        currentMission = mission;
        UpdateUI();

        claimButton.onClick.AddListener(OnClaimButtonClicked);
    }

    void UpdateUI()
    {
        if (currentMission == null) return;

        // Información básica
        titleText.text = currentMission.title;
        descriptionText.text = currentMission.description;
        rewardText.text = $"Recompensa: {currentMission.rewardAmount}";

        // Progreso
        progressText.text = $"{currentMission.currentSteps}/{currentMission.requiredSteps}";
        progressSlider.value = currentMission.GetProgressPercentage();

        // Botón de reclamar
        claimButton.interactable = currentMission.isCompleted && !currentMission.isClaimed;

        // Mostrar tiempo restante
        TimeSpan remaining = currentMission.endTime - DateTime.Now;

        if (remaining.TotalSeconds > 0)
        {
            if (remaining.TotalHours >= 1)
                timeRemainingText.text = $"{Mathf.FloorToInt((float)remaining.TotalHours)}h {remaining.Minutes}m restantes";
            else
                timeRemainingText.text = $"{remaining.Minutes}m {remaining.Seconds}s restantes";
        }
        else
        {
            timeRemainingText.text = "Expirada";
        }
    }

    void UpdateTimeRemaining()
    {
        if (currentMission.IsExpired())
        {
            timeRemainingText.text = "Expirada";
            timeRemainingText.color = Color.red;
        }
        else
        {
            System.TimeSpan timeLeft = currentMission.endTime - DateTime.Now;

            if (timeLeft.TotalDays >= 1)
            {
                timeRemainingText.text = $"{timeLeft.Days}d {timeLeft.Hours}h";
            }
            else
            {
                timeRemainingText.text = $"{timeLeft.Hours}h {timeLeft.Minutes}m";
            }

            timeRemainingText.color = Color.green;
        }
    }

    void OnClaimButtonClicked()
    {
        if (currentMission == null) return;

        MissionManager.Instance.ClaimMissionReward(currentMission);
        UpdateUI();
    }

    void Update()
    {
        // Actualizar tiempo restante en tiempo real
        if (currentMission != null && !currentMission.isClaimed)
        {
            UpdateTimeRemaining();
        }
    }
}