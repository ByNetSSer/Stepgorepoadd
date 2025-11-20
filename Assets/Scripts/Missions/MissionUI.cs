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

        titleText.text = currentMission.title;
        descriptionText.text = currentMission.description;
        rewardText.text = $"Recompensa: {currentMission.rewardDescription}";

        // Actualizar progreso
        progressText.text = $"{currentMission.currentSteps}/{currentMission.requiredSteps}";
        progressSlider.value = currentMission.GetProgressPercentage();

        // Configurar botón de reclamar
        claimButton.interactable = currentMission.isCompleted && !currentMission.isClaimed;
        claimButton.gameObject.SetActive(!currentMission.isClaimed);

        if (currentMission.isClaimed)
        {
            claimButton.GetComponentInChildren<TextMeshProUGUI>().text = "Reclamado";
            claimButton.interactable = false;
        }
        else
        {
            claimButton.GetComponentInChildren<TextMeshProUGUI>().text = "Reclamar";
        }

        // Actualizar tiempo restante
        UpdateTimeRemaining();
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
        if (currentMission != null && currentMission.isCompleted && !currentMission.isClaimed)
        {
            MissionManager.Instance.ClaimMissionReward(currentMission);
            UpdateUI();
        }
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