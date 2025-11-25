using System;
using UnityEngine;

[System.Serializable]
public class Mission
{
    public string id;
    public string title;
    public string description;
    public int requiredSteps;
    public int currentSteps;

    // CAMBIO: ahora es un monto
    public int rewardAmount;

    public bool isCompleted;
    public bool isClaimed;
    public MissionType type;
    public int difficultyLevel;
    public DateTime startTime;
    public DateTime endTime;

    public Mission(string id, string title, string description, int requiredSteps, int rewardAmount, MissionType type, int difficultyLevel = 0)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.requiredSteps = requiredSteps;

        // CAMBIO
        this.rewardAmount = rewardAmount;

        this.type = type;
        this.difficultyLevel = difficultyLevel;

        currentSteps = 0;
        isCompleted = false;
        isClaimed = false;

        SetTimeBounds();
    }

    public void SetTimeBounds()
    {
        var now = DateTime.Now;
        startTime = now.Date;

        switch (type)
        {
            case MissionType.Daily:
                endTime = startTime.AddDays(1);
                break;
            case MissionType.Long:
                endTime = startTime.AddDays(3);
                break;
            case MissionType.Weekly:
                endTime = startTime.AddDays(7);
                break;
        }
    }

    public void UpdateProgress(int steps)
    {
        currentSteps = Mathf.Min(steps, requiredSteps);
        isCompleted = currentSteps >= requiredSteps;
    }

    public float GetProgressPercentage()
    {
        return (float)currentSteps / requiredSteps;
    }

    public bool IsExpired()
    {
        return DateTime.Now > endTime;
    }

    public void ClaimReward()
    {
        if (isCompleted && !isClaimed)
        {
            isClaimed = true;
            Debug.Log($"Ganaste: {rewardAmount} monedas 💰");
        }
    }
}

public enum MissionType
{
    Daily,
    Long,
    Weekly
}
